using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetAttributes;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class ProviderAttributesController : Controller
    {
        private readonly IMediator _mediator;

        public ProviderAttributesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetProviderAttributes()
        {
            var result = await _mediator.Send(new GetAttributesQuery());

            return Ok(result.Attributes);
        }
    }
}
