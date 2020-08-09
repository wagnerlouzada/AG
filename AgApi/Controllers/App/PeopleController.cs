using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Base.Controllers;
using Models.Requests.Controllers;
using Microsoft.Extensions.Options;
using System.IO;

using Microsoft.AspNetCore.Hosting;

namespace AppV.Controllers.People
{
    public class PeopleController : BaseControllerExtended
    {

        public PeopleController(IMediator mediator, IOptions<AppV.Models.ConfigurationString> appSettings)  : base(mediator, appSettings)
        {
        }

        //public PeopleController(IMediator mediator, ILogger logger) : base(mediator, logger)
        //{ 
        //}
        //public PeopleController() // : base(mediator, logger)
        //{ }

        public JsonResult GetPeople(PeopleRequest req)
        {
            var resp = Mediator.Send(req).Result;
            //return Json(resp, JsonRequestBehavior.DenyGet);
            return Json(resp );
        }

        public JsonResult GetPeopleList(PeopleListRequest req)
        {
            var resp = Mediator.Send(req).Result;
            return Json(resp);
        }

        // GET: People
        public ActionResult Index()
        {
            return View();
        }

        // GET: People/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: People/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: People/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: People/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: People/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: People/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: People/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: People/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult SavePeople(SavePeopleRequest req)
        {
            try
            {
                var resp = Mediator.Send(req).Result;
                //return Json(resp, JsonRequestBehavior.DenyGet);
                return Json(resp);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

    }


    //public class CameraController : Controller
    //{
    //    private readonly DatabaseContext _context;
    //    private readonly IHostingEnvironment _environment;
    //    public CameraController(IHostingEnvironment hostingEnvironment, DatabaseContext context)
    //    {
    //        _environment = hostingEnvironment;
    //        _context = context;
    //    }
    //
    //    [HttpGet]
    //    public IActionResult Capture()
    //    {
    //        return View();
    //    }
    //
    //    [HttpPost]
    //    public IActionResult Capture(string name)
    //    {
    //        var files = HttpContext.Request.Form.Files;
    //        if (files != null)
    //        {
    //            foreach (var file in files)
    //            {
    //                if (file.Length > 0)
    //                {
    //                    // Getting Filename  
    //                    var fileName = file.FileName;
    //                    // Unique filename "Guid"  
    //                    var myUniqueFileName = Convert.ToString(Guid.NewGuid());
    //                    // Getting Extension  
    //                    var fileExtension = Path.GetExtension(fileName);
    //                    // Concating filename + fileExtension (unique filename)  
    //                    var newFileName = string.Concat(myUniqueFileName, fileExtension);
    //                    //  Generating Path to store photo   
    //                    var filepath = Path.Combine(_environment.WebRootPath, "CameraPhotos") + $@"\{newFileName}";
    //
    //                    if (!string.IsNullOrEmpty(filepath))
    //                    {
    //                        // Storing Image in Folder  
    //                        StoreInFolder(file, filepath);
    //                    }
    //
    //                    var imageBytes = System.IO.File.ReadAllBytes(filepath);
    //                    if (imageBytes != null)
    //                    {
    //                        // Storing Image in Folder  
    //                        StoreInDatabase(imageBytes);
    //                    }
    //
    //                }
    //            }
    //            return Json(true);
    //        }
    //        else
    //        {
    //            return Json(false);
    //        }
    //
    //    }
    //
    //    /// <summary>  
    //    /// Saving captured image into Folder.  
    //    /// </summary>  
    //    /// <param name="file"></param>  
    //    /// <param name="fileName"></param>  
    //    private void StoreInFolder(IFormFile file, string fileName)
    //    {
    //        using (FileStream fs = System.IO.File.Create(fileName))
    //        {
    //            file.CopyTo(fs);
    //            fs.Flush();
    //        }
    //    }
    //
    //    /// <summary>  
    //    /// Saving captured image into database.  
    //    /// </summary>  
    //    /// <param name="imageBytes"></param>  
    //    private void StoreInDatabase(byte[] imageBytes)
    //    {
    //        try
    //        {
    //            if (imageBytes != null)
    //            {
    //                string base64String = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
    //                string imageUrl = string.Concat("data:image/jpg;base64,", base64String);
    //
    //                ImageStore imageStore = new ImageStore()
    //                {
    //                    CreateDate = DateTime.Now,
    //                    ImageBase64String = imageUrl,
    //                    ImageId = 0
    //                };
    //
    //                _context.ImageStore.Add(imageStore);
    //                _context.SaveChanges();
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            throw;
    //        }
    //    }
    //}

}