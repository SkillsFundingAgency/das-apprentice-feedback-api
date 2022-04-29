﻿using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetProvider
{
    public class GetProviderByUkprnQueryHandler : IRequestHandler<GetProviderByUkprnQuery, GetProviderByUkprnResult>
    {
        private readonly IApprenticeFeedbackRepository _apprenticeFeedbackRepository;

        public GetProviderByUkprnQueryHandler(IApprenticeFeedbackRepository apprenticeFeedbackRepository)
        {
            _apprenticeFeedbackRepository = apprenticeFeedbackRepository;
        }

        public async Task<GetProviderByUkprnResult> Handle(GetProviderByUkprnQuery request, CancellationToken cancellationToken)
        {
            var apprenticeFeedbackTargets = await _apprenticeFeedbackRepository.GetApprenticeFeedbackTargets(request.ApprenticeId, request.Ukprn);
                        
            if (apprenticeFeedbackTargets?.Any() == false)
            {
                return default;
            }

            TrainingProvider trainingProvider = apprenticeFeedbackTargets
                .Select(r => (ApprenticeFeedbackTarget)r)
                .FilterForEligibleActiveApprenticeFeedbackTargets()
                .SingleOrDefault();

            return new GetProviderByUkprnResult
            {
                TrainingProvider = trainingProvider
            };
        }
    }
}
