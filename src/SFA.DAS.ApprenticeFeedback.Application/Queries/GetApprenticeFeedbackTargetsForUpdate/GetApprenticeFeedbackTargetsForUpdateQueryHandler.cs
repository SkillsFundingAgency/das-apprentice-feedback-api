using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Domain.Models.Enums;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackTargetsForUpdate
{
    public static class ApprenticeFeedbackTargetFilters
    {
        public static IQueryable<Domain.Entities.ApprenticeFeedbackTarget> HasStartedOrUnknown(this IQueryable<Domain.Entities.ApprenticeFeedbackTarget> afts, IDateTimeHelper dateTimeHelper)
        {
            return afts.Where(aft => !aft.StartDate.HasValue || (aft.StartDate.HasValue && aft.StartDate.Value.Date < dateTimeHelper.Now.Date));
        }

        public static IQueryable<Domain.Entities.ApprenticeFeedbackTarget> StatusNotCompleted(this IQueryable<Domain.Entities.ApprenticeFeedbackTarget> afts)
        {
            return afts.Where(aft => aft.Status != (int)FeedbackTargetStatus.Complete);
        }
        public static IQueryable<Domain.Entities.ApprenticeFeedbackTarget> FeedbackEligibilityNotCalculatedRecently(this IQueryable<Domain.Entities.ApprenticeFeedbackTarget> afts, IDateTimeHelper dateTimeHelper, ApplicationSettings appSettings)
        {
            return afts.Where(aft => !(aft.EligibilityCalculationDate.HasValue && aft.EligibilityCalculationDate.Value.Date.AddDays(appSettings.EligibilityCalculationThrottleDays) > dateTimeHelper.Now.Date));
        }
        public static IQueryable<Domain.Entities.ApprenticeFeedbackTarget> NotGivenFeedbackRecently(this IQueryable<Domain.Entities.ApprenticeFeedbackTarget> afts, IDateTimeHelper dateTimeHelper, ApplicationSettings appSettings)
        {
            return afts.Where(aft => aft.ApprenticeFeedbackResults.OrderByDescending(a => a.DateTimeCompleted).FirstOrDefault() == null ?
                true
                : !(aft.ApprenticeFeedbackResults.OrderByDescending(a => a.DateTimeCompleted).FirstOrDefault().DateTimeCompleted.Value.Date.AddDays(appSettings.RecentDenyPeriodDays) > dateTimeHelper.Now.Date));
        }
    }


    public class GetApprenticeFeedbackTargetsForUpdateQueryHandler : IRequestHandler<GetApprenticeFeedbackTargetsForUpdateQuery, GetApprenticeFeedbackTargetsForUpdateResult>
    {
        private readonly IApprenticeFeedbackTargetContext _apprenticeFeedbackTargetDataContext;
        private readonly ApplicationSettings _appSettings;
        private readonly IDateTimeHelper _dateTimeHelper;

        public GetApprenticeFeedbackTargetsForUpdateQueryHandler(
            IApprenticeFeedbackTargetContext apprenticeFeedbackTargetDataContext
            , ApplicationSettings appSettings
            , IDateTimeHelper dateTimeHelper
            )
        {
            _apprenticeFeedbackTargetDataContext = apprenticeFeedbackTargetDataContext;
            _appSettings = appSettings;
            _dateTimeHelper = dateTimeHelper;
        }

        public async Task<GetApprenticeFeedbackTargetsForUpdateResult> Handle(GetApprenticeFeedbackTargetsForUpdateQuery request, CancellationToken cancellationToken)
        {
            var apprenticeFeedbackTargets = _apprenticeFeedbackTargetDataContext
                .Entities
                    .HasStartedOrUnknown(_dateTimeHelper)
                    .StatusNotCompleted()
                    .FeedbackEligibilityNotCalculatedRecently(_dateTimeHelper, _appSettings)
                    .NotGivenFeedbackRecently(_dateTimeHelper, _appSettings)
                    .OrderByDescending(aft => aft.CreatedOn)
                    .Take(request.BatchSize)
                    .Select(aft => new Domain.Models.ApprenticeFeedbackTargetForUpdate
                    {
                        ApprenticeFeedbackTargetId = aft.Id,
                        ApprenticeId = aft.ApprenticeId,
                        ApprenticeshipId = aft.ApprenticeshipId
                    });

            return new GetApprenticeFeedbackTargetsForUpdateResult
            {
                ApprenticeFeedbackTargets = await apprenticeFeedbackTargets.ToListAsync()
            };
        }
    }
}
