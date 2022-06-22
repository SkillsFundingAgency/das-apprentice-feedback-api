using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackResult
{
    public class GetApprenticeFeedbackResultQueryHandler : IRequestHandler<GetApprenticeFeedbackResultQuery, GetApprenticeFeedbackResultResult>
    {
        private readonly IApprenticeFeedbackTargetContext _apprenticeFeedbackTargetContext;
        private readonly IApprenticeFeedbackResultContext _apprenticeFeedbackResultContext;
        private readonly IProviderAttributeContext _providerAttributeContext;
        private readonly IDateTimeHelper _dateTimeHelper;

        public GetApprenticeFeedbackResultQueryHandler(
            IApprenticeFeedbackTargetContext apprenticeFeedbackTargetContext,
            IApprenticeFeedbackResultContext apprenticeFeedbackResultContext,
            IProviderAttributeContext providerAttributeContext,
            IDateTimeHelper dateTimeHelper
            )
        {
            _apprenticeFeedbackTargetContext = apprenticeFeedbackTargetContext;
            _apprenticeFeedbackResultContext = apprenticeFeedbackResultContext;
            _providerAttributeContext = providerAttributeContext;
            _dateTimeHelper = dateTimeHelper;
        }

        public Task<GetApprenticeFeedbackResultResult> Handle(GetApprenticeFeedbackResultQuery request, CancellationToken cancellationToken)
        {
            var result = new GetApprenticeFeedbackResultResult();

            var recentFeedbackResultsForUkprn = _apprenticeFeedbackTargetContext.Entities
                                    .Include(r => r.ApprenticeFeedbackResults)
                                    .Where(t => t.Ukprn == request.Ukprn
                                                && t.ApprenticeFeedbackResults.All(r => r.DateTimeCompleted >= _dateTimeHelper.Now.Date.AddMonths(-12))
                                                )
                                    // @ToDo: must have more than 10 feedback responses from different learners
                                    .Select(f => new { ApprenticeFeedbackResultId = f.ApprenticeFeedbackResults.OrderByDescending(r => r.DateTimeCompleted).First().Id, Rating = f.ApprenticeFeedbackResults.OrderByDescending(r => r.DateTimeCompleted).First().ProviderRating })
                                    .ToList();

            var apprenticeFeedbackResultIds = recentFeedbackResultsForUkprn.Select(r => r.ApprenticeFeedbackResultId).ToList();

            var ratings = recentFeedbackResultsForUkprn
                        .GroupBy(info => info.Rating)
                        .Select(group => new
                        {
                            Rating = group.Key,
                            Count = group.Count()
                        }).ToList();

            var attributeResults = _providerAttributeContext.Entities
                                    .Include(a => a.Attribute)
                                    .Where(pa => apprenticeFeedbackResultIds.Contains(pa.ApprenticeFeedbackResultId) )
                                    .ToList();


            /*



                        var foo = attributeResults.GroupBy(a => a.AttributeId)
                                                    .Select(grp => new 
                                                    {
                                                        AttributeId = grp.Key,
                                                        Value = grp.Count()
                                                    });

                        */


            if (recentFeedbackResultsForUkprn.Any())
            {
                result.Ukprn = request.Ukprn;

                foreach (var rating in ratings)
                {
                    if(!string.IsNullOrEmpty(rating.Rating))
                    {
                        result.ProviderRating.Add(rating.Rating, rating.Count);
                    }
                }

                // temp
                foreach(var ar in attributeResults)
                {
                    result.ProviderAttribute.Add(new Domain.Entities.AttributeResult() 
                    { 
                        Name = ar.Attribute.AttributeName,
                        Category = "",  // ??
                        Agree = 0,
                        Disagree = 0
                    });
                }

            }


            return Task.FromResult(result);
        }
    }
}
