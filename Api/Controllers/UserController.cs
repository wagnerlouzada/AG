using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Identity.Data;

namespace Ag.ApiControllers
{
    [Produces("application/json")]
    [Route("api")]
    public class UserController : Controller
    {

        [Route("user")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("text/welcome")]
        [Authorize]
        public IActionResult GetWelcomeText()
        {
            return Content("Welcome " + User.Identity.Name);
        }

        [Route("user")]
        [Authorize]
        public IActionResult GetUser()
        {
            return Content("User: " + User.Identity.Name);
        }

    }
}
