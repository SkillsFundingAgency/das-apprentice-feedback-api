
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Application.Commands.GenerateFeedbackTransactions;
using SFA.DAS.ApprenticeFeedback.Application.Commands.ProcessEmailTransaction;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetFeedbackTransactions;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class FeedbackTransactionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<FeedbackTransactionController> _log;

        public FeedbackTransactionController(IMediator mediator, ILogger<FeedbackTransactionController> log)
        {
            _mediator = mediator;
            _log = log;
        }

        [HttpPost]
        public async Task<IActionResult> FeedbackTransaction()
        {
            try
            {
                _log.LogInformation("Starting FeedbackTransaction");
                var result = await _mediator.Send(new GenerateFeedbackTransactionsCommand());
                return Ok(result);
            }
            catch (Exception e)
            {
                _log.LogError(e, "Error attempting to generate feedback transactions");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error generating feedback transactions: {e.Message}");
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
    }
}
