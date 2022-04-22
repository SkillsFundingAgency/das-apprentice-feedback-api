using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateApprenticeFeedbackTarget;
using SFA.DAS.ApprenticeFeedback.Application.Commands.UpdateApprenticeFeedbackTarget;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetApprenticeFeedbackTargets;
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

        [HttpGet("{apprenticeId}")]
        public async Task<IActionResult> GetAllForApprentice(Guid apprenticeId)
        {
            try
            {
                var result = await _mediator.Send(new GetApprenticeFeedbackTargetsQuery { ApprenticeId = apprenticeId });
                return Ok(result.ApprenticeFeedbackTargets);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error attempting to retrieve apprentice feedback targets for ApprenticeId: {apprenticeId}");

                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateApprenticeFeedbackTargetCommand request)
        {
            try
            {
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error attempting to create apprentice feedback target for ApprenticeId: {request.ApprenticeId}, Commitments Apprentice Id {request.CommitmentApprenticeshipId}");
                
                return BadRequest();
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> ProcessUpdate([FromBody] UpdateApprenticeFeedbackTargetCommand request)
        {
            try
            {
                var result = await _mediator.Send(request);
                return Ok(result.UpdatedApprenticeFeedbackTarget);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error attempting to update apprentice feedback target for ApprenticeFeedbackTargetId: {request.ApprenticeFeedbackTargetId}");
                return BadRequest();
            }
        }
    }
}
