﻿using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetProvider
{
    public class GetProviderByUkprnQueryHandler : IRequestHandler<GetProviderByUkprnQuery, GetProviderByUkprnResult>
    {
        private readonly IApprenticeFeedbackTargetContext _apprenticeFeedbackTargetDbContext;
        private readonly ApplicationSettings _appSettings;

        public GetProviderByUkprnQueryHandler(IApprenticeFeedbackTargetContext apprenticeFeedbackTargetDbContext,
            ApplicationSettings appSettings)
        {
            _apprenticeFeedbackTargetDbContext = apprenticeFeedbackTargetDbContext;
            _appSettings = appSettings;
        }

        public async Task<GetProviderByUkprnResult> Handle(GetProviderByUkprnQuery request, CancellationToken cancellationToken)
        {
            var apprenticeFeedbackTargets = await _apprenticeFeedbackTargetDbContext.GetAllForApprenticeIdAndUkprnAndIncludeFeedbackResultsAsync(request.ApprenticeId, request.Ukprn);

            if (apprenticeFeedbackTargets == null || !apprenticeFeedbackTargets.Any())
            {
                return null;
            }

            var apprenticeFeedbackTarget = apprenticeFeedbackTargets
                .FilterForEligibleActiveApprenticeFeedbackTargets()
                .Select(s => TrainingProvider.Create(s, _appSettings))
                .SingleOrDefault();

            return new GetProviderByUkprnResult
            {
                TrainingProvider = apprenticeFeedbackTarget
            };
        }
    }
}
