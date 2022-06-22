using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackResult;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class ApprenticeFeedbackResultController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ApprenticeFeedbackResultController> _logger;

        public ApprenticeFeedbackResultController(IMediator mediator, ILogger<ApprenticeFeedbackResultController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{ukprn}")]
        public async Task<IActionResult> GetUkprn(long ukprn)
        {
            var result = await _mediator.Send(new GetApprenticeFeedbackResultQuery { Ukprn = ukprn });
            if(0 == result.Ukprn)
            {
                return new StatusCodeResult(204);
            }
            return Ok(result);
        }
    }
}
