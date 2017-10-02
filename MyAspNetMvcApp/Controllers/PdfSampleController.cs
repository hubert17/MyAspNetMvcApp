using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyAspNetMvcApp.Models;

namespace MyAspNetMvcApp.Controllers
{
    public class PdfSampleController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: PdfSample
        public ActionResult Index()
        {
            var books = db.Books.ToList();

            return View(books);
        }

        public ActionResult Print()
        {
            var books = db.Books.ToList();
            return new Rotativa.ViewAsPdf(books);
        }

        public ActionResult Send()
        {
            var books = db.Books.ToList();            
            var pdfViewResult = new Rotativa.ViewAsPdf("Print", books)
            {
                FileName = "books.pdf"
            };

            var pdfAttachment = new Gabs.Helpers.EmailUtil.EmailAttachment()
            {
                Data = pdfViewResult.BuildPdf(this.ControllerContext),
                ContentType = "application/pdf",
                FileName = pdfViewResult.FileName
            };

            bool result = Gabs.Helpers.EmailUtil.SendEmail("hewbertgabon@gmail.com", "Sample PDF Email", "Hello! Attach is the pdf file.", pdfAttachment);
            if (result)
                TempData["MessageBox"] = "PDF Email successfully sent.";
            else
                TempData["MessageBox"] = "Sending failed.";

            return RedirectToAction("Index");
        }
    }
}