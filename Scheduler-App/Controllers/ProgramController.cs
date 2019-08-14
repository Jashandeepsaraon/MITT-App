using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNet.Identity;
using Scheduler_App.Models;
using Scheduler_App.Models.Domain;
using Scheduler_App.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Scheduler_App.Controllers
{
    public class ProgramController : Controller

    {
        private ApplicationDbContext DbContext;
        public ProgramController()
        {
            DbContext = new ApplicationDbContext();
        }

        //Method to get list of Programs
        public ActionResult Index()
        {
            var model = DbContext
                .ProgramDatabase
                .ProjectTo<CreateEditSchoolProgramViewModel>()
                .ToList();
            return View(model);
        }

        //GET : CreateProgram
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult CreateProgram()
        {
            return View();
        }

        [HttpPost]
       // [Authorize(Roles = "Admin")]
        public ActionResult CreateProgram(CreateEditSchoolProgramViewModel formData)
        {
            return SaveProgram(null, formData);
        }

        private ActionResult SaveProgram(int? id, CreateEditSchoolProgramViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (DbContext.ProgramDatabase.Any(p =>
            p.Name == formData.Name &&
            (!id.HasValue || p.Id != id.Value)))
            {
                ModelState.AddModelError(nameof(CreateEditSchoolProgramViewModel.Name),
                    "Program Name Should Be Unique");
                return View();
            }

            var program = Mapper.Map<Program>(formData);
            if (!id.HasValue)
            {
                var date = formData.StartDate.Date;
                var newDate = program.StartDate.Date;
                program.StartDate = newDate;
                DbContext.ProgramDatabase.Add(program);
                DbContext.SaveChanges();
                return RedirectToAction("Details", new { id = program.Id });
            }

            else
            {
                program = DbContext.ProgramDatabase.FirstOrDefault(p => p.Id == id);
                if (program == null)
                {
                    return RedirectToAction(nameof(ProgramController.Index));
                }
            }
           //var startDate =  program.Courses.Select(p => p.StartDate);
           // var endDate = program.Courses.Select(p => p.EndDate);
           // foreach(var sd in startDate)
           // {

           // }
            program.Name = formData.Name;
            program.StartDate = formData.StartDate;
            //var firstCourse = program.Courses.First();
            //var startDate = firstCourse.StartDate;
            //firstCourse.StartDate = formData.StartDate;
            //var lastCourse = program.Courses.Last();
            //if(lastCourse != null)
            //{
            //    foreach(var course in program.Courses)
            //    {
            //        var c = lastCourse.Hours / lastCourse.DailyHours;
            //        int newC = Convert.ToInt32(c - 1);
            //        int workDays = 0;
            //        lastCourse.EndDate = lastCourse.StartDate.AddDays(newC);
            //        course.StartDate = Convert.ToDateTime(lastCourse.EndDate);
            //        while (lastCourse.StartDate != lastCourse.EndDate)
            //        {
            //            if (lastCourse.StartDate.DayOfWeek != DayOfWeek.Saturday && lastCourse.StartDate.DayOfWeek != DayOfWeek.Sunday)
            //            {
            //                workDays++;
            //            }

            //            lastCourse.StartDate = lastCourse.StartDate.AddDays(1);
            //            //lastCourse.StartDate = lastCourse.StartDate.AddDays(-newC);
            //            //course.StartDate = t;
            //        }
            //        lastCourse.EndDate = lastCourse.StartDate.AddDays(workDays);
            //        lastCourse.StartDate = lastCourse.StartDate.AddDays(-newC);

            //        course.StartDate = Convert.ToDateTime(lastCourse.EndDate);
                //}
            //}         
            DbContext.SaveChanges();
            return RedirectToAction("Details", new { id = program.Id });
        }

        //GET: EditProgram
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditProgram(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(ProgramController.Index));
            }
            var program = DbContext.ProgramDatabase.FirstOrDefault(p => p.Id == id);
            if (program == null)
            {
                return RedirectToAction(nameof(ProgramController.Index));
            }

            var model = new CreateEditSchoolProgramViewModel();
            model.Name = program.Name;
            model.StartDate = program.StartDate;
            return View(model);
        }

        //POST:
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult EditProgram(int id, CreateEditSchoolProgramViewModel formData)
        {
            return SaveProgram(id, formData);
        }
        // GET:
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(ProgramController.Index));
            }
            Program program = DbContext.ProgramDatabase.Find(id);
            if (program == null)
            {
                return RedirectToAction(nameof(ProgramController.Index));
            }
            return View(program);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Program program = DbContext.ProgramDatabase.Find(id);
            //var p = DbContext.ProgramDatabase.ToList();
            var course = DbContext.CourseDatabase.Where(p => p.ProgramId == id ).ToList();
            foreach(var ca in course)
            {
              DbContext.CourseDatabase.Remove(ca);
            }
            DbContext.ProgramDatabase.Remove(program);
            DbContext.SaveChanges();
            TempData["Message"] = "You Successfully deleted the Program.";
            return RedirectToAction(nameof(ProgramController.Index));
        }

        //GET : CreateProgramCourse
        [HttpGet]

        public ActionResult ProgramCourse()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ProgramCourse(int? id, CreateEditCourseViewModel formData, int? programId)
        {
            if (!ModelState.IsValid)
            {
                return View(formData);
            }
            var course = Mapper.Map<Course>(formData);
            if (!id.HasValue)
            {
                DbContext.CourseDatabase.Add(course);
                DbContext.SaveChanges();
            }
            else
            {
                course = DbContext.CourseDatabase.FirstOrDefault(p => p.Id == id);
                if (course == null)
                {
                    return RedirectToAction(nameof(ProgramController.Index));
                }
            }
            course.Name = formData.Name;
            course.Hours = formData.Hours;

            DbContext.SaveChanges();
            return RedirectToAction(nameof(ProgramController.Details), new { id = programId });
        }
        [HttpPost]
        public ActionResult DeleteProgramCourse(int id,int Courseid)
        {
            Program program = DbContext.ProgramDatabase.Find(id);
            var course = DbContext.CourseDatabase.FirstOrDefault(p => p.Id == Courseid);
            var courseId = course.Id;
            Courseid = courseId;
            DbContext.CourseDatabase.Remove(course);
            DbContext.SaveChanges();
            return RedirectToAction(nameof(ProgramController.Details), new { id = program.Id }) ;
        }

        //GET:
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction(nameof(ProgramController.Index));

            var program = DbContext.ProgramDatabase.FirstOrDefault(p =>
            p.Id == id.Value);
            
            if (program == null)
            {
                return RedirectToAction(nameof(ProgramController.Index));
            }
            var model = new SchoolProgramViewModel();
            {
                model.Name = program.Name;
                model.StartDate = program.StartDate;
                model.Courses = program.Courses;
                ViewBag.id = id;
            }
            return View(model);
        }
    }
}
