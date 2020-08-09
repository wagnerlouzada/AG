using CacheManager.Core;
using System;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Global;
using Microsoft.Extensions.Options;
using Serilog;

namespace Base.Controllers
{
    [Authorize]
    public class BaseController : Controller, IDisposable
    {
        protected IMediator Mediator;
        //public readonly ILogger _logger;

        /// <summary>
        /// /
        /// </summary>
        private readonly IOptions<AppV.Models.ConfigurationString> _appSettings;

        //public BaseController(IMediator mediator)
        public BaseController(IMediator mediator, IOptions<AppV.Models.ConfigurationString> appSettings)
        {
            Mediator = mediator;
            //_logger = logger;

            //////
            ///
            /////////////
            _appSettings = appSettings;
        }

        private ICacheManager<Object> _cache;

        public ICacheManager<Object> cacheManager
        {
            get
            {
                _cache = _cache ?? CacheFactory.FromConfiguration<Object>(GlobalVariables.CacheName, null);
                return _cache;
            }
        }

        //public string Username
        //{
        //    get { return ((ClaimsPrincipal)HttpContext.User).FindFirst(ClaimTypes.Name).Value; }
        //}
        //
        //public ActionResult SignOut()
        //{
        //    //string[] parms = { "ApplicationCookie" };
        //    //Request.SignOut();
        //    //Session.Abandon();
        //    return RedirectToAction("Login", "Login");
        //}

        [HttpGet]
        public JsonResult ErrorTest()
        {
            int x, y;
            x = 10;
            y = 0;
            var total = x / y;
            return Json(x);
        }

    }
}
