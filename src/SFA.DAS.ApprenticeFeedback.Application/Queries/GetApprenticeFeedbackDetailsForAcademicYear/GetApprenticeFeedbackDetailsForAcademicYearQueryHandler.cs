using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackDetailsForAcademicYear
{
    public class GetApprenticeFeedbackDetailsForAcademicYearQueryHandler : IRequestHandler<GetApprenticeFeedbackDetailsForAcademicYearQuery, GetApprenticeFeedbackDetailsForAcademicYearResult>
    {
        private readonly IProviderAttributeSummaryContext _providerAttributeSummaryContext;
        private readonly IProviderStarsSummaryContext _providerStarsSummaryContext;

        public GetApprenticeFeedbackDetailsForAcademicYearQueryHandler(
            IProviderAttributeSummaryContext providerAttributeSummaryContext,
            IProviderStarsSummaryContext providerStarsSummaryContext)
        {
            _providerAttributeSummaryContext = providerAttributeSummaryContext;
            _providerStarsSummaryContext = providerStarsSummaryContext;
        }

        public async Task<GetApprenticeFeedbackDetailsForAcademicYearResult> Handle(GetApprenticeFeedbackDetailsForAcademicYearQuery request, CancellationToken cancellationToken)
        {
            if (request.Ukprn == 0)
            {
                return new GetApprenticeFeedbackDetailsForAcademicYearResult
                {
                    ProviderAttribute = new List<AttributeResult>()
                };
            }

            var providerAttributeSummaries = await _providerAttributeSummaryContext.FindProviderAttributeSummaryPerAcademicYearAndIncludeAttributes(request.Ukprn, request.AcademicYear);
            var providerStarsSummary = await _providerStarsSummaryContext.FindProviderStarsSummaryForAcademicYear(request.Ukprn, request.AcademicYear);

            if (providerAttributeSummaries == null || providerStarsSummary == null)
            {
                return new GetApprenticeFeedbackDetailsForAcademicYearResult
                {
                    Ukprn = request.Ukprn,
                    ProviderAttribute = new List<AttributeResult>(),
                    TimePeriod = request.AcademicYear
                };
            }

            return new GetApprenticeFeedbackDetailsForAcademicYearResult
            {
                Ukprn = request.Ukprn,
                ReviewCount = providerStarsSummary.ReviewCount,
                Stars = providerStarsSummary.Stars,
                ProviderAttribute = providerAttributeSummaries
                .Where(p => p.TimePeriod == providerStarsSummary.TimePeriod)
                .GroupBy(x => new {x.Attribute.Category, x.Attribute.AttributeName})
                .Select(s => new AttributeResult
                {
                    Agree = s.Sum(y => y.Agree),
                    Disagree = s.Sum(y => y.Disagree),
                    Name = s.Key.AttributeName,
                    Category = s.Key.Category
                }),
                TimePeriod = providerStarsSummary.TimePeriod
            };
        }
    }
}
