using System.Linq;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeFeedback.Domain.Constants;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackRatingSummary
{
    public class GetApprenticeFeedbackRatingSummaryQueryHandler : IRequestHandler<GetApprenticeFeedbackRatingSummaryQuery, GetApprenticeFeedbackRatingSummaryResult>
    {
        private readonly IProviderStarsSummaryContext _providerStarsSummaryContext;

        public GetApprenticeFeedbackRatingSummaryQueryHandler(IProviderStarsSummaryContext providerStarsSummaryContext)
        {
            _providerStarsSummaryContext = providerStarsSummaryContext;
        }

        public async Task<GetApprenticeFeedbackRatingSummaryResult> Handle(GetApprenticeFeedbackRatingSummaryQuery request, CancellationToken cancellationToken)
        {
            var providerStarsSummaries = await _providerStarsSummaryContext.Entities
                .Where(pss => pss.TimePeriod == ReviewDataPeriod.AggregatedData)
                .ToListAsync();
            return new GetApprenticeFeedbackRatingSummaryResult(providerStarsSummaries);
        }
    }
}
