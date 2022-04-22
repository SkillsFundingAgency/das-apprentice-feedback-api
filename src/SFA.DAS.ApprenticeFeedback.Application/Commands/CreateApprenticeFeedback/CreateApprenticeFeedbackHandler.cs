using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedback
{
    public class CreateApprenticeFeedbackHandler : IRequestHandler<CreateApprenticeFeedbackCommand, CreateApprenticeFeedbackResponse>
    {
        public readonly IApprenticeFeedbackRepository _apprenticeFeedbackRepository;
        public readonly ILogger<CreateApprenticeFeedbackHandler> _logger;
        public readonly IDateTimeHelper _timeHelper;

        public CreateApprenticeFeedbackHandler(IApprenticeFeedbackRepository apprenticeFeedbackRepository, IDateTimeHelper timeHelper, ILogger<CreateApprenticeFeedbackHandler> logger)
        {
            _apprenticeFeedbackRepository = apprenticeFeedbackRepository;
            _timeHelper = timeHelper;
            _logger = logger;
        }

        public async Task<CreateApprenticeFeedbackResponse> Handle(CreateApprenticeFeedbackCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Fetch ApprenticeFeedbackTarget record by Id. Id used: {request.ApprenticeFeedbackTargetId}");
            var apprenticeFeedbackTarget = await _apprenticeFeedbackRepository.GetApprenticeFeedbackTargetById(request.ApprenticeFeedbackTargetId);

            if (apprenticeFeedbackTarget == null)
            {
                _logger.LogError($"No ApprenticeFeedbackTarget record found. ApprenticeFeedbackTargetId: {request.ApprenticeFeedbackTargetId}");
                throw new InvalidOperationException($"Apprentice Feedback Target not found. ApprenticeFeedbackTargetId: {request.ApprenticeFeedbackTargetId}");
            }

            var validAttributes = await _apprenticeFeedbackRepository.GetAttributes();
            var validAttributesIds = validAttributes.Select(a => a.AttributeId).ToList();
            var providedAttributesIds = request.FeedbackAttributes.Select(a => a.Id).ToList();
            var invalidAttributesIds = providedAttributesIds.Except(validAttributesIds).ToList();

            if (invalidAttributesIds.Count > 0)
            {
                var attributesProvidedNames = GetAttributeNames(request);
                string invalidAttributeNamesOutput = CreateInvalidOutput(request, invalidAttributesIds);
                _logger.LogError($"Some or all of the attributes supplied to create the feedback record do not exist in the database. Attributes provided in the request: {attributesProvidedNames}, the following attributes are invalid: {invalidAttributeNamesOutput}");
                throw new InvalidOperationException($"Some or all of the attributes supplied to create the feedback record do not exist in the database. Attributes provided in the request: {attributesProvidedNames}, the following attributes are invalid: {invalidAttributeNamesOutput}");
            }

            var feedback = CreateApprenticeFeedbackResult(request, apprenticeFeedbackTarget);

            feedback = await _apprenticeFeedbackRepository.CreateApprenticeFeedbackResult(feedback);

            _logger.LogDebug($"Successfully created feedback object with Id: {feedback.Id}");
            return new CreateApprenticeFeedbackResponse() { ApprenticeFeedbackResultId = feedback.Id };
        }

        private string GetAttributeNames(CreateApprenticeFeedbackCommand request)
        {
            var attributeNames = request.FeedbackAttributes.Select(a => a.Name).ToList();
            return string.Join(", ", attributeNames);
        }

        private string CreateInvalidOutput(CreateApprenticeFeedbackCommand request, List<int> invalidAttributesIds)
        {
            var invalidAttributesNames = (from a in request.FeedbackAttributes
                                          where invalidAttributesIds.Contains(a.Id)
                                          select a.Name).ToList();
            return String.Join(", ", invalidAttributesNames);
        }

        private Domain.Entities.ApprenticeFeedbackResult CreateApprenticeFeedbackResult(CreateApprenticeFeedbackCommand request, Domain.Entities.ApprenticeFeedbackTarget apprenticeFeedbackTarget)
        {
            return new Domain.Entities.ApprenticeFeedbackResult()
            {
                ApprenticeFeedbackTargetId = apprenticeFeedbackTarget.Id,
                StandardUId = request.StandardUId,
                DateTimeCompleted = _timeHelper.Now,
                ProviderRating = request.OverallRating.ToString(),
                ProviderAttributes = request.FeedbackAttributes.
                    Select(s => new Domain.Entities.ProviderAttribute { AttributeId = s.Id, AttributeValue = (int)s.Status }).ToList(),
                AllowContact = request.AllowContact
            };
        }
    }
}