using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedbackTarget
{
    public class CreateApprenticeFeedbackTargetCommandHandler : IRequestHandler<CreateApprenticeFeedbackTargetCommand, CreateApprenticeFeedbackTargetCommandResponse>
    {
        private readonly IApprenticeFeedbackRepository _apprenticeFeedbackRepository;

        public CreateApprenticeFeedbackTargetCommandHandler(IApprenticeFeedbackRepository apprenticeFeedbackRepository)
        {
            _apprenticeFeedbackRepository = apprenticeFeedbackRepository;
        }

        public async Task<CreateApprenticeFeedbackTargetCommandResponse> Handle(CreateApprenticeFeedbackTargetCommand request, CancellationToken cancellationToken)
        {
            var apprenticeFeedbackTarget = await _apprenticeFeedbackRepository.GetApprenticeFeedbackTarget(request.ApprenticeId, request.CommitmentApprenticeshipId);
            Guid? feedbackId;
            if (apprenticeFeedbackTarget == null)
            {
                feedbackId = await _apprenticeFeedbackRepository.CreateApprenticeFeedbackTarget(new Domain.Models.ApprenticeFeedbackTarget
                {
                    ApprenticeId = request.ApprenticeId,
                    ApprenticeshipId = request.CommitmentApprenticeshipId,
                    Status = Domain.Models.ApprenticeFeedbackTarget.FeedbackTargetStatus.NotYetActive,
                });
            }
            else
            {
                Domain.Models.ApprenticeFeedbackTarget updatedTarget = apprenticeFeedbackTarget;
                updatedTarget.ResetFeedbackTarget();
                apprenticeFeedbackTarget = await _apprenticeFeedbackRepository.UpdateApprenticeFeedbackTarget(updatedTarget);
                feedbackId = apprenticeFeedbackTarget.Id;
            }

            return new CreateApprenticeFeedbackTargetCommandResponse
            {
                FeedbackId = feedbackId
            };
        }
    }
}
