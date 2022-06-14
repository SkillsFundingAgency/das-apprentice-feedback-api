using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using SFA.DAS.ApprenticeFeedback.Domain.Models;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedback
{
    public class CreateApprenticeFeedbackHandler : IRequestHandler<CreateApprenticeFeedbackCommand, CreateApprenticeFeedbackResponse>
    {
        public readonly IApprenticeFeedbackRepository _apprenticeFeedbackRepository;
        public readonly ILogger<CreateApprenticeFeedbackHandler> _logger;
        public readonly IDateTimeHelper _timeHelper;
        private readonly IApprenticeFeedbackTargetContext _dbContext;

        public CreateApprenticeFeedbackHandler(IApprenticeFeedbackRepository apprenticeFeedbackRepository, IApprenticeFeedbackTargetContext dbContext, IDateTimeHelper timeHelper, ILogger<CreateApprenticeFeedbackHandler> logger)
        {
            _apprenticeFeedbackRepository = apprenticeFeedbackRepository;
            _dbContext = dbContext;
            _timeHelper = timeHelper;
            _logger = logger;
        }

        public async Task<CreateApprenticeFeedbackResponse> Handle(CreateApprenticeFeedbackCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Fetch ApprenticeFeedbackTarget record by Id. Id used: {request.ApprenticeFeedbackTargetId}");
            ApprenticeFeedbackTarget apprenticeFeedbackTarget = await _dbContext.GetApprenticeFeedbackTargetByIdAsync(request.ApprenticeFeedbackTargetId);

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
                ApprenticeFeedbackTargetId = apprenticeFeedbackTarget.Id.Value,
                StandardUId = apprenticeFeedbackTarget.StandardUId,
                DateTimeCompleted = now,
                ProviderRating = request.OverallRating.ToString(),
                ProviderAttributes = request.FeedbackAttributes.
                    Select(s => new Domain.Entities.ProviderAttribute { AttributeId = s.Id, AttributeValue = (int)s.Status }).ToList(),
                AllowContact = request.AllowContact
            };
            feedback = await _apprenticeFeedbackRepository.CreateApprenticeFeedbackResult(feedback);

            // Update Feedback Target with new status
            apprenticeFeedbackTarget.UpdateTargetAfterFeedback(now);
            await _apprenticeFeedbackRepository.UpdateApprenticeFeedbackTarget(apprenticeFeedbackTarget);

            _logger.LogDebug($"Successfully created feedback object with Id: {feedback.Id}");
            return new CreateApprenticeFeedbackResponse() { ApprenticeFeedbackResultId = feedback.Id };
        }

        private async Task ValidateProviderAttributes(CreateApprenticeFeedbackCommand request)
        {
            var validAttributes = await _apprenticeFeedbackRepository.GetAttributes();
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