using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using testNdoc.Models;

namespace testNdoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        NDocContext db;

         public HomeController(ILogger<HomeController> logger)
        {
            db = new NDocContext();
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View(await db.Sections.Where(x => x.IsRemove != true).ToListAsync());
        }

        public IActionResult GetLastDocuments()
        {
            var model = db.Documents.Where(x => x.IsRemove != true).ToList().Take(8).OrderByDescending(x => x.Id);
            return PartialView("TableDocument", model);
        }

        public IActionResult TableDocument(int id)
        {

            var model = db.Documents.Where(d => d.SectionId == id).OrderByDescending(x => x.Id); 
            return PartialView(model);
        }   

        public async Task <IActionResult> Search(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                try
                {
                    var model = await db.Documents.Where(x => x.Name.Contains(name)).ToListAsync();

                    //Debug.WriteLine("Кол-во строк " + model.Count);
                    return PartialView("TableDocument", model);

                }
                catch (Exception ex)
                {
                   Debug.WriteLine(ex.Message);
                }

            }
            return NotFound();

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

















        //private readonly IWebHostEnvironment webHostEnvironment;

        //public HomeController(IWebHostEnvironment webHostEnvironment)
        //{
        //    this.webHostEnvironment = webHostEnvironment;
        //}

        //public IActionResult OnPostMyUploader(IFormFile MyUploader)
        //{
        //    if (MyUploader != null)
        //    {
        //        string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Files");
        //        string filePath = Path.Combine(uploadsFolder, MyUploader.FileName);
        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            MyUploader.CopyTo(fileStream);
        //        }
        //        return new ObjectResult(new { status = "success" });

        //    }

        //    return new ObjectResult(new { status = "fail" });



        //}


        //public IActionResult Index()
        //{
        //    return View("Create");
        //}
    }
}
