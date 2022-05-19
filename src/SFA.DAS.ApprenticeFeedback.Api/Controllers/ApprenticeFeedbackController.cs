using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedback;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class ApprenticeFeedbackController : ControllerBase
    {
        public readonly IMediator _mediator;
        public readonly ILogger<ApprenticeFeedbackController> _logger;

        public ApprenticeFeedbackController(IMediator mediator, ILogger<ApprenticeFeedbackController> logger)
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
    }
}
