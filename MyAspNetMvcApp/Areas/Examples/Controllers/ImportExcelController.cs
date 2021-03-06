﻿using SimpleExcelImport;
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

                    var duplicates = output.GroupBy(x => x.IdNumber)
                                .Select(g => new { Value = g.Key, Count = g.Count() })
                                .Where(h => h.Count > 1)
                                .Select(s => s.Value);

                    if(duplicates.Count() > 0)
                    {
                        TempData[BSMessage.TYPE] = BSMessage.MessageType.DANGER;
                        TempData[BSMessage.PANEL] = "The following duplicate record/s has been removed: " + string.Join(", ", duplicates);
                    }

                    output = output.GroupBy(x => x.IdNumber).Select(x => x.First()).ToList();

                    foreach (var stud in output)
                    {
                        Char[] separators = new Char[] { ',' }; // only the space character, in this case
                        var names = stud.FullName.Split(separators);
                        stud.FullName = names[0];
                        separators = new Char[] { ' ' };
                        stud.FirstName = names[1].Split(separators)[1];
                    }
                }
                else
                {
                    using (var db = new ExamplesDbContext())
                    {
                        output = db.Students.Select(s => new StudentExcelViewModel
                        {
                            IdNumber = s.IdNumber,
                            FullName = s.LastName,
                            FirstName = s.FirstName,
                            YearSection = s.YearSection
                        }).ToList();
                        ViewBag.FromDb = true;
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

            try
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                for (int i = 0; i < IdNumber.Length; i++)
                {
                    students.Add(new Student()
                    {
                        IdNumber = IdNumber[i],
                        LastName = textInfo.ToTitleCase(FullName[i].ToLower()),
                        FirstName = textInfo.ToTitleCase(FirstName[i].ToLower()),
                        YearSection = YearSection[i]
                    });
                }

                using (var db = new ExamplesDbContext())
                {
                    db.Students.AddRange(students);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ViewData[BSMessage.TYPE] = BSMessage.MessageType.DANGER;
                ViewData[BSMessage.PANEL] = ex.GetBaseException().Message;
            }

            return View(students);
        }

        public ActionResult Delete(string IdNumber)
        {
            using (var db = new ExamplesDbContext())
            {
                var stud = db.Students.Where(x => x.IdNumber == IdNumber).FirstOrDefault();
                db.Students.Remove(stud);
                db.SaveChanges();
                TempData[BSMessage.DIALOGBOX] = stud.FirstName + " " + stud.LastName + " has been deleted.";
            }
            return RedirectToAction("Index");
        }
    }
}