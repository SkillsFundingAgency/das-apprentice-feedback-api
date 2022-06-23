﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Entities;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackResult.GetApprenticeFeedbackResultsResult;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackResult
{
    public class GetApprenticeFeedbackResultsQueryHandler : IRequestHandler<GetApprenticeFeedbackResultsQuery, GetApprenticeFeedbackResultsResult>
    {
        private readonly IApprenticeFeedbackTargetContext _apprenticeFeedbackTargetContext;
        private readonly IApprenticeFeedbackResultContext _apprenticeFeedbackResultContext;
        private readonly IProviderAttributeContext _providerAttributeContext;
        private readonly IAttributeContext _attributeContext;
        private readonly IDateTimeHelper _dateTimeHelper;
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
            _apprenticeFeedbackTargetContext = apprenticeFeedbackTargetContext;
            _apprenticeFeedbackResultContext = apprenticeFeedbackResultContext;
            _providerAttributeContext = providerAttributeContext;
            _attributeContext = attributeContext;
            _dateTimeHelper = dateTimeHelper;
            _settings = settings;
        }

        public async Task<GetApprenticeFeedbackResultsResult> Handle(GetApprenticeFeedbackResultsQuery request, CancellationToken cancellationToken)
        {
            var result = new GetApprenticeFeedbackResultsResult();

            var sprocResult = await _apprenticeFeedbackResultContext.GetFeedbackForProvidersAsync(request.Ukprns, _settings.ReportingMinNumberOfResponses, _settings.ReportingFeedbackCutoffMonths);

            if (sprocResult.Any())
            {
                var ukprns = sprocResult.Select(u => u.Ukprn).Distinct().ToArray();
                var attributes = _attributeContext.Entities.ToList();

                foreach (var ukprn in ukprns)
                {
                    var ratings = sprocResult.Where(r => r.Ukprn == ukprn).GroupBy(grp => grp.ProviderRating).Select(grp => new { Rating = grp.Key, Count = grp.Count() });

                    var feedbackResult = new UkprnFeedback();
                    feedbackResult.Ukprn = ukprn;

                    foreach (var rating in ratings)
                    {
                        if (!string.IsNullOrEmpty(rating.Rating))
                        {
                            feedbackResult.ProviderRating.Add(rating.Rating, rating.Count);
                        }
                    }

                    // @ToDo: a more elegant way of doing this ?
                    var apprenticeFeedbackResultIds = sprocResult.Where(r => r.Ukprn == ukprn).Select(sr => sr.ApprenticeFeedbackResultId).Distinct();
                    var providerAttributeResults = _providerAttributeContext.Entities.Where(a => apprenticeFeedbackResultIds.Contains(a.ApprenticeFeedbackResultId)).ToList();

                    foreach (var a in attributes)
                    {
                        feedbackResult.ProviderAttribute.Add(new Domain.Entities.AttributeResult()
                        {
                            Name = a.AttributeName,
                            Category = a.Category,
                            Agree = providerAttributeResults.Where(par => par.AttributeId == a.AttributeId && par.AttributeValue == 1).Count(),
                            Disagree = providerAttributeResults.Where(par => par.AttributeId == a.AttributeId && par.AttributeValue == 0).Count()
                        });
                    }

                    result.UkprnFeedbacks.Add(feedbackResult);
                }
            }

            return result;
        }
    }
}