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
                program.StartDate = DateTime.Now;
                DbContext.ProgramDatabase.Add(program);
                DbContext.SaveChanges();
            }

            else
            {
                program = DbContext.ProgramDatabase.FirstOrDefault(p => p.Id == id);
                if (program == null)
                {
                    return RedirectToAction(nameof(ProgramController.Index));
                }
            }
            program.Name = formData.Name;
            program.StartDate = formData.StartDate;
            DbContext.SaveChanges();
            return RedirectToAction(nameof(ProgramController.Index));
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
            DbContext.ProgramDatabase.Remove(program);
            DbContext.SaveChanges();
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
            return RedirectToAction(nameof(ProgramController.Index));
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
            return RedirectToAction(nameof(ProgramController.Index));
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
