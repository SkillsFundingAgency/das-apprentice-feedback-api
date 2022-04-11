using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.PostSubmitFeedback
{
    public class CreateApprenticeFeedbackHandler : IRequestHandler<CreateApprenticeFeedbackCommand, CreateApprenticeFeedbackResponse>
    {
        public readonly IApprenticeFeedbackRepository _apprenticeFeedbackRepository;

        public CreateApprenticeFeedbackHandler(IApprenticeFeedbackRepository apprenticeFeedbackRepository)
        {
            _apprenticeFeedbackRepository = apprenticeFeedbackRepository;
        }


        public async Task<CreateApprenticeFeedbackResponse> Handle(CreateApprenticeFeedbackCommand request, CancellationToken cancellationToken)
        {

            // Needs to retrieve apprentice feedback target based on apprentice id which should be in request
            // May not return a single record. ( currently we assume it returns one )
            var apprenticeFeedbackTarget = await _apprenticeFeedbackRepository.GetApprenticeFeedbackTargetById(request.ApprenticeId);

            //if i can't find apprentice target record then need to throw an exception as wont be valid feedback
            if (apprenticeFeedbackTarget == null)
            {
                //throw new Exception("Apprentice Feedback Target not found.");
                //create a new target record since we dont have one and return the id - look in other controller for reference
            }

            //validate the attributes supplied -check that the id exists in attribute table if not throw exception
            var validAttributes = await _apprenticeFeedbackRepository.GetAttributes();

            var allValidAttributes = request.FeedbackAttributes.Select(s => s.Id).All(t => validAttributes.Select(t => t.AttributeId).Contains(t));
            if(!allValidAttributes)
            {
                // throw
            }

            //foreach(var a in request.FeedbackAttributes)
            //{
            //    var exists = validAttributes.Where(s => s.AttributeId == a.Id).FirstOrDefault();

            //    if(exists == null)
            //    {
            //        // throw exception it's invalid
            //    }

            //}

            
            // Need to craete Apprentice Feedback Result
            // Need to create ProviderAttribute entries.

            var feedback = new Domain.Entities.ApprenticeFeedbackResult
            {
                ApprenticeFeedbackTargetFeedbackId = apprenticeFeedbackTarget.Id,
                Ukprn = request.Ukprn,
                LarsCode = request.LarsCode,
                StandardUId = request.StandardUId,
                StandardReference = request.StandardReference,
                DateTimeCompleted = DateTime.UtcNow,
                ProviderRating = request.OverallRating.ToString(),
                //ProviderName = request.ProviderName,
                ProviderAttributes = request.FeedbackAttributes.
                    Select(s => new Domain.Entities.ProviderAttribute { AttributeId = s.Id, AttributeValue = (int)s.Status }).ToList()
            };
            
            var updatedFeedback = await _apprenticeFeedbackRepository.CreateApprenticeFeedbackResult(feedback);

            return new CreateApprenticeFeedbackResponse();
        }
    }
}