using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackTargetsForUpdate
{
    public class GetApprenticeFeedbackTargetsForUpdateQueryHandler : IRequestHandler<GetApprenticeFeedbackTargetsForUpdateQuery, GetApprenticeFeedbackTargetsForUpdateResult>
    {
        private readonly IApprenticeFeedbackDataContext _dbContext;
        public readonly IDateTimeHelper _dateTimeHelper;

        public GetApprenticeFeedbackTargetsForUpdateQueryHandler(IApprenticeFeedbackDataContext dbContext, IDateTimeHelper dateTimeHelper)
        {
            _dbContext = dbContext;
            _dateTimeHelper = dateTimeHelper;
        }

        public async Task<GetApprenticeFeedbackTargetsForUpdateResult> Handle(GetApprenticeFeedbackTargetsForUpdateQuery request, CancellationToken cancellationToken)
        {
            var apprenticeFeedbackTargets = _dbContext.ApprenticeFeedbackTargets
                .Where(aft => 
                    aft.StartDate.Value.Date <= _dateTimeHelper.Now.Date
                    && aft.Status == (int)FeedbackTargetStatus.Active
                    && aft.FeedbackEligibility == (int)FeedbackEligibilityStatus.Allow
                    // FeedbackEligibilityStatus throttle
                    )
                .OrderByDescending(aft => aft.CreatedOn)
                ;

            return new GetApprenticeFeedbackTargetsForUpdateResult
            {
                ApprenticeFeedbackTargets = apprenticeFeedbackTargets.Select(s => (Domain.Models.ApprenticeFeedbackTarget)s).ToList()
            };
        }
    }
}
