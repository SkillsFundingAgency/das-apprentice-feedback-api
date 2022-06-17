using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Application.Queries.GetAttributes
{
    public class GetAttributesQueryHandler : IRequestHandler<GetAttributesQuery, GetAttributesResult>
    {
        private readonly IAttributeContext _attributeContext;

        public GetAttributesQueryHandler(IAttributeContext attributeContext)
        {
            _attributeContext = attributeContext;
        }
        public async Task<GetAttributesResult> Handle(GetAttributesQuery request, CancellationToken cancellationToken)
        {
            var entities = await _attributeContext.Entities.ToListAsync();

            var attributes = entities.Select(entity => (Domain.Models.Attribute)entity).ToList();

            return new GetAttributesResult
            {
                ProviderAttributes = attributes
            };
        }
    }
}
