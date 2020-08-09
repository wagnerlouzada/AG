using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;


namespace Base.Controllers
{
    [Authorize]
    public abstract class BaseControllerExtended : BaseController
    {

        //public readonly ILogger _logger;
        public String MasterLayout = "~/Views/Shared/_Layout.cshtml";
       

        //public BaseControllerExtended(IMediator mediator, ILogger logger) : base(mediator)
        //{
        //    _logger = logger;
        //}
        public BaseControllerExtended(IMediator mediator, IOptions<AppV.Models.ConfigurationString> appSettings) : base(mediator, appSettings)
        {
           // _logger = logger;
        }


    }
}