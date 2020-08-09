using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Ag.ApiControllers
{

    public class HomeController : Controller
    {

        [Route("home")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
            //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<string> GetAccessToken()
        {
            var exp = await HttpContext.GetTokenAsync("expires_at");
            var expires = DateTime.Parse(exp);

            if (expires > DateTime.Now)
            {
                return await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            }

            return await GetRefreshedAccessToken();
        }

        private async Task<string> GetRefreshedAccessToken()
        {
            var discoClient = await DiscoveryClient.GetAsync("https://localhost:44367");
            var tokenClient = new TokenClient(discoClient.TokenEndpoint, "mvc", "secret");
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            var tokenResponse = await tokenClient.RequestRefreshTokenAsync(refreshToken);

            if (!tokenResponse.IsError)
            {
                var auth = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                auth.Properties.UpdateTokenValue(OpenIdConnectParameterNames.AccessToken, tokenResponse.AccessToken);
                auth.Properties.UpdateTokenValue(OpenIdConnectParameterNames.RefreshToken, tokenResponse.RefreshToken);
                var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn);
                auth.Properties.UpdateTokenValue("expires_at", expiresAt.ToString("o", CultureInfo.InvariantCulture));
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, auth.Principal, auth.Properties);
                return tokenResponse.AccessToken;
            }

            throw tokenResponse.Exception;
        }

    }
}
