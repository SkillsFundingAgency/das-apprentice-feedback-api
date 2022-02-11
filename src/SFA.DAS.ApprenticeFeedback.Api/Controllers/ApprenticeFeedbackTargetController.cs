using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeFeedback.Api.ApiRequests;
using SFA.DAS.ApprenticeFeedback.Application.Commands.CreateFeedbackTarget;
using SFA.DAS.ApprenticeFeedback.Domain.Models;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class ApprenticeFeedbackTargetController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApprenticeFeedbackTargetController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFeedbackTargetRequest request)
        {
            try
            {
                var command = new CreateFeedbackTargetCommand
                {
                    FeedbackTarget = new FeedbackTarget
                    {
                        ApprenticeId = request.ApprenticeId,
                        ApprenticeshipId = request.ApprenticeshipId,
                        Status = request.Status,
                        FirstName = request.FirstName,
                        EmailAddress = request.EmailAddress
                    }
                };

                await _mediator.Send(command);

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
