﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedback
{
    public class CreateApprenticeFeedbackHandler : IRequestHandler<CreateApprenticeFeedbackCommand, CreateApprenticeFeedbackResponse>
    {
        private readonly IApprenticeFeedbackTargetContext _apprenticeFeedbackTargetContext;
        private readonly IApprenticeFeedbackResultContext _apprenticeFeedbackResultContext;
        private readonly IAttributeContext _attributeContext;
        private readonly ILogger<CreateApprenticeFeedbackHandler> _logger;
        private readonly IDateTimeHelper _timeHelper;

        public CreateApprenticeFeedbackHandler(
            IApprenticeFeedbackTargetContext apprenticeFeedbackTargetContext,
            IApprenticeFeedbackResultContext apprenticeFeedbackResultContext,
            IAttributeContext attributeContext,
            IDateTimeHelper timeHelper,
            ILogger<CreateApprenticeFeedbackHandler> logger)
        {
            _apprenticeFeedbackTargetContext = apprenticeFeedbackTargetContext;
            _apprenticeFeedbackResultContext = apprenticeFeedbackResultContext;
            _attributeContext = attributeContext;
            _timeHelper = timeHelper;
            _logger = logger;
        }

        public async Task<CreateApprenticeFeedbackResponse> Handle(CreateApprenticeFeedbackCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Fetch ApprenticeFeedbackTarget record by Id. Id used: {request.ApprenticeFeedbackTargetId}");
            var apprenticeFeedbackTarget = await _apprenticeFeedbackTargetContext.FindByIdAndIncludeFeedbackResultsAsync(request.ApprenticeFeedbackTargetId);

            if (apprenticeFeedbackTarget == null)
            {
                _logger.LogError($"No ApprenticeFeedbackTarget record found. ApprenticeFeedbackTargetId: {request.ApprenticeFeedbackTargetId}");
                throw new InvalidOperationException($"Apprentice Feedback Target not found. ApprenticeFeedbackTargetId: {request.ApprenticeFeedbackTargetId}");
            }

            await ValidateProviderAttributes(request);

            // Create Feedback for new submission
            var now = _timeHelper.Now;
            var feedback = new Domain.Entities.ApprenticeFeedbackResult()
            {
                ApprenticeFeedbackTargetId = apprenticeFeedbackTarget.Id,
                StandardUId = apprenticeFeedbackTarget.StandardUId,
                DateTimeCompleted = now,
                ProviderRating = request.OverallRating.ToString(),
                ProviderAttributes = request.FeedbackAttributes.
                    Select(s => new Domain.Entities.ProviderAttribute { AttributeId = s.Id, AttributeValue = (int)s.Status }).ToList(),
                AllowContact = request.AllowContact
            };

            _apprenticeFeedbackResultContext.Add(feedback);
            apprenticeFeedbackTarget.UpdateTargetAfterFeedback(now);
            await _apprenticeFeedbackResultContext.SaveChangesAsync();

            _logger.LogDebug($"Successfully created feedback object with Id: {feedback.Id}");
            return new CreateApprenticeFeedbackResponse() { ApprenticeFeedbackResultId = feedback.Id };
        }

        private async Task ValidateProviderAttributes(CreateApprenticeFeedbackCommand request)
        {
            var validAttributes = await _attributeContext.Entities.ToListAsync();
            var validAttributesIds = validAttributes.Select(a => a.AttributeId).ToList();
            var providedAttributesIds = request.FeedbackAttributes.Select(a => a.Id).ToList();
            var invalidAttributesIds = providedAttributesIds.Except(validAttributesIds).ToList();

            if (invalidAttributesIds.Count > 0)
            {
                var attributesProvidedNames = string.Join(", ", request.FeedbackAttributes.Select(a => a.Name));
                string invalidAttributeNamesOutput = CreateInvalidOutput(request, invalidAttributesIds);
                _logger.LogError($"Some or all of the attributes supplied to create the feedback record do not exist in the database. Attributes provided in the request: {attributesProvidedNames}, the following attributes are invalid: {invalidAttributeNamesOutput}");
                throw new InvalidOperationException($"Some or all of the attributes supplied to create the feedback record do not exist in the database. Attributes provided in the request: {attributesProvidedNames}, the following attributes are invalid: {invalidAttributeNamesOutput}");
            }
        }

        private string CreateInvalidOutput(CreateApprenticeFeedbackCommand request, List<int> invalidAttributesIds)
        {
            var invalidAttributesNames = (from a in request.FeedbackAttributes
                                          where invalidAttributesIds.Contains(a.Id)
                                          select a.Name).ToList();
            return string.Join(", ", invalidAttributesNames);
        }
    }
}