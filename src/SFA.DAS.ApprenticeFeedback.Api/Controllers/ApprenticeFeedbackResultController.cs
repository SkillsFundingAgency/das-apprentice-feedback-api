using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedback;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class ApprenticeFeedbackResultController : ControllerBase
    {
        public readonly IMediator _mediator;
        public readonly ILogger<ApprenticeFeedbackResultController> _logger;

        public ApprenticeFeedbackResultController(IMediator mediator, ILogger<ApprenticeFeedbackResultController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("/{ukprn}")]
        public IActionResult GetUkprn(long ukprn)
        {
            return Ok(TempDataPopulation.CreateFeedbackResponse(ukprn));
        }

        [HttpPost("/request")]
        public IActionResult PostUkprns([FromBody] FetchApprenticeFeedbackRequest request)
        {
            return Ok(request.Ukprns.Select(TempDataPopulation.CreateFeedbackResponse));
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
