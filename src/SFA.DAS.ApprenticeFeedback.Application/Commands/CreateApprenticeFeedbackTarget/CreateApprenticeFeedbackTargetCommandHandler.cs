using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
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
            var result = await _apprenticeFeedbackRepository.CreateApprenticeFeedbackTarget(new Domain.Models.ApprenticeFeedbackTarget
            {
                ApprenticeId = request.ApprenticeId,
                ApprenticeshipId = request.CommitmentApprenticeshipId,
                Status = Domain.Models.ApprenticeFeedbackTarget.FeedbackTargetStatus.NotYetActive,
            });

            return new CreateApprenticeFeedbackTargetCommandResponse
            {
                FeedbackId = result
            };
        }
    }
}
