using SimpleExcelImport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using MyAspNetMvcApp.Areas.Examples.ViewModels;

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
                    //var data = System.IO.File.ReadAllBytes(@"C:\Users\Drive_D\My Documents\COC LESSONS\ADV WEB-1\__VSProjects\excel import\Students.xlsx");
                    var data = ExcelFile.ToFileByteArray();
                    ImportFromExcel import = new ImportFromExcel();
                    import.LoadXlsx(data);
                    output = import.ExcelToList<StudentExcelViewModel>(0, 1);

                    foreach (var stud in output)
                    {
                        Char[] separators = new Char[] { ',' }; // only the space character, in this case
                        var names = stud.LastName.Split(separators);
                        stud.LastName = names[0];
                        stud.FirstName = names[1];
                    }
                }
            }
            catch
            {
            }

            return View(output);
        }

        public ActionResult BatchSubmit(string[] IdNumber, string[] LastName, string[] FirstName, string[] YearSection)
        {
            var students = new List<StudentExcelViewModel>();
            for (int i = 0; i < IdNumber.Length; i++)
            {
                students.Add(new StudentExcelViewModel()
                {
                    IdNumber = IdNumber[i],
                    LastName = LastName[i],
                    FirstName = FirstName[i],
                    YearSection = YearSection[i]
                });
            }

            return View(students);
        }
    }
}