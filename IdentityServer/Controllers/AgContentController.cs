using Ag.Domain;
using Api.Models;
using Identity.Data;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ag.ApiControllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AgContentController : Controller
    {

        private readonly AgContext _context;

        public AgContentController(AgContext context)
        {
            _context = context;
        }

        [Route("home")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("GetContents")]
        [Authorize]
        public IActionResult GetContents(string Fields = "", string Filter = "", string Order = "", int Top = 0)
        {

            String FilterId = "";
            try
            {
                long filter = Convert.ToInt64(Filter);
                FilterId = "ID==" + filter.ToString();
                Filter = FilterId;
            }
            catch { }

            var contents = GetContent(Fields, Filter, Order, Top);
            return new JsonResult(contents);        
        }

        public List<AgContent> GetContent(string Fields = "", string Filter = "", string Order = "", int Top = 0)
        {
            string condicao = "";
            string ordem = "";
            string campos = "";
            int qtd = 10;

            if (Filter != "")
            {
                condicao = Filter;
            }
            else
            {
                condicao = "FkRoot == 1";
            }

            if (Order != "")
            {
                ordem = Order;
            }
            else
            {
                ordem = "Description";
            }

            if (Fields != "")
            {
                campos = "new (" + Fields + ")";
            }
            else
            {
                campos = "new (Id, Description, KEY, FkRoot)";
            }

            if (Top > 0)
            {
                qtd = Top;
            }
            else
            {
                qtd = 10;
            }

            List<AgContent> contents = _context.AgContent
                                    .Where(condicao)
                                    .OrderBy(ordem)
                                    .Select(p => p)
                                    .Take(qtd)
                                    .ToList();

            return contents;

        }

        // GET: api/AgContent
        [HttpGet]
        [Authorize] // sync Task<IActionResult> 
        //public async Task<IEnumerable<AgContent>> Get()
        public async Task<IActionResult> Get()
        {
            //
            //string accessToken;
            //try
            //{
            //    accessToken = await GetAccessToken();
            //}
            //catch (Exception ex)
            //{
            //    //ViewBag.Error = ex.GetBaseException().Message;
            //    //return View();
            //    return null;
            //}
            //
            //var client = new HttpClient();
            //client.SetBearerToken(accessToken);

            var contents =  GetContent( ) ;

            return View("List", contents);
        }


        [HttpGet("{id}", Name = "Edit")]
        [Authorize]
        //public AgContent Get(long id)
        public async Task<IActionResult> Edit(long id)
        {

            //string accessToken;
            //try
            //{
            //    accessToken = await GetAccessToken();
            //}
            //catch (Exception ex)
            //{
            //    //ViewBag.Error = ex.GetBaseException().Message;
            //    //return View();
            //    return null;
            //}

            //var client = new HttpClient();
            //client.SetBearerToken(accessToken);
            string Filtro = "ID==" + id.ToString() + "";
            //String url = HttpUtility.UrlEncode( $"https://localhost:44374/api/agcontent/getcontents/?Filter={id.ToString}");
            //String url = $"https://localhost:44374/api/agcontent/getcontents/?Filter={id.ToString()}";

            //var content = await client.GetStringAsync(url);
            //var ApiResponse = content;

            //var AccessToken = accessToken;
            //var RefreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            //var Json = JArray.Parse(content).ToString();

            //var result = JsonConvert.DeserializeObject<List<AgContent>>(Json).Take(1);
            //AgContent res = result.FirstOrDefault();

            //return result;
            var contents = GetContent("",Filtro,"",1).Take(1).FirstOrDefault();

            return View("Edit", contents); ;// result; // new List<AgContent>();
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
