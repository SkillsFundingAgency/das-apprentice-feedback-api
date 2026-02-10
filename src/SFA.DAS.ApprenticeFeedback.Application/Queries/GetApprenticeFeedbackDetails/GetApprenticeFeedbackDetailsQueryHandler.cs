using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackDetails
{
    public class GetApprenticeFeedbackDetailsQueryHandler : IRequestHandler<GetApprenticeFeedbackDetailsQuery, GetApprenticeFeedbackDetailsResult>
    {
        private readonly IProviderAttributeSummaryContext _providerAttributeSummaryContext;
        private readonly IProviderStarsSummaryContext _providerStarsSummaryContext;

        public GetApprenticeFeedbackDetailsQueryHandler(
            IProviderAttributeSummaryContext providerAttributeSummaryContext,
            IProviderStarsSummaryContext providerStarsSummaryContext)
        {
            _providerAttributeSummaryContext = providerAttributeSummaryContext;
            _providerStarsSummaryContext = providerStarsSummaryContext;
        }

        public async Task<GetApprenticeFeedbackDetailsResult> Handle(GetApprenticeFeedbackDetailsQuery request, CancellationToken cancellationToken)
        {
            if (request.Ukprn == 0)
            {
                return new GetApprenticeFeedbackDetailsResult
                {
                    ProviderAttribute = new List<AttributeResult>()
                };
            }

            var providerAttributeSummaries = await _providerAttributeSummaryContext.FindProviderAttributeSummaryAndIncludeAttributes(request.Ukprn);
            var providerStarsSummaries = _providerStarsSummaryContext.Entities.FirstOrDefault(u => u.Ukprn == request.Ukprn);

            return new GetApprenticeFeedbackDetailsResult
            {
                Ukprn = request.Ukprn,
                ReviewCount = providerStarsSummaries?.ReviewCount ?? 0,
                Stars = providerStarsSummaries?.Stars ?? 0,
                ProviderAttribute = providerAttributeSummaries
                .GroupBy(x => new {x.Attribute.Category,x.Attribute.AttributeName})                
                .Select(s => new AttributeResult
                {
                    Agree = s.Sum(y => y.Agree),
                    Disagree = s.Sum(y => y.Disagree),
                    Name = s.Key.AttributeName,
                    Category = s.Key.Category
                })
            };
        }
    }
}
