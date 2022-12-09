using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetAttributes;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class AttributesController : Controller
    {
        private readonly IMediator _mediator;

        public AttributesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{attributeType}")]
        public async Task<IActionResult> GetAttributes(string attributeType)
        {
            if (string.IsNullOrWhiteSpace(attributeType)) return BadRequest();

            var result = await _mediator.Send(new GetAttributesQuery() { AttributeType = attributeType });

            return Ok(result.ProviderAttributes);
        }
    }
}
