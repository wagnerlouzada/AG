using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Identity.WebApp.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        [Route("signin")]
        public IActionResult SignIn()
        {
            var challenge = new AuthenticationProperties { RedirectUri = "/" };
            var result = Challenge(challenge);
            return result;
        }

        [Route("signout")]
        [HttpPost]
        public async Task SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync("oidc");
        }
    }
}