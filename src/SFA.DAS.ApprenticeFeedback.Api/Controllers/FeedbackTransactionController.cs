
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MediatR;
using SFA.DAS.ApprenticeFeedback.Application.Commands.GenerateFeedbackTransactions;
using Microsoft.AspNetCore.Http;

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
    }
}
