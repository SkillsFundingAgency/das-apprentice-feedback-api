using MediatR;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetAttributes
{
    public class GetAttributesQueryHandler : IRequestHandler<GetAttributesQuery, GetAttributesResult>
    {
        private readonly IApprenticeFeedbackRepository _apprenticeFeedbackRepository;

        public GetAttributesQueryHandler(IApprenticeFeedbackRepository apprenticeFeedbackRepository)
        {
            _apprenticeFeedbackRepository = apprenticeFeedbackRepository;
        }
        public async Task<GetAttributesResult> Handle(GetAttributesQuery request, CancellationToken cancellationToken)
        {
            var entities = await _apprenticeFeedbackRepository.GetAttributes();

            var attributes = entities.Select(entity => (Domain.Models.Attribute)entity).ToList();

            return new GetAttributesResult
            {
                ProviderAttributes = attributes
            };
        }
    }
}
