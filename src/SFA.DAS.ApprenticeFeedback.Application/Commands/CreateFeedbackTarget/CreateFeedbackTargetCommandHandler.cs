using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.CreateFeedbackTarget
{
    public class CreateFeedbackTargetCommandHandler : IRequestHandler<CreateFeedbackTargetCommand, CreateFeedbackTargetCommandResponse>
    {
        private readonly IApprenticeFeedbackService _apprenticeFeedbackService;

        public CreateFeedbackTargetCommandHandler(IApprenticeFeedbackService apprenticeFeedbackService)
        {
            _apprenticeFeedbackService = apprenticeFeedbackService;
        }

        public async Task<CreateFeedbackTargetCommandResponse> Handle(CreateFeedbackTargetCommand request, CancellationToken cancellationToken)
        {
            var result = await _apprenticeFeedbackService.CreateFeedbackTarget(request.FeedbackTarget);

            return new CreateFeedbackTargetCommandResponse
            {
                IsCreated = result
            };
        }
    }
}
