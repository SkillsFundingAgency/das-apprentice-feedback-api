using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeFeedback.OuterApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeFeedback.Api.Controllers
{
    [ApiController]
    public class StandardsController : Controller
    {
        [HttpPost]
        public IActionResult CacheStandards(CacheStandardsCommand standards)
        {
            return View();
        }
    }
}
