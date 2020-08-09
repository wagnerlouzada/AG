using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AppV.Models;
using Microsoft.AspNetCore.Hosting;

namespace AppV.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            MyMethod(HttpContext);
            return View();
        }

        public void MyMethod(Microsoft.AspNetCore.Http.HttpContext context)
        {
            var host = $"{context.Request.Scheme}://{context.Request.Host}";

            // Other code
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Sobre Virtual Visitor";

            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult VirtualVisitor()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return base.View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
