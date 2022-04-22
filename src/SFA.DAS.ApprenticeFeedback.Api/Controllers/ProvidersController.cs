using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetAllProviders;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class ProvidersController : Controller
    {
        private readonly IMediator _mediator;

        public ProvidersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{apprenticeId}")]
        public async Task<IActionResult> GetAllProvidersForApprentice(Guid apprenticeId)
        {
            var result = await _mediator.Send(new GetAllProvidersForApprenticeQuery() { ApprenticeId = apprenticeId });
            return Ok(result);
        }
    }
}
