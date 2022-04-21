using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

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

            var validAttributesNames = validAttributes.Select(attribute => attribute.AttributeName).ToList();

            var providedAttributesNames = request.FeedbackAttributes.Select(attribute => attribute.Name).ToList();

            string attributesProvided = string.Empty;

            foreach (var attribute in request.FeedbackAttributes)
            {
                if (string.IsNullOrEmpty(attributesProvided))
                {
                    attributesProvided = attribute.Name;
                }
                else
                {
                    attributesProvided = $"{attributesProvided}, {attribute.Name}";
                }
            }

            var invalidAttributeNames = providedAttributesNames.Except(validAttributesNames).ToList();

            string invalid = string.Empty;
            foreach (var attributeName in invalidAttributeNames)
            {
                if (string.IsNullOrEmpty(invalid))
                {
                    invalid = attributeName;
                }
                else
                {
                    invalid = $"{invalid}, {attributeName}";
                }
            }

            var allValidAttributes = request.FeedbackAttributes.Select(s => s.Id).All(t => validAttributes.Select(t => t.AttributeId).Contains(t));
            if (!allValidAttributes)
            {
                _logger.LogError($"Some or all of the attributes supplied to create the feedback record do not exist in the database. Attributes provided in the request: {attributesProvided}, the following attributes are invalid: {invalid}");
                throw new InvalidOperationException($"Some or all of the attributes supplied to create the feedback record do not exist in the database. Attributes provided in the request: {attributesProvided}, the following attributes are invalid: {invalid}");
            }

            var feedback = new Domain.Entities.ApprenticeFeedbackResult
            {
                ApprenticeFeedbackTargetId = apprenticeFeedbackTarget.Id,
                StandardUId = request.StandardUId,
                DateTimeCompleted = _timeHelper.Now,
                ProviderRating = request.OverallRating.ToString(),
                ProviderAttributes = request.FeedbackAttributes.
                    Select(s => new Domain.Entities.ProviderAttribute { AttributeId = s.Id, AttributeValue = (int)s.Status }).ToList(),
                AllowContact = request.AllowContact
            };

            feedback = await _apprenticeFeedbackRepository.CreateApprenticeFeedbackResult(feedback);

            _logger.LogDebug($"Successfully created feedback object with Id: {feedback.Id}");
            return new CreateApprenticeFeedbackResponse() { ApprenticeFeedbackResultId = feedback.Id };
        }
    }
}