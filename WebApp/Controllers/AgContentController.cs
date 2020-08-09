using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ag.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using System.Web;

namespace AgRazor.Controllers
{
    //[Route("api/[controller]")]
    [Route("ag")]
    //[ApiController]
    public class AgContentController : Controller
    {

        // GET: api/AgContent
        [HttpGet]
        [Authorize] // sync Task<IActionResult> 
        //public async Task<IEnumerable<AgContent>> Get()
        public async Task<IActionResult>  Get()
        {

            string accessToken;
            try
            {
                accessToken = await GetAccessToken();
            }
            catch (Exception ex)
            {
                //ViewBag.Error = ex.GetBaseException().Message;
                //return View();
                return null;
            }

            var client = new HttpClient();
            client.SetBearerToken(accessToken);

            var content = await client.GetStringAsync("https://localhost:44374/api/agcontent/getcontents");
            var ApiResponse = content;

            var AccessToken = accessToken;
            var RefreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            var Json = JArray.Parse(content).ToString();

            var result = JsonConvert.DeserializeObject<List<AgContent>>(Json);

            //return result;
            return View("List", result); ;// result; // new List<AgContent>();
        }

        ////// GET: api/AgContent/5
        //[HttpGet("{id}", Name = "GetContent")]
        //[Authorize]
        ////public AgContent Get(long id)
        //public async Task<IActionResult> Get(long id)
        //{

        //    string accessToken;
        //    try
        //    {
        //        accessToken = await GetAccessToken();
        //    }
        //    catch (Exception ex)
        //    {
        //        //ViewBag.Error = ex.GetBaseException().Message;
        //        //return View();
        //        return null;
        //    }

        //    var client = new HttpClient();
        //    client.SetBearerToken(accessToken);

        //    var content = await client.GetStringAsync("https://localhost:44374/api/agcontent/getcontents");
        //    var ApiResponse = content;

        //    var AccessToken = accessToken;
        //    var RefreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
        //    var Json = JArray.Parse(content).ToString();

        //    var result = JsonConvert.DeserializeObject<List<AgContent>>(Json);

        //    //return result;
        //    return View("List", result); ;// result; // new List<AgContent>();
        //}

        [HttpGet("{id}", Name = "Edit")]
        [Authorize]
        //public AgContent Get(long id)
        public async Task<IActionResult> Edit(long id)
        {

            string accessToken;
            try
            {
                accessToken = await GetAccessToken();
            }
            catch (Exception ex)
            {
                //ViewBag.Error = ex.GetBaseException().Message;
                //return View();
                return null;
            }

            var client = new HttpClient();
            client.SetBearerToken(accessToken);
            string Filtro = "\"ID=" + id.ToString()  + "\"";
            //String url = HttpUtility.UrlEncode( $"https://localhost:44374/api/agcontent/getcontents/?Filter={id.ToString}");
            String url = $"https://localhost:44374/api/agcontent/getcontents/?Filter={id.ToString()}";

            var content = await client.GetStringAsync(url);
            var ApiResponse = content;

            var AccessToken = accessToken;
            var RefreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            var Json = JArray.Parse(content).ToString();

            var result = JsonConvert.DeserializeObject<List<AgContent>>(Json).Take(1);
            AgContent res = result.FirstOrDefault();

            //return result;
            return View("Edit", res); ;// result; // new List<AgContent>();
        }


        // POST: api/AgContent
        [HttpPost]
        [Authorize]
        public void Post([FromBody] AgContent value)
        {
        }

        //// PUT: api/AgContent/5
        [HttpPut("{id}")]
        [Authorize]
        public void Put(long id, [FromBody] AgContent value)
        {
        }

        //// DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [Authorize]
        public void Delete(long id)
        {
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
