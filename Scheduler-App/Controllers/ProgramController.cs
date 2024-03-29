﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentDateTime;
using Scheduler_App.Models;
using Scheduler_App.Models.Domain;
using Scheduler_App.Models.Enum;
using Scheduler_App.Models.ViewModel;
using System;
using System.Linq;
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

        public ActionResult Index()
        {
            var model = DbContext
                .ProgramDatabase
                .ProjectTo<CreateEditSchoolProgramViewModel>()
                .ToList();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = nameof(UserRoles.Admin))]
        public ActionResult CreateProgram()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRoles.Admin))]
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

            program.Name = formData.Name;
            program.StartDate = formData.StartDate;
            var firstCourse = program.Courses.First();
            var startDate = firstCourse.StartDate;
            firstCourse.StartDate = formData.StartDate;
            var lastCourse = program.Courses.Last();
            if(lastCourse != null)
            {
                foreach(var course in program.Courses)
                {
                    if (firstCourse == null)
                    {
                        course.StartDate = course.Program.StartDate;
                        var totalDays = Convert.ToInt32(course.Hours / course.DailyHours);
                        course.EndDate = course.StartDate.AddBusinessDays(totalDays - 1);             
                        var startTime = course.StartTime = course.StartDate.TimeOfDay;
                        var remainingHours = course.Hours - (course.DailyHours * (totalDays));
                        var hours = remainingHours + startTime.Hours;
                        var remainingTime = TimeSpan.FromHours(hours);
                        remainingTime = remainingTime.Add(TimeSpan.FromMinutes(startTime.Minutes));
                        course.EndTime = remainingTime;
                    }
                    else
                    {
                        var totalDays = Convert.ToInt32(course.Hours / course.DailyHours);
                        course.EndDate = lastCourse.EndDate.AddBusinessDays(totalDays - 1);
                        course.StartDate = Convert.ToDateTime(lastCourse.EndDate);
                        var startTime = course.StartTime = course.StartDate.TimeOfDay;
                        var remainingHours = course.Hours - (course.DailyHours * (totalDays));
                        var hours = remainingHours + startTime.Hours;
                        var remainingTime = TimeSpan.FromHours(hours);
                        remainingTime = remainingTime.Add(TimeSpan.FromMinutes(startTime.Minutes));
                        course.EndTime = remainingTime;
                    }
                }
            }

            DbContext.SaveChanges();

            return RedirectToAction("Details", new { id = program.Id });
        }

        [HttpGet]
        [Authorize(Roles = nameof(UserRoles.Admin))]
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

        [HttpPost]
        [Authorize(Roles = nameof(UserRoles.Admin))]
        public ActionResult EditProgram(int id, CreateEditSchoolProgramViewModel formData)
        {
            return SaveProgram(id, formData);
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(ProgramController.Index));
            }
            var program = DbContext.ProgramDatabase.Find(id);
            if (program == null)
            {
                return RedirectToAction(nameof(ProgramController.Index));
            }
            return View(program);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var program = DbContext.ProgramDatabase.Find(id);
            var course = DbContext.CourseDatabase.Where(p => p.ProgramId == id).ToList();
            foreach (var singleCourse in course)
            {
                DbContext.CourseDatabase.Remove(singleCourse);
            }

            DbContext.ProgramDatabase.Remove(program);
            DbContext.SaveChanges();

            TempData["Message"] = "You Successfully deleted the Program.";

            return RedirectToAction(nameof(ProgramController.Index));
        }

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
                if (course != null)
                {
                    var program = DbContext.ProgramDatabase.FirstOrDefault(p => p.Id == programId);
                    var firstCourse = program.Courses.ElementAtOrDefault(0);
                    if (firstCourse == null)
                    {
                        course.StartDate = program.StartDate;
                        var totalDays = Convert.ToInt32(course.Hours / course.DailyHours);
                        course.EndDate = course.StartDate.AddBusinessDays(totalDays - 1);
                        var startTime = course.StartTime = course.StartDate.TimeOfDay;
                        var remainingHours = course.Hours - (course.DailyHours * (totalDays));
                        var hours = remainingHours + startTime.Hours;
                        var remainingTime = TimeSpan.FromHours(hours);
                        remainingTime = remainingTime.Add(TimeSpan.FromMinutes(startTime.Minutes));
                        course.EndTime = remainingTime;                     
                    }
                    else
                    {
                        var lastCourse = program.Courses.Last();
                        var totalDays = Convert.ToInt32(course.Hours / course.DailyHours);
                        course.EndDate = lastCourse.EndDate.AddBusinessDays(totalDays - 1);
                        course.StartDate = Convert.ToDateTime(lastCourse.EndDate);
                        var startTime = course.StartTime = course.StartDate.TimeOfDay;
                        var remainingHours = course.Hours - (course.DailyHours * (totalDays - 1));
                        var hours = remainingHours + startTime.Hours;
                        var remainingTime = TimeSpan.FromHours(hours);
                        remainingTime = remainingTime.Add(TimeSpan.FromMinutes(startTime.Minutes));
                        course.EndTime = remainingTime;
                    }
                }

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
        public ActionResult DeleteProgramCourse(int id, int Courseid)
        {
            Program program = DbContext.ProgramDatabase.Find(id);
            var course = DbContext.CourseDatabase.FirstOrDefault(p => p.Id == Courseid);
            var courseId = course.Id;
            Courseid = courseId;

            DbContext.CourseDatabase.Remove(course);
            DbContext.SaveChanges();

            return RedirectToAction(nameof(ProgramController.Details), new { id = program.Id });
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction(nameof(ProgramController.Index));

            var program = DbContext.ProgramDatabase.FirstOrDefault(p => p.Id == id.Value);
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