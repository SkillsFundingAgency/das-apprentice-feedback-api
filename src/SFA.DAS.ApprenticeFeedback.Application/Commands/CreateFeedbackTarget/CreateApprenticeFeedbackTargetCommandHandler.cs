using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.CreateFeedbackTarget
{
    public class CreateApprenticeFeedbackTargetCommandHandler : IRequestHandler<CreateApprenticeFeedbackTargetCommand, CreateApprenticeFeedbackTargetCommandResponse>
    {
        private readonly IApprenticeFeedbackService _apprenticeFeedbackService;

        public CreateApprenticeFeedbackTargetCommandHandler(IApprenticeFeedbackService apprenticeFeedbackService)
        {
            _apprenticeFeedbackService = apprenticeFeedbackService;
        }

        public async Task<CreateApprenticeFeedbackTargetCommandResponse> Handle(CreateApprenticeFeedbackTargetCommand request, CancellationToken cancellationToken)
        {
            var result = await _apprenticeFeedbackService.CreateApprenticeFeedbackTarget(request.FeedbackTarget);

            return new CreateApprenticeFeedbackTargetCommandResponse
            {
                FeedbackId = result
            };
        }
    }
}
