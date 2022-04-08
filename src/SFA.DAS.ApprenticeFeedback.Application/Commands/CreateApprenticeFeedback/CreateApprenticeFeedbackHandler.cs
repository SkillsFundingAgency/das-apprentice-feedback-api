using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.PostSubmitFeedback
{
    public class CreateApprenticeFeedbackHandler : IRequestHandler<CreateApprenticeFeedbackCommand, CreateApprenticeFeedbackResponse>
    {
        public readonly IApprenticeFeedbackRepository _apprenticeFeedbackRepository;

        public CreateApprenticeFeedbackHandler(IApprenticeFeedbackRepository apprenticeFeedbackRepository)
        {
            _apprenticeFeedbackRepository = apprenticeFeedbackRepository;
        }


        public Task<CreateApprenticeFeedbackResponse> Handle(CreateApprenticeFeedbackCommand request, CancellationToken cancellationToken)
        {

            // Needs to retrieve apprentice feedback target based on apprentice id which should be in request
            var target = _apprenticeFeedbackRepository.GetApprenticeFeedbackTargetById(request.ApprenticeId);

            //if i can't find apprentice target record then need to throw an exception as wont be valid feedback
            if (target == null)
            {
                //throw new Exception("Apprentice Feedback Target not found.");
                //create a new target record since we dont have one and return the id - look in other controller for reference
            }

            //validate the attributes supplied -check that the id exists in attribute table if not throw exception
            var exists = _apprenticeFeedbackRepository.ValidateApprenticeId(request.ApprenticeId);

            if (exists == null)
            {
                throw new Exception("Apprentice Id does not exist.");
            }

            //need to create apprenticefeedbackresult record, which will also contain list of provider attributes

            var feedback = new Domain.Entities.ApprenticeFeedback //ApprenticeFeedbackResult? doesnt have the properties needed
            {
                ApprenticeId = request.ApprenticeId,
                Ukprn = request.Ukprn,
                Rating = request.OverallRating,
                ProviderName = request.ProviderName,
                LarsCode = request.LarsCode,
                StandardUId = request.StandardUId,
                StandardReference = request.StandardReference
            };
            _apprenticeFeedbackRepository.SaveApprenticeFeedback(feedback);
        }
    }
}
