using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedback;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackResult;
using System;
using System.Linq;
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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateApprenticeFeedbackCommand request)
        {
            try
            {
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error saving apprentice feedback to database.");
                return BadRequest();
            }
        }


        [HttpGet("{ukprn}")]
        public async Task<IActionResult> GetUkprn(long ukprn)
        {
            var result = await _mediator.Send(new GetApprenticeFeedbackResultsQuery { Ukprns = new long[]{ ukprn } });
            if (null == result.UkprnFeedbacks || !result.UkprnFeedbacks.Any())
            {
                return new StatusCodeResult(204);
            }
            return Ok(result.UkprnFeedbacks);
        }

        [HttpPost("request")]
        public async Task<IActionResult> PostUkprns([FromBody] GetApprenticeFeedbackResultsQuery request)
        {
            var result = await _mediator.Send(request);
            if (null == result.UkprnFeedbacks || !result.UkprnFeedbacks.Any())
            {
                return new StatusCodeResult(204);
            }
            return Ok(result.UkprnFeedbacks);
        }
    }
}
