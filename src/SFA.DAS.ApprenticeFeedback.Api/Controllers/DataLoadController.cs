using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Application.Commands.GenerateFeedbackSummaries;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class DataLoadController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DataLoadController> _logger;

        public DataLoadController(IMediator mediator, ILogger<DataLoadController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("generate-feedback-summaries")]
        public async Task<IActionResult> GenerateFeedbackSummaries()
        {
            try
            {
                _logger.LogInformation("Beginning DataLoad to Generate Feedback Summaries");
                await _mediator.Send(new GenerateFeedbackSummariesCommand());
                _logger.LogInformation("Finished DataLoad to Generate Feedback Summaries");
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed DataLoad to Generate Feedback Summaries");
                return BadRequest();
            }
        }
    }
}
