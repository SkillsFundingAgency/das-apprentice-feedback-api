using MediatR;
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
        private readonly ApplicationSettings _appSettings;

        public GetProviderByUkprnQueryHandler(IApprenticeFeedbackRepository apprenticeFeedbackRepository,
            ApplicationSettings appSettings)
        {
            _apprenticeFeedbackRepository = apprenticeFeedbackRepository;
            _appSettings = appSettings;
        }

        public async Task<GetProviderByUkprnResult> Handle(GetProviderByUkprnQuery request, CancellationToken cancellationToken)
        {
            var apprenticeFeedbackTargets = await _apprenticeFeedbackRepository.GetApprenticeFeedbackTargets(request.ApprenticeId, request.Ukprn);
                        
            if (apprenticeFeedbackTargets == null || !apprenticeFeedbackTargets.Any())
            {
                return null;
            }

            var apprenticeFeedbackTarget = apprenticeFeedbackTargets
                .Select(r => (ApprenticeFeedbackTarget)r)
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
