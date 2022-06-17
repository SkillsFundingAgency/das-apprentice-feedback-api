using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedbackTarget
{
    public class CreateApprenticeFeedbackTargetCommandHandler : IRequestHandler<CreateApprenticeFeedbackTargetCommand, CreateApprenticeFeedbackTargetCommandResponse>
    {
        private readonly IApprenticeFeedbackTargetContext _apprenticeFeedbackTargetContext;

        public CreateApprenticeFeedbackTargetCommandHandler(IApprenticeFeedbackTargetContext apprenticeFeedbackTargetDataContext)
        {
            _apprenticeFeedbackTargetContext = apprenticeFeedbackTargetDataContext;
        }

        public async Task<CreateApprenticeFeedbackTargetCommandResponse> Handle(CreateApprenticeFeedbackTargetCommand request, CancellationToken cancellationToken)
        {
            var apprenticeFeedbackTarget = await _apprenticeFeedbackTargetContext.FindByApprenticeIdAndApprenticeshipIdAndIncludeFeedbackResultsAsync(request.ApprenticeId, request.CommitmentApprenticeshipId);
            Guid? apprenticeFeedbackTargetId;
            if (apprenticeFeedbackTarget == null)
            {
                var entity = _apprenticeFeedbackTargetContext.Add(new Domain.Models.ApprenticeFeedbackTarget
                {
                    ApprenticeId = request.ApprenticeId,
                    ApprenticeshipId = request.CommitmentApprenticeshipId,
                    // This will be set by the daily job once it's processed for the first time.
                    Status = FeedbackTargetStatus.Unknown,
                });
                await _apprenticeFeedbackTargetContext.SaveChangesAsync();
                apprenticeFeedbackTargetId = entity.Entity.Id;
            }
            else
            {
                Domain.Models.ApprenticeFeedbackTarget updatedTarget = apprenticeFeedbackTarget;
                updatedTarget.ResetFeedbackTarget();
                apprenticeFeedbackTarget = await UpdateApprenticeFeedbackTarget(updatedTarget);
                apprenticeFeedbackTargetId = apprenticeFeedbackTarget.Id;
            }

            return new CreateApprenticeFeedbackTargetCommandResponse
            {
                ApprenticeFeedbackTargetId = apprenticeFeedbackTargetId.GetValueOrDefault(Guid.Empty)
            };
        }

        private async Task<Domain.Entities.ApprenticeFeedbackTarget> UpdateApprenticeFeedbackTarget(Domain.Entities.ApprenticeFeedbackTarget apprenticeFeedbackTarget)
        {
            var feedbackTarget = await _apprenticeFeedbackTargetContext.Entities.FirstOrDefaultAsync(s => s.Id == apprenticeFeedbackTarget.Id);
            if (feedbackTarget == null)
            {
                return null;
            }

            feedbackTarget.StartDate = apprenticeFeedbackTarget.StartDate;
            feedbackTarget.EndDate = apprenticeFeedbackTarget.EndDate;
            feedbackTarget.Ukprn = apprenticeFeedbackTarget.Ukprn;
            feedbackTarget.ProviderName = apprenticeFeedbackTarget.ProviderName;
            feedbackTarget.StandardName = apprenticeFeedbackTarget.StandardName;
            feedbackTarget.StandardUId = apprenticeFeedbackTarget.StandardUId;
            feedbackTarget.LarsCode = apprenticeFeedbackTarget.LarsCode;

            if (feedbackTarget.FeedbackEligibility != apprenticeFeedbackTarget.FeedbackEligibility ||
                feedbackTarget.Status != apprenticeFeedbackTarget.Status)
            {
                feedbackTarget.Status = apprenticeFeedbackTarget.Status;
                feedbackTarget.FeedbackEligibility = apprenticeFeedbackTarget.FeedbackEligibility;
                feedbackTarget.EligibilityCalculationDate = apprenticeFeedbackTarget.EligibilityCalculationDate;
            }

            await _apprenticeFeedbackTargetContext.SaveChangesAsync();

            return feedbackTarget;
        }
    }
}
