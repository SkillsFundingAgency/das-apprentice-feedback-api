using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateExitSurvey;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class ExitSurveyController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ExitSurveyController> _logger;

        public ExitSurveyController(IMediator mediator, ILogger<ExitSurveyController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateExitSurvey([FromBody] CreateExitSurveyCommand request)
        {
            try
            {
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error attempting to create exit survey for apprentice feedback target id: {request.ApprenticeFeedbackTargetId}");
                return BadRequest();
            }
        }
    }
}
