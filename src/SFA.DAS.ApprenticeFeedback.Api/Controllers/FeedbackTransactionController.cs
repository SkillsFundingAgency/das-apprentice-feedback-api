using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Api.TaskQueue;
using SFA.DAS.ApprenticeFeedback.Application.Commands.GenerateFeedbackTransactions;
using SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessEmailTransaction;
using SFA.DAS.ApprenticeFeedback.Application.Commands.TrackEmailTransactionClick;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetFeedbackTransactions;
using SFA.DAS.ApprenticeFeedback.Application.Extensions;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class FeedbackTransactionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<FeedbackTransactionController> _log;

        public FeedbackTransactionController(IMediator mediator, IBackgroundTaskQueue taskQueue, ILogger<FeedbackTransactionController> log)
        {
            _mediator = mediator;
            _taskQueue = taskQueue;
            _log = log;
        }

        [HttpPost("generate-email-transactions")]
        public IActionResult GenerateEmailTransactions()
        {
            var requestName = "generate feedback transactions";

            try
            {
                _log.LogInformation($"Received request to {requestName}");

                _taskQueue.QueueBackgroundRequest(
                    new GenerateFeedbackTransactionsCommand(), requestName, (response, duration, log) =>
                    {
                        var result = response as GenerateFeedbackTransactionsCommandResponse;
                        log.LogInformation($"Completed request to {requestName}: Resulted in {result.Count} transactions with created date {result.CreatedOn} in {duration.ToReadableString()}");
                    });

                _log.LogInformation($"Queued request to {requestName}");
                
                return Accepted();
            }
            catch (Exception e)
            {
                _log.LogError(e, $"Error attempting to {requestName}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error {requestName}: {e.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFeedbackTransactionsToEmail(int batchSize)
        {
            try
            {
                var result = await _mediator.Send(new GetFeedbackTransactionsToEmailQuery() { BatchSize = batchSize });
                return Ok(result.FeedbackTransactionsToEmail);
            }
            catch (Exception e)
            {
                var msg = "Error attempting to get feedback transactions to email";
                _log.LogError(e, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, $"{msg}: {e.Message}");
            }
        }

        [HttpPost("{feedbackTransactionId}")]
        public async Task<IActionResult> ProcessEmailTransaction(int feedbackTransactionId, [FromBody] ProcessEmailTransactionCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                if (null == result) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                var msg = $"Error attempting to process feedback transaction id [{feedbackTransactionId}] to email";
                _log.LogError(e, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, $"{msg}: {e.Message}");
            }
        }

        [HttpPost("{feedbackTransactionId}/track-click")]
        public async Task<IActionResult> TrackEmailTransactionClick(long feedbackTransactionId, [FromBody] TrackEmailTransactionClickCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception e)
            {
                var msg = $"Error attempting to record feedback transaction id [{feedbackTransactionId}] email click";
                _log.LogError(e, msg);
                return StatusCode(StatusCodes.Status500InternalServerError, $"{msg}: {e.Message}");
            }
        }
    }
}
