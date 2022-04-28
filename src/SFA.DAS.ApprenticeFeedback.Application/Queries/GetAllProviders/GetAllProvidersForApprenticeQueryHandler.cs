using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetAllProviders
{
    public class GetAllProvidersForApprenticeQueryHandler : IRequestHandler<GetAllProvidersForApprenticeQuery, GetAllProvidersForApprenticeResult>
    {
        private readonly IApprenticeFeedbackRepository _apprenticeFeedbackRepository;

        public GetAllProvidersForApprenticeQueryHandler(IApprenticeFeedbackRepository apprenticeFeedbackRepository)
        {
            _apprenticeFeedbackRepository = apprenticeFeedbackRepository;
        }

        public async Task<GetAllProvidersForApprenticeResult> Handle(GetAllProvidersForApprenticeQuery request, CancellationToken cancellationToken)
        {
            // Get all feedback targets for given apprentice guid.

            var afts = await _apprenticeFeedbackRepository.GetApprenticeFeedbackTargets(request.ApprenticeId);

            // Filter out ones we are not interested in.

            var filteredAfts = afts.Where(afts =>
                afts.StartDate.HasValue
                && afts.Ukprn.HasValue
                && afts.Status != (int)Domain.Models.ApprenticeFeedbackTarget.FeedbackTargetStatus.Unknown
                && afts.FeedbackEligibility != (int)Domain.Models.ApprenticeFeedbackTarget.FeedbackEligibilityStatus.Unknown
                && afts.FeedbackEligibility != (int)Domain.Models.ApprenticeFeedbackTarget.FeedbackEligibilityStatus.Deny_Complete
                );

            // Map to Provider

            var providers = filteredAfts.Select(p => new TrainingProvider()
            {
                StartDate = p.StartDate.Value,
                EndDate = p.EndDate,
                Status = p.Status,
                Ukprn = p.Ukprn.Value,
                ProviderName = p.ProviderName,
                FeedbackEligibility = p.FeedbackEligibility
            });


            var result = new GetAllProvidersForApprenticeResult()
            {
                TrainingProviders = providers,
                
            };

            return result;
        }
    }
}
