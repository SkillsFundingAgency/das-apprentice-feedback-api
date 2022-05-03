using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetAllProviders;
using SFA.DAS.ApprenticeFeedback.Application.Queries.GetProvider;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class ProvidersController : Controller
    {
        private readonly IMediator _mediator;
        /// <summary>
        /// The providers controller is designed to map back from apprentice feedback targets into the providers that a 
        /// given apprentice can feedback upon.
        /// 
        /// As a result, when supplying the apprentice Id, that can correspond to many feedback records, but the
        /// combination of apprentice id plus ukprn should act as a composite key for an individual provider ( if it exists )
        /// 
        /// There is a possiblity an apprentice has more than one apprenticeship with one provider due to overlap
        /// but this is filtered out at the handler level so only one apprenticeship / target is returned.
        /// 
        /// This is to explain why the UKPRN isn't the unique key of this controller.
        /// </summary>
        /// <param name="mediator"></param>
        public ProvidersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{apprenticeId}")]
        public async Task<IActionResult> GetAllProvidersForApprentice(Guid apprenticeId)
        {
            var result = await _mediator.Send(new GetAllProvidersForApprenticeQuery() { ApprenticeId = apprenticeId });
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("{apprenticeId}/{ukprn}")]
        public async Task<IActionResult> GetProviderForApprenticeAndUkprn(Guid apprenticeId, long ukprn)
        {
            var result = await _mediator.Send(new GetProviderByUkprnQuery() { ApprenticeId = apprenticeId, Ukprn = ukprn });
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
