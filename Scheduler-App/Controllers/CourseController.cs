using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNet.Identity;
using Scheduler_App.Models;
using Scheduler_App.Models.Domain;
using Scheduler_App.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            var model = DbContext
                .CourseDatabase
                .ProjectTo<CourseViewModel>()
                .ToList();
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

            var allInstructor = DbContext.InstructorDatabase
                .Select(p => new SelectListItem()
                {
                    Text = p.FirstName,
                    Value = p.Id.ToString(),
                }).ToList();
            var model = new CreateEditCourseViewModel();
            model.ProgramList = allProgram;
            model.InstructorList = allInstructor;
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

            var allInstructor = DbContext.InstructorDatabase
                .Select(p => new SelectListItem()
                {
                    Text = p.FirstName,
                    Value = p.Id.ToString(),
                }).ToList();

            if (!ModelState.IsValid)
            {
                formData.ProgramList = allProgram;
                formData.InstructorList = allInstructor;
                return View(formData);
            }

            if (DbContext.CourseDatabase.Any(p =>
            p.Name == formData.Name &&
            (!id.HasValue || p.Id != id.Value)))
            {
                ModelState.AddModelError(nameof(CreateEditCourseViewModel.Name),
                    "Course Name should be unique");
                formData.InstructorList = allInstructor;
                formData.ProgramList = allProgram;
                return View(formData);
            }

            var course = Mapper.Map<Course>(formData);

            if (!id.HasValue)
            {
                course.Program.Courses.Add(course);
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
            course.Program = DbContext.ProgramDatabase.FirstOrDefault(p => p.Id == formData.ProgramId);
            course.Instructor = DbContext.InstructorDatabase.FirstOrDefault(p => p.Id == formData.InstructorsId);
            course.ProgramName = DbContext.ProgramDatabase.FirstOrDefault(p => p.Id == formData.ProgramId).Name;
            course.InstructorName = DbContext.InstructorDatabase.FirstOrDefault(p => p.Id == formData.InstructorsId).FirstName;

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

        //GET: Details of the Course
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
            allCourses.InstructorName = course.InstructorName;
            allCourses.ProgramName = course.ProgramName;
            return View(allCourses);
        }

        // Delete Method for course
        // GET:
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = DbContext.CourseDatabase.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]

        public ActionResult DeleteConfirmed(int id)
        {
            Course course = DbContext.CourseDatabase.Find(id);
            DbContext.CourseDatabase.Remove(course);
            DbContext.SaveChanges();
            return RedirectToAction(nameof(CourseController.Index));
        }
    }
}