using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetAttributes;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    [ApiController]
    public class ProviderAttributeController : Controller
    {
        private readonly IMediator _mediator;

        public ProviderAttributeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("/")]
        public async Task<IActionResult> Index()
        {
            var result = await _mediator.Send(new GetAttributesQuery());
            return Ok(result.Attributes);
        }
    }
}
