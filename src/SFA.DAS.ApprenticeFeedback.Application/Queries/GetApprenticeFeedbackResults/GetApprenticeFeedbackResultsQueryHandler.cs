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
        private readonly IApprenticeFeedbackResultContext _apprenticeFeedbackResultContext;
        private readonly IProviderAttributeContext _providerAttributeContext;
        private readonly IAttributeContext _attributeContext;
        private readonly ApplicationSettings _settings;

        public GetApprenticeFeedbackResultsQueryHandler(
            IApprenticeFeedbackTargetContext apprenticeFeedbackTargetContext,
            IApprenticeFeedbackResultContext apprenticeFeedbackResultContext,
            IProviderAttributeContext providerAttributeContext,
            IAttributeContext attributeContext,
            IDateTimeHelper dateTimeHelper,
            ApplicationSettings settings
            )
        {
            _apprenticeFeedbackResultContext = apprenticeFeedbackResultContext;
            _providerAttributeContext = providerAttributeContext;
            _attributeContext = attributeContext;
            _settings = settings;
        }

        public async Task<GetApprenticeFeedbackResultsResult> Handle(GetApprenticeFeedbackResultsQuery request, CancellationToken cancellationToken)
        {
            var result = new GetApprenticeFeedbackResultsResult();

            var sprocResult = await _apprenticeFeedbackResultContext.GetFeedbackForProvidersAsync(request.Ukprns, _settings.ReportingMinNumberOfResponses, _settings.ReportingFeedbackCutoffMonths);

            if (sprocResult.Any())  
            {
                var providers = sprocResult.GroupBy(grp => grp.Ukprn);
                
                foreach (var providerGrouping in providers)
                {
                    var ratings = providerGrouping.GroupBy(grp => grp.ProviderRating, (k,v) => v.FirstOrDefault())
                        .Select(r => new RatingResult { Rating = r.ProviderRating, Count = r.ProviderRatingCount })
                        .ToList();

                    var attributes = providerGrouping.GroupBy(grp => grp.AttributeName, (k, v) => v.FirstOrDefault())
                        .Select(a => new AttributeResult
                        {
                            Name = a.AttributeName,
                            Category = a.Category,
                            Agree = a.ProviderAttributeAgree,
                            Disagree = a.ProviderAttributeDisagree
                        }).ToList();

                    var feedbackResult = new UkprnFeedback
                    {
                        Ukprn = providerGrouping.Key,
                        ProviderRating = ratings,
                        ProviderAttribute = attributes
                    };

                    result.UkprnFeedbacks.Add(feedbackResult);
                }
            }

            return result;
        }
    }
}
