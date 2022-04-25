using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
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
            var foo = await _apprenticeFeedbackRepository.GetProvidersForFeedback(request.ApprenticeId);

            var result = new GetAllProvidersForApprenticeResult()
            {

            };

            return result;
        }
    }
}
