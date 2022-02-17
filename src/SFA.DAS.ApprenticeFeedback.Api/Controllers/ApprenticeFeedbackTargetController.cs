using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Api.ApiRequests;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateFeedbackTarget;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class ApprenticeFeedbackTargetController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ApprenticeFeedbackTargetController> _logger;

        public ApprenticeFeedbackTargetController(IMediator mediator, ILogger<ApprenticeFeedbackTargetController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateApprenticeFeedbackTargetRequest request)
        {
            try
            {
                var command = new CreateApprenticeFeedbackTargetCommand
                {
                    FeedbackTarget = new ApprenticeFeedbackTarget
                    {
                        ApprenticeId = request.ApprenticeId,
                        ApprenticeshipId = request.ApprenticeshipId,
                        Status = request.Status
                    }
                };

                var result = await _mediator.Send(command);

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error attempting to create apprentice feedback target for ApprenticeId: {request.ApprenticeId}, ApprenticeshipId {request.ApprenticeshipId}");
                
                return BadRequest();
            }
        }
    }
}
