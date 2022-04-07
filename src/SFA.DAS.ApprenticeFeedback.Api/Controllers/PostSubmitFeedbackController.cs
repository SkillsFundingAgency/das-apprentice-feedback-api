using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class PostSubmitFeedbackController : ControllerBase
    {
        public readonly IMediator _mediator;
        public readonly ILogger<PostSubmitFeedbackController> _logger;

        public PostSubmitFeedbackController(IMediator mediator, ILogger<PostSubmitFeedbackController> logger)
        {
            _mediator = mediator;   
            _logger = logger;
        }

        //post method
    }
}
