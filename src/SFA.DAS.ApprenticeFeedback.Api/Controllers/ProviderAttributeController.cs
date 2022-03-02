using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeFeedback.Domain.Interfaces;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    [ApiController]
    public class ProviderAttributeController : Controller
    {
        private readonly IApprenticeFeedbackRepository _apprenticeFeedbackRepository;
       
        public ProviderAttributeController(IApprenticeFeedbackRepository apprenticeFeedbackRepository)
        {
            _apprenticeFeedbackRepository = apprenticeFeedbackRepository;
        }

        [HttpGet("/")]
        public async Task<IActionResult> Index()
        {
            var attributes = _apprenticeFeedbackRepository.GetProviderAttributes();
            return Ok(attributes);
        }
    }
}
