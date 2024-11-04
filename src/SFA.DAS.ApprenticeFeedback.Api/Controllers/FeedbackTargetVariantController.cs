using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Api.TaskQueue;
using SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessFeedbackTargetVariants;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class FeedbackTargetVariantController : ControllerBase
    {
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly ILogger<FeedbackTargetVariantController> _log;

        public FeedbackTargetVariantController(ILogger<FeedbackTargetVariantController> log, 
            IBackgroundTaskQueue backgroundTaskQueue)
        {
            _backgroundTaskQueue = backgroundTaskQueue;
            _log = log;
        }

        [HttpPost("process-variants")]
        public IActionResult ProcessVariants(ProcessFeedbackTargetVariantsCommand command)
        {
            var requestName = "process feedback target variants";

            try
            {
                _log.LogInformation("Received request to {RequestName}", requestName);

                _backgroundTaskQueue.QueueBackgroundRequest(
                    command, requestName, (response, duration, log) =>
                    {
                        log.LogInformation("Completed request {RequestName}", requestName);
                    });

                _log.LogInformation("Queued request to {RequestName}", requestName);

                return Accepted();
            }
            catch (Exception e)
            {
                _log.LogError(e, "Error attempting to {RequestName}", requestName);
                return BadRequest();
            }
        }
    }
}
