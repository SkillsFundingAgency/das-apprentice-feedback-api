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
        public readonly ILogger _logger;
        public readonly IDateTimeHelper _time;

        public CreateApprenticeFeedbackHandler(IApprenticeFeedbackRepository apprenticeFeedbackRepository, ILogger logger, IDateTimeHelper time)
        {
            _apprenticeFeedbackRepository = apprenticeFeedbackRepository;
            _time = time;
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

            var allValidAttributes = request.FeedbackAttributes.Select(s => s.Id).All(t => validAttributes.Select(t => t.AttributeId).Contains(t));
            if(!allValidAttributes)
            {
                _logger.LogError($"Some or all of the attributes supplied to create the feedback record do not exist in the database. Attributes provided in the request: {request.FeedbackAttributes.ToList()}");
                throw new InvalidOperationException($"Some or all of the attributes supplied to create the feedback record do not exist in the database. Attributes provided in the request: {request.FeedbackAttributes.ToList()}");
            }

            var feedback = new Domain.Entities.ApprenticeFeedbackResult
            {
                ApprenticeFeedbackTargetId = apprenticeFeedbackTarget.Id,
                StandardUId = request.StandardUId,
                DateTimeCompleted = _time.Now,
                ProviderRating = request.OverallRating.ToString(),
                ProviderAttributes = request.FeedbackAttributes.
                    Select(s => new Domain.Entities.ProviderAttribute { AttributeId = s.Id, AttributeValue = (int)s.Status }).ToList(),
                AllowContact = request.AllowContact
            };

            var updatedFeedback = await _apprenticeFeedbackRepository.CreateApprenticeFeedbackResult(feedback);

            _logger.LogInformation($"Successfully created feedback object with Id: {feedback.Id}");

            return new CreateApprenticeFeedbackResponse() { ApprenticeFeedbackResultId = updatedFeedback.Id };
        }
    }
}