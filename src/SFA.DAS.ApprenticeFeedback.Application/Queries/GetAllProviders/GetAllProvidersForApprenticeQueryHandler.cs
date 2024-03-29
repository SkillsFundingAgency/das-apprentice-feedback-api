﻿using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain;
using SFA.DAS.ApprenticeFeedback.Domain.Configuration;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetAllProviders
{
    public class GetAllProvidersForApprenticeQueryHandler : IRequestHandler<GetAllProvidersForApprenticeQuery, GetAllProvidersForApprenticeResult>
    {
        private readonly IApprenticeFeedbackTargetContext _apprenticeFeedbackTargetDataContext;
        private readonly ApplicationSettings _appSettings;

        public GetAllProvidersForApprenticeQueryHandler(IApprenticeFeedbackTargetContext apprenticeFeedbackTargetDataContext,
            ApplicationSettings appSettings)
        {
            _apprenticeFeedbackTargetDataContext = apprenticeFeedbackTargetDataContext;
            _appSettings = appSettings;
        }

        public async Task<GetAllProvidersForApprenticeResult> Handle(GetAllProvidersForApprenticeQuery request, CancellationToken cancellationToken)
        {
            var afts = await _apprenticeFeedbackTargetDataContext.GetAllForApprenticeIdAndIncludeFeedbackResultsAsync(request.ApprenticeId);

            if (afts == null || !afts.Any())
            {
                return null;
            }

            var trainingProviders = afts
                .FilterForEligibleActiveApprenticeFeedbackTargets()
                .Select(s => TrainingProvider.Create(s, _appSettings));

            return new GetAllProvidersForApprenticeResult()
            {
                TrainingProviders = trainingProviders,
                InitialDenyPeriodDays = _appSettings.InitialDenyPeriodDays,
                FinalAllowedPeriodDays = _appSettings.FinalAllowedPeriodDays,
                RecentDenyPeriodDays = _appSettings.RecentDenyPeriodDays,
            };
        }
    }
}
