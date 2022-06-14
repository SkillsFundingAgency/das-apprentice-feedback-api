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
        private readonly IApprenticeFeedbackDataContext _dbContext;

        public CreateApprenticeFeedbackTargetCommandHandler(IApprenticeFeedbackDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CreateApprenticeFeedbackTargetCommandResponse> Handle(CreateApprenticeFeedbackTargetCommand request, CancellationToken cancellationToken)
        {
            var apprenticeFeedbackTarget = await _dbContext.ApprenticeFeedbackTargets
                .Include(s => s.ApprenticeFeedbackResults)
                .FirstOrDefaultAsync(aft => aft.ApprenticeId == request.ApprenticeId && aft.ApprenticeshipId == request.CommitmentApprenticeshipId);

            Guid? apprenticeFeedbackTargetId;
            if (apprenticeFeedbackTarget == null)
            {
                var entity = await _dbContext.ApprenticeFeedbackTargets.AddAsync(new Domain.Models.ApprenticeFeedbackTarget
                {
                    ApprenticeId = request.ApprenticeId,
                    ApprenticeshipId = request.CommitmentApprenticeshipId,
                    // This will be set by the daily job once it's processed for the first time.
                    Status = FeedbackTargetStatus.Unknown,
                });
                await _dbContext.SaveChangesAsync();

                apprenticeFeedbackTargetId = entity.Entity.Id;
            }
            else
            {
                Domain.Models.ApprenticeFeedbackTarget updatedTarget = apprenticeFeedbackTarget;
                updatedTarget.ResetFeedbackTarget();

                var feedbackTarget = await _dbContext.ApprenticeFeedbackTargets.FirstOrDefaultAsync(s => s.Id == updatedTarget.Id);
                if (feedbackTarget == null)
                {
                    return null;
                }

                feedbackTarget.StartDate = updatedTarget.StartDate;
                feedbackTarget.EndDate = updatedTarget.EndDate;
                feedbackTarget.Ukprn = updatedTarget.Ukprn;
                feedbackTarget.ProviderName = updatedTarget.ProviderName;
                feedbackTarget.StandardName = updatedTarget.StandardName;
                feedbackTarget.StandardUId = updatedTarget.StandardUId;
                feedbackTarget.LarsCode = updatedTarget.LarsCode;

                if (feedbackTarget.FeedbackEligibility != (int)updatedTarget.FeedbackEligibility ||
                    feedbackTarget.Status != (int)updatedTarget.Status)
                {
                    feedbackTarget.Status = (int)updatedTarget.Status;
                    feedbackTarget.FeedbackEligibility = (int)updatedTarget.FeedbackEligibility;
                    feedbackTarget.EligibilityCalculationDate = updatedTarget.EligibilityCalculationDate;
                }

                await _dbContext.SaveChangesAsync();

                apprenticeFeedbackTargetId = apprenticeFeedbackTarget.Id;
            }

            return new CreateApprenticeFeedbackTargetCommandResponse
            {
                ApprenticeFeedbackTargetId = apprenticeFeedbackTargetId.GetValueOrDefault(Guid.Empty)
            };
        }
    }
}
