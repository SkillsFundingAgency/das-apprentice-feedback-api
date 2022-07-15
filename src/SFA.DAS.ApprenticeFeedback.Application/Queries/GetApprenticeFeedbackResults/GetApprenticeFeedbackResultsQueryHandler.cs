using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackResult.GetApprenticeFeedbackResultsResult;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackResult
{
    public class GetApprenticeFeedbackResultsQueryHandler : IRequestHandler<GetApprenticeFeedbackResultsQuery, GetApprenticeFeedbackResultsResult>
    {
        private readonly IProviderRatingSummaryContext _providerRatingSummaryContext;
        private readonly IProviderAttributeSummaryContext _providerAttributeSummaryContext;
        private readonly IProviderStarsSummaryContext _providerStarsSummaryContext;

        public GetApprenticeFeedbackResultsQueryHandler(
            IProviderRatingSummaryContext providerRatingSummaryContext,
            IProviderAttributeSummaryContext providerAttributeSummaryContext,
            IProviderStarsSummaryContext providerStarsSummaryContext)
        {
            _providerRatingSummaryContext = providerRatingSummaryContext;
            _providerAttributeSummaryContext = providerAttributeSummaryContext;
            _providerStarsSummaryContext = providerStarsSummaryContext;
        }

        public async Task<GetApprenticeFeedbackResultsResult> Handle(GetApprenticeFeedbackResultsQuery request, CancellationToken cancellationToken)
        {
            var result = new GetApprenticeFeedbackResultsResult();

            if (request.Ukprns == null || !request.Ukprns.Any())
            {
                return result;
            }

            var providerRatingSummaries = _providerRatingSummaryContext.Entities.Where(s => request.Ukprns.Contains(s.Ukprn));
            var providerAttributeSummaries = await _providerAttributeSummaryContext.FindProviderAttributeSummaryAndIncludeAttributes(request.Ukprns);
            var providerStarsSummaries = _providerStarsSummaryContext.Entities.Where(u => request.Ukprns.Contains(u.Ukprn));

            foreach (var ukprn in request.Ukprns)
            {
                result.UkprnFeedbacks.Add(
                    new UkprnFeedback
                    {
                        Ukprn = ukprn,
                        ProviderRating = providerRatingSummaries.Where(x => x.Ukprn == ukprn).Select(a =>
                            new RatingResult
                            {
                                Count = a.RatingCount,
                                Rating = a.Rating
                            }),
                        ProviderAttribute = providerAttributeSummaries.Where(x => x.Ukprn == ukprn).Select(b =>
                            new AttributeResult
                            {
                                Name = b.Attribute.AttributeName,
                                Category = b.Attribute.Category,
                                Agree = b.Agree,
                                Disagree = b.Disagree
                            })
                    });
            }

            return result;
        }
    }
}
