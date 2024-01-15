using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Api.TaskQueue;
using SFA.DAS.ApprenticeFeedback.Application.Commands.GenerateFeedbackSummaries;
using SFA.DAS.ApprenticeFeedback.Application.Commands.GenerateFeedbackTransactions;
using SFA.DAS.ApprenticeFeedback.Application.Extensions;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class DataLoadController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<DataLoadController> _logger;

        public DataLoadController(IMediator mediator, IBackgroundTaskQueue taskQueue, ILogger<DataLoadController> logger)
        {
            _mediator = mediator;
            _taskQueue = taskQueue;
            _logger = logger;
        }

        [HttpPost("generate-feedback-summaries")]
        public IActionResult GenerateFeedbackSummaries()
        {
            var requestName = "generate feedback summaries";

            try
            {
                _logger.LogInformation($"Received request to {requestName}");

                _taskQueue.QueueBackgroundRequest(
                    new GenerateFeedbackSummariesCommand(), requestName, (response, duration, log) =>
                    {
                        var result = response as GenerateFeedbackSummariesCommandResponse;
                        log.LogInformation($"Completed request to {requestName}: Request completed in {duration.ToReadableString()}");
                    });

                _logger.LogInformation($"Queued request to {requestName}");

                return Accepted();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error attempting to {requestName}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error {requestName}: {e.Message}");
            }
        }
    }
}
