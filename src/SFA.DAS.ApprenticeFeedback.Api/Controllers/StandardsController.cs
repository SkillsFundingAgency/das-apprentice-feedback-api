using Microsoft.AspNetCore.Mvc;
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
        public IActionResult CacheStandards()
        {
            return View();
        }
    }
}
