using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackTargets
{
    public class GetApprenticeFeedbackTargetsQueryHandler : IRequestHandler<GetApprenticeFeedbackTargetsQuery, GetApprenticeFeedbackTargetsResult>
    {
        private readonly IApprenticeFeedbackRepository _apprenticeFeedbackRepository;

        public GetApprenticeFeedbackTargetsQueryHandler(IApprenticeFeedbackRepository apprenticeFeedbackRepository)
        {
            _apprenticeFeedbackRepository = apprenticeFeedbackRepository;
        }
        public async Task<GetApprenticeFeedbackTargetsResult> Handle(GetApprenticeFeedbackTargetsQuery request, CancellationToken cancellationToken)
        {
            var entities = await _apprenticeFeedbackRepository.GetApprenticeFeedbackTarget(request.ApprenticeId);

            var attributes = entities.Select(entity => (Domain.Models.Attribute)entity).ToList();

            return new GetApprenticeFeedbackTargetsResult
            {
                ProviderAttributes = attributes
            };
        }
    }
}
