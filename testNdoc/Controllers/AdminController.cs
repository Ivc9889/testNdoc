using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace testNdoc.Areas.AdminControllers
{
    [Authorize]
    public class AdminController : Controller
    {

        NDocContext db;
        private readonly IWebHostEnvironment webHostEnvironment;

        // GET: AdminController

        List<Documents> doc;
        List<Section> list;
        public AdminController(IWebHostEnvironment webHostEnvironment)
        {

            db = new NDocContext();
            this.webHostEnvironment = webHostEnvironment;



            doc = new List<Documents>
            {
                new Documents{Id=1,Name="фдлвдф"}
            };
            list = new List<Section>
            {
                new Section{Id=1,Name="фдлвдф"}
            };


        }



        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                Section delete = await db.Sections.FirstOrDefaultAsync(p => p.Id == id);
                if (delete != null)
                    return View(delete);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                Section delete = await db.Sections.FirstOrDefaultAsync(p => p.Id == id);
                if (delete != null)
                {
                    delete.IsRemove = true;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                Section name = await db.Sections.FirstOrDefaultAsync(p => p.Id == id);
                if (name != null)
                    return View(name);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Section user)
        {
            db.Sections.Update(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }



        public IActionResult OnPostMyUploader()
        {
            return View();
        }

        [HttpPost]
        [RequestSizeLimit(808 * 1024 * 1024)]       //unit is bytes => 500Mb
        [RequestFormLimits(MultipartBodyLengthLimit = 808 * 1024 * 1024)]
        public IActionResult OnPostMyUploader(IFormFile MyUploader, string Name, int SectionId)
        {
            if (MyUploader != null)
            {
                try
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(MyUploader.FileName);
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Files");
                    string filePath = Path.Combine(uploadsFolder, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        MyUploader.CopyTo(fileStream);
                    }
                    Documents document = new Documents() { Name = Name, SectionId = SectionId, FileName = fileName, DateAdd = DateTime.Now };
                    db.Add(document);
                    db.SaveChanges();
                    return new ObjectResult(new { status = "success" });
                }
                catch (Exception)
                {
                    return new ObjectResult(new { status = "fail" });
                }

            }

            return new ObjectResult(new { status = "fail" });



        }

        


        public IActionResult TableDocument(int id)
        {
            ViewBag.SectionName = db.Sections.Find(id).Name;
            var model = db.Documents.Where(x => x.IsRemove != true).Where(d => d.SectionId == id);
            return PartialView(model);
        }


        public IActionResult Table_site()
        {
            return View("Table_Site");
        }


        //public IActionResult FileAdd()
        //{
        //    return View("Create");
        //}


        //public async Task<IActionResult> Index()
        //{
        //    return View(await db.Sections.ToListAsync());
        //}
        //public IActionResult Create()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public async Task<IActionResult> Create(Section user)
        //{
        //    db.Sections.Add(user);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Create");
        //}



        //public ActionResult Index()
        //{
        //    //return View(db.Sections?.OrderBy(x => x.Name));

        //}



        //[HttpPost]
        //public async Task<IActionResult> AddFile(IFormFile[] filesupload)
        //{
        //    if (filesupload == null || filesupload.Length == 0)
        //    {
        //        ViewData["Message"] = "Выберите как минимум один файл";

        //    }
        //    else
        //    {
        //        foreach (IFormFile img in filesupload)
        //        {
        //            var saveimg = Path.Combine(_web.WebRootPath, "Files",
        //                img.FileName);
        //            var fileselected = new FileStream(saveimg, FileMode.Create);
        //            await img.CopyToAsync(fileselected);
        //            ViewData["Message"] = "Выбранные файлы сохранены" + filesupload.Length + "Сохранено";
        //        }
        //    }

        //    return View();
        //}


        [HttpGet]
        public ActionResult CreateDocuments(int sectionId)
        {
            Documents model = new Documents() { SectionId = sectionId };
            return PartialView();
        }

        [HttpGet]
        [ActionName("DeleteDocument")]
        public async Task<IActionResult> ConfirmDeleteDocument(int? id)
        {
            if (id != null)
            {
                Documents delete = await db.Documents.FirstOrDefaultAsync(p => p.Id == id);
                if (delete != null)
                    return View(delete);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDocument(int? id)
        {
            if (id != null)
            {
                Documents delete = await db.Documents.FirstOrDefaultAsync(p => p.Id == id);
                if (delete != null)
                {
                    delete.IsRemove = true;

                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Files");
                    FileInfo fileInf = new FileInfo(Path.Combine(uploadsFolder, delete.FileName));
                    if (fileInf.Exists)
                    {
                        fileInf.Delete();
                    }

                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return NotFound();
        }



        [HttpGet]
        public async Task<IActionResult> EditDocument(int? id)
        {
            if (id != null)
            {
                Documents name = await db.Documents.FirstOrDefaultAsync(p => p.Id == id);
                if (name != null)
                    return View(name);
            }
            return NotFound();
        }

        [HttpPost]
        [RequestSizeLimit(808 * 1024 * 1024)]       //unit is bytes => 500Mb
        [RequestFormLimits(MultipartBodyLengthLimit = 808 * 1024 * 1024)]
        public IActionResult EditDocument(IFormFile MyUploader, string Name, int Id)
        {
            Documents updateDoc = db.Documents.Find(Id);
            if (MyUploader != null)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(MyUploader.FileName);
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Files");
                string filePath = Path.Combine(uploadsFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    MyUploader.CopyTo(fileStream);
                }

                FileInfo fileInf = new FileInfo(Path.Combine(uploadsFolder, updateDoc.FileName));
                if (fileInf.Exists)
                {
                    fileInf.Delete();
                }
                updateDoc.FileName = fileName;
            }
            updateDoc.Name = Name;
            db.SaveChanges();
            return RedirectToAction("Сreate", "Admin");
        }



        public async Task<IActionResult> DocIndex()
        {
            return View(await db.Documents.ToListAsync());
        }

        public ActionResult CreateDocument(int sectionId)
        {
            return View(new Documents() { SectionId = sectionId });
        }

        [HttpPost]
        public ActionResult CreateDocument(Documents document)
        {
            //document.DateAdd = DateTime.Now - Нужно менять в методе добав. файла;
            db.Documents.Add(document);
            db.SaveChanges();
            return RedirectToAction("Create");


        }

        [HttpGet]
        public ActionResult TableDocumented()
        {
            var model = db.Documents.OrderBy(x => x.Name);
            return PartialView("TableDocument", model);

        }

        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminController/Create


        public async Task<ActionResult> Index()
        {
            return View(await db.Sections.Where(x => x.IsRemove != true).ToListAsync());

        }

        public IActionResult Create()
        {
            return View("Create");
        }




        [HttpPost]
        public ActionResult Create(Section section)
        {
            db.Sections.Add(section);
            db.SaveChanges();

            return RedirectToAction("Create");
        }

        [HttpGet]
        public ActionResult TableSection()
        {
            var model = db.Sections.Where(x => x.IsRemove != true).OrderBy(x => x.Name);
            return PartialView("_TableSection", model);

            //var model = db.Sections.OrderBy(x => x.Name).Where(x => x.IsRemove != true);
            //return PartialView("_TableSection", model);

        }






    }
}
