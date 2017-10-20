using SimpleExcelImport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using MyAspNetMvcApp.Areas.Examples.ViewModels;
using MyAspNetMvcApp.Areas.Examples.Models;
using System.Globalization;

namespace MyAspNetMvcApp.Areas.Examples.Controllers
{
    public class ImportExcelController : Controller
    {
        // GET: Examples/ImportExcel
        public ActionResult Index(HttpPostedFileBase ExcelFile)
        {
            var output = new List<StudentExcelViewModel>();
            try
            {
                if(ExcelFile != null)
                {
                    var data = ExcelFile.ToFileByteArray();
                    ImportFromExcel import = new ImportFromExcel();
                    if(Path.GetExtension(ExcelFile.FileName.ToLower()) == ".xlsx")
                        import.LoadXlsx(data);
                    else if (Path.GetExtension(ExcelFile.FileName.ToLower()) == ".xls")
                        import.LoadXls(data);
                    else
                    {
                        TempData[BSMessage.TYPE] = BSMessage.MessageType.DANGER;
                        TempData[BSMessage.DIALOGBOX] =  "Invalid Excel worksheet: " + Path.GetExtension(ExcelFile.FileName);
                        return RedirectToAction("Index");
                    }

                    output = import.ExcelToList<StudentExcelViewModel>(0, 1);

                    foreach (var stud in output)
                    {
                        Char[] separators = new Char[] { ',' }; // only the space character, in this case
                        var names = stud.FullName.Split(separators);
                        stud.FullName = names[0];
                        stud.FirstName = names[1];
                    }
                }
            }
            catch
            {
            }

            return View(output);
        }

        [HttpPost]
        public ActionResult BatchSubmit(string[] IdNumber, string[] FullName, string[] FirstName, string[] YearSection)
        {
            var students = new List<Student>();
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            for (int i = 0; i < IdNumber.Length; i++)
            {
                students.Add(new Student()
                {
                    IdNumber = IdNumber[i],
                    LastName = textInfo.ToTitleCase(FullName[i]),
                    FirstName = textInfo.ToTitleCase(FirstName[i]),
                    YearSection = YearSection[i]
                });
            }

            using (var db = new AppDbContext())
            {
                db.Students.AddRange(students);
                db.SaveChanges();
            }

            return View(students);
        }
    }
}