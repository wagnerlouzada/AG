using Ag.Domain;
using Api.Models;
using Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
                FilterId = "ID=" + filter.ToString();
                Filter = FilterId;
            }
            catch { }

            var contents = GetContent(Fields, Filter, Order, Top);
            return new JsonResult(contents);        
        }

        public List<dynamic> GetContent(string Fields = "", string Filter = "", string Order = "", int Top = 0)
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

            var contents = _context.AgContent
                                    .Where(condicao)
                                    .OrderBy(ordem)
                                    //.Select(campos)
                                    .Select(p => p)
                                    .Take(qtd)
                                    .ToDynamicList();

            return contents;

        }

    }
}
