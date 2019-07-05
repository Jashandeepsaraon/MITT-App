using Microsoft.AspNet.Identity;
using Scheduler_App.Models;
using Scheduler_App.Models.Domain;
using Scheduler_App.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Scheduler_App.Controllers
{
    public class CourseController : Controller
    {
        private ApplicationDbContext DbContext;
        public CourseController()
        {
            DbContext = new ApplicationDbContext();
        }

        // GET: Course
        public ActionResult Index()
        {
            var model = DbContext.CourseDatabase
                .Select(p => new CreateEditCourseViewModel
                {
                     Id = p.Id,
                    Name = p.Name,
                    Hours = p.Hours,
                }).ToList();
            return View(model);
        }

        //GET : CreateCourse
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateCourse()
        {
            var allProgram = DbContext.ProgramDatabase
               .Select(p => new SelectListItem()
               {
                   Text = p.Name,
                   Value = p.Id.ToString(),
               }).ToList();

            var allInstroctor = DbContext.InstructorDatabase
                .Select(p => new SelectListItem()
                {
                    Text = p.FirstName,
                    Value = p.Id.ToString(),
                }).ToList();
            var model = new CreateEditCourseViewModel();
            model.ProgramList = allProgram;
            model.InstructorList = allInstroctor;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateCourse(CreateEditCourseViewModel formData)
        {
            return SaveCourse(null, formData);
        }

        private ActionResult SaveCourse(int? id, CreateEditCourseViewModel formData)
        {
            var allProgram = DbContext.ProgramDatabase
                .Select(p => new SelectListItem()
                {
                    Text = p.Name,
                    Value = p.Id.ToString(),
                }).ToList();

            var allInstroctor = DbContext.InstructorDatabase
                .Select(p => new SelectListItem()
                {
                    Text = p.FirstName,
                    Value = p.Id.ToString(),
                }).ToList();

            if (!ModelState.IsValid)
            {
                formData.ProgramList = allProgram;
                formData.InstructorList = allInstroctor;
                return View(formData);
            }

           Course course;

            if (!id.HasValue)
            {
                course = new Course();
                DbContext.CourseDatabase.Add(course);
                DbContext.SaveChanges();
            }
            else
            {
                course = DbContext.CourseDatabase.FirstOrDefault(p => p.Id == id);
                if (course == null)
                {
                    return RedirectToAction(nameof(CourseController.Index));
                }
            }
            
            course.Name = formData.Name;
            course.Hours = formData.Hours;
            course.SchoolProgram = DbContext.ProgramDatabase.FirstOrDefault(p => p.Name == formData.Name);
            course.Instructor = DbContext.InstructorDatabase.FirstOrDefault(p => p.FirstName == formData.Name);

            DbContext.SaveChanges();
            return RedirectToAction(nameof(CourseController.Index));
        }

        //GET: EditCourse
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditCourse(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(CourseController.Index));
            }
            var course = DbContext.CourseDatabase.FirstOrDefault(p => p.Id == id);

            if (course == null)
            {
                return RedirectToAction(nameof(CourseController.Index));
            }

            var allProgram = DbContext.ProgramDatabase
                .Select(p => new SelectListItem()
                {
                    Text = p.Name,
                    Value = p.Id.ToString(),
                }).ToList();

            var allInstroctor = DbContext.InstructorDatabase
                .Select(p => new SelectListItem()
                {
                    Text = p.FirstName,
                    Value = p.Id.ToString(),
                }).ToList();
            var model = new CreateEditCourseViewModel();
            model.Name = course.Name;
            model.Hours = course.Hours;
            model.ProgramList = allProgram;
            model.InstructorList = allInstroctor;
            return View(model);
        }

        //POST:
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult EditCourse(int id, CreateEditCourseViewModel formData)
        {
            return SaveCourse(id, formData);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction(nameof(CourseController.Index));

            var userId = User.Identity.GetUserId();

            var course = DbContext.CourseDatabase.FirstOrDefault(p =>
            p.Id == id.Value);

            if (course == null)
                return RedirectToAction(nameof(CourseController.Index));

            var allCourses = new CourseViewModel();
            allCourses.Name = course.Name;
            allCourses.Hours = course.Hours;
            return View(allCourses);
        }
    }
}