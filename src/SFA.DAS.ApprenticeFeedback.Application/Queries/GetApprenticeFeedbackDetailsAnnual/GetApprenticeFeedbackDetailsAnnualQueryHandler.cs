using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackDetailsAnnual
{
    public class GetApprenticeFeedbackDetailsAnnualQueryHandler : IRequestHandler<GetApprenticeFeedbackDetailsAnnualQuery, GetApprenticeFeedbackDetailsAnnualResult>
    {
        private readonly IProviderAttributeSummaryContext _providerAttributeSummaryContext;
        private readonly IProviderStarsSummaryContext _providerStarsSummaryContext;

        public GetApprenticeFeedbackDetailsAnnualQueryHandler(
            IProviderAttributeSummaryContext providerAttributeSummaryContext,
            IProviderStarsSummaryContext providerStarsSummaryContext)
        {
            _providerAttributeSummaryContext = providerAttributeSummaryContext;
            _providerStarsSummaryContext = providerStarsSummaryContext;
        }

        public async Task<GetApprenticeFeedbackDetailsAnnualResult> Handle(GetApprenticeFeedbackDetailsAnnualQuery request, CancellationToken cancellationToken)
        {
            var annualFeedbackList = new List<GetApprenticeFeedbackDetailAnnual>();

            var result = new GetApprenticeFeedbackDetailsAnnualResult()
            {
                AnnualApprenticeFeedbackDetails = annualFeedbackList
            };

            if (request.Ukprn == 0)
            {
                return result;
            }

            var providerAttributeSummaries = await _providerAttributeSummaryContext.FindProviderAttributeSummaryAnnualAndIncludeAttributes(request.Ukprn);
            var providerStarsSummaries = await _providerStarsSummaryContext.FindProviderStarsSummaryAnnual(request.Ukprn);

            foreach (var providerStarsSummary in providerStarsSummaries)
            {
                var annualApprenticeFeedback = new GetApprenticeFeedbackDetailAnnual
                {
                    Ukprn = request.Ukprn,
                    ReviewCount = providerStarsSummary.ReviewCount,
                    Stars = providerStarsSummary.Stars,
                    ProviderAttribute = providerAttributeSummaries.Where(p => p.TimePeriod == providerStarsSummary.TimePeriod).Select(s => new AttributeResult
                    {
                        Agree = s.Agree,
                        Disagree = s.Disagree,
                        Name = s.Attribute.AttributeName,
                        Category = s.Attribute.Category
                    }),
                    TimePeriod = providerStarsSummary.TimePeriod
                };

                annualFeedbackList.Add(annualApprenticeFeedback);
            }

            return result;
        }
    }
}
