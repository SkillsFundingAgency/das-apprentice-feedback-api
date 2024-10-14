using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog;
using SFA.DAS.ApprenticeFeedback.Api.TaskQueue;
using SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessFeedbackTargetVariants;
using SFA.DAS.ApprenticeFeedback.Application.Extensions;
using System;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class FeedbackTargetVariantController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly ILogger<FeedbackTargetVariantController> _log;

        public FeedbackTargetVariantController(IMediator mediator,ILogger<FeedbackTargetVariantController> log, 
            IBackgroundTaskQueue backgroundTaskQueue)
        {
            _mediator = mediator;
            _backgroundTaskQueue = backgroundTaskQueue;
            _log = log;
        }

        [HttpPost("process-variants")]
        public async Task<IActionResult> ProcessVariants(ProcessFeedbackTargetVariantsCommand command)
        {
            var requestName = "process feedback target variants";

            try
            {
                _log.LogInformation("Received request to {RequestName}", requestName);

                _backgroundTaskQueue.QueueBackgroundRequest(
                    command, requestName, (response, duration, log) =>
                    {
                        log.LogInformation($"Completed request to {requestName}");
                    });

                _log.LogInformation("Queued request to {RequestName}", requestName);

                return Accepted();
            }
            catch (Exception e)
            {
                _log.LogError(e, $"Error attempting to process feedback target variants");
                return BadRequest();
            }
        }
    }
}
