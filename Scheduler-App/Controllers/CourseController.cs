using AutoMapper;
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
    public class CourseController : Controller
    {
        private ApplicationDbContext DbContext;
        public CourseController()
        {
            DbContext = new ApplicationDbContext();
        }

        [Authorize]
        public ActionResult Index()
        {
            var model = DbContext
                .CourseDatabase
                .ProjectTo<CourseViewModel>()
                .ToList();

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = nameof(UserRoles.Admin) + "," + nameof(UserRoles.Instructor))]
        public ActionResult CreateCourse()
        {
            var allProgram = DbContext.ProgramDatabase
               .Select(p => new SelectListItem()
               {
                   Text = p.Name,
                   Value = p.Id.ToString(),
               }).ToList();

            var course = DbContext.CourseDatabase.ToList();
            if (course == null)
            {
                return RedirectToAction("Index");
            }

            var model = new CreateEditCourseViewModel();
            model.ProgramList = allProgram;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRoles.Admin) + "," + nameof(UserRoles.Instructor))]
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

            if (formData == null)
            {
                ModelState.AddModelError("", "No form data found.");
                formData.ProgramList = allProgram;
                return View(formData);
            }

            if (!ModelState.IsValid)
            {
                formData.ProgramList = allProgram;
                return View(formData);
            }

            if (DbContext.CourseDatabase.Any(p =>
            p.Name == formData.Name &&
            (!id.HasValue || p.Id != id.Value)))
            {
                ModelState.AddModelError(nameof(CreateEditCourseViewModel.Name),
                    "Course Name should be unique");
                formData.ProgramList = allProgram;
                return View(formData);
            }

            var course = Mapper.Map<Course>(formData);
            if (course.InstructorId == 0 && course.InstructorId == null && !id.HasValue)
            {
                DbContext.CourseDatabase.Add(course);
                DbContext.SaveChanges();
            }
            else
            {
                if (!id.HasValue)
                {

                    course.Program = DbContext.ProgramDatabase.FirstOrDefault(p => p.Id == formData.ProgramId);
                    course.Program.Name = DbContext.ProgramDatabase.FirstOrDefault(p => p.Id == formData.ProgramId).Name;
                    if (formData.PrerequisiteForId != null)
                    {
                        course.PrerequisiteForId = DbContext.CourseDatabase.FirstOrDefault(p => p.Id == formData.PrerequisiteForId).Id;
                    }
                    if (formData.PrerequisiteOfId != null)
                    {
                        course.PrerequisiteOfId = DbContext.CourseDatabase.FirstOrDefault(p => p.Id == formData.PrerequisiteOfId).Id;
                    }
                    if (course != null)
                    {
                        var firstCourse = course.Program.Courses.ElementAtOrDefault(0);
                        if (firstCourse == null)
                        {
                            course.StartDate = course.Program.StartDate;
                            var totalDays = Convert.ToInt32(course.Hours / course.DailyHours);
                            course.EndDate = course.StartDate.AddBusinessDays(totalDays - 1);             /*var hours = Convert.ToDouble(course.DailyHours);*/
                            var startTime = course.StartTime = course.StartDate.TimeOfDay;
                            var remainingHours = course.Hours - (course.DailyHours * (totalDays));
                            var hours = remainingHours + startTime.Hours;
                            var remainingTime = TimeSpan.FromHours(hours);
                            remainingTime = remainingTime.Add(TimeSpan.FromMinutes(startTime.Minutes));
                            course.EndTime = remainingTime;
                        }
                        else
                        {
                            var lastCourse = course.Program.Courses.Last();
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
                        DbContext.CourseDatabase.Add(course);
                        DbContext.SaveChanges();
                        return RedirectToAction(nameof(CourseController.Details), new { id = course.Id });
                    }
                }
                else
                {
                    course = DbContext.CourseDatabase.FirstOrDefault(p => p.Id == id);
                    if (course == null)
                    {
                        return RedirectToAction(nameof(CourseController.Index));
                    }
                }
            }
            course.Name = formData.Name;
            course.Hours = formData.Hours;
            course.PrerequisiteForId = formData.PrerequisiteForId;
            if (course.PrerequisiteForId != null)
            {
                course.PrerequisiteForId = DbContext.CourseDatabase.FirstOrDefault(p => p.Id == formData.PrerequisiteForId).Id;
                course.PrerequisiteFor = DbContext.CourseDatabase.FirstOrDefault(p => p.Id == formData.PrerequisiteForId).Name;
            }

            course.PrerequisiteOfId = formData.PrerequisiteOfId;
            if (course.PrerequisiteOfId != null)
            {
                course.PrerequisiteOfId = DbContext.CourseDatabase.FirstOrDefault(p => p.Id == formData.PrerequisiteOfId).Id;
                course.PrerequisiteOf = DbContext.CourseDatabase.FirstOrDefault(p => p.Id == formData.PrerequisiteOfId).Name;
            }

            DbContext.SaveChanges();
            return RedirectToAction(nameof(CourseController.Details), new { id = course.Id });
        }

        [HttpGet]
        [Authorize(Roles = nameof(UserRoles.Admin) + "," + nameof(UserRoles.Instructor))]
        public ActionResult EditCourse(int? id, int programId)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(CourseController.Index));
            }
            var course = DbContext.CourseDatabase.FirstOrDefault(p => p.Id == id);
            if (course == null)
            {
                ModelState.AddModelError("", "Course is not found.");
                return View("Error");
            }

            var program = DbContext.ProgramDatabase.FirstOrDefault(p => p.Id == programId);
            if (program == null)
            {
                ModelState.AddModelError("", "Program not found.");
                return View("Error");
            }

            var allProgram = DbContext.ProgramDatabase
                .Select(p => new SelectListItem()
                {
                    Text = p.Name,
                    Value = p.Id.ToString(),
                }).ToList();

            var prerequisiteFor = program.Courses.Where(p => p.Id != id)
                .Select(p => new SelectListItem()
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList();

            var prerequisiteOf = program.Courses.Where(p => p.Id != id)
                .Select(p => new SelectListItem()
                {
                    Text = p.Name,
                    Value = p.Id.ToString()
                }).ToList();

            var model = new CreateEditCourseViewModel();
            model.ProgramId = course.ProgramId;
            model.Name = course.Name;
            model.Hours = course.Hours;
            model.ProgramList = allProgram;
            model.PrerequisiteFor = prerequisiteFor;
            model.PrerequisiteOf = prerequisiteOf;
            course.ProgramId = programId;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRoles.Admin) + "," + nameof(UserRoles.Instructor))]
        public ActionResult EditCourse(int id, CreateEditCourseViewModel formData)
        {
            return SaveCourse(id, formData);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(CourseController.Index));
            }

            var course = DbContext.CourseDatabase.FirstOrDefault(p =>
            p.Id == id.Value);

            if (course == null)
            {
                ModelState.AddModelError("", "Course is not found.");
                return View("Error");
            }

            var courseDetail = new CourseViewModel();
            courseDetail.Name = course.Name;
            courseDetail.Instructor = course.Instructor;
            courseDetail.StartDate = course.StartDate;
            courseDetail.EndDate = course.EndDate;
            courseDetail.StartTime = course.StartTime;
            courseDetail.EndTime = course.EndTime;
            courseDetail.ProgramName = course.Program.Name;
            if (courseDetail.Instructor != null)
            {
                courseDetail.Instructor.FirstName = course.Instructor.FirstName;
                courseDetail.Instructor.LastName = course.Instructor.LastName;
            }

            if (course.PrerequisiteForId != null)
            {
                courseDetail.PrerequisiteForId = course.PrerequisiteForId;
                courseDetail.PrerequisiteFor = course.PrerequisiteFor;
            }

            if (course.PrerequisiteOfId != null)
            {
                courseDetail.PrerequisiteOfId = course.PrerequisiteOfId;
                courseDetail.PrerequisiteOf = course.PrerequisiteOf;
            }

            ViewBag.id = id;
            return View(courseDetail);
        }

        [HttpGet]
        public ActionResult AssignCourse(int? instructorId)
        {
            var program = DbContext.ProgramDatabase
                .Select(p => new SelectListItem()
                {
                    Text = p.Name,
                    Value = p.Id.ToString(),
                }).ToList();

            ViewBag.program = program;

            if (!instructorId.HasValue)
            {
                return RedirectToAction(nameof(InstructorController.Detail));
            }

            var model = new AssignCourseViewModel();
            var instructor = DbContext.InstructorDatabase.FirstOrDefault(p => p.Id == instructorId);
            model.InstructorId = instructor.Id;
            var courseList = DbContext.CourseDatabase.ToList();
            var course = DbContext.CourseDatabase
                .Where(p => p.ProgramId != 0)
                .Select(c => new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                }).ToList();

            model.AddCourses = course;
            model.ProgramList = program;

            return View(model);
        }

        public JsonResult GetCourses(int ProgramId)
        {
            var courseList = DbContext.CourseDatabase
                .Where(c => c.ProgramId == ProgramId)
                .Select(c => new
                {
                    Name = c.Name,
                    Id = c.Id,
                }).ToList();

            return Json(courseList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AssignCourse(AssignCourseViewModel model)
        {
            var instructor = DbContext.InstructorDatabase.FirstOrDefault(p => p.Id == model.InstructorId);
            if (instructor == null)
            {
                ModelState.AddModelError("", "Instructor is not found.");
                return View("Error");
            }

            if (model.RemoveSelectedCourses != null)
            {
                var instructorId = instructor.Id.ToString();
                instructorId = null;
            }

            if (model.AddSelectedCourses != null)
            {
                var assigncourse = DbContext.CourseDatabase.FirstOrDefault(p => p.Id.ToString() == model.AddSelectedCourses);
                assigncourse.InstructorId = instructor.Id;
                assigncourse.Instructor = instructor;
                instructor.Courses.Add(assigncourse);
                DbContext.SaveChanges();
            }

            return RedirectToAction("Detail", "Instructor", new { id = instructor.Id });
        }

        [HttpPost]
        public ActionResult RemoveCourse(int? id, int? instructorId)
        {
            var instructor = DbContext.InstructorDatabase.FirstOrDefault(p => p.Id == instructorId);
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(CourseController.Details));
            }

            var course = instructor.Courses.FirstOrDefault(p => p.Id == id);
            if (course == null)
            {
                ModelState.AddModelError("", "Course is not found.");
                return View("Error");
            }

            if (course != null)
            {
                var AssignedCourse = instructor.Courses.Remove(course);
                course.Instructor = null;
                DbContext.SaveChanges();
            }

            return RedirectToAction("Detail", "Instructor", new { id = instructor.Id });
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(CourseController.Index));
            }

            var course = DbContext.CourseDatabase.Find(id);
            if (course == null)
            {
                ModelState.AddModelError("", "Course is not found.");
                return View("Error");
            }

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var course = DbContext.CourseDatabase.Find(id);
            course.Instructor = null;
            DbContext.CourseDatabase.Remove(course);
            DbContext.SaveChanges();
            TempData["Message"] = "You Successfully deleted the Course";
            return RedirectToAction(nameof(CourseController.Index));
        }

        public ActionResult Calendar()
        {
            return View();
        }

        public JsonResult GetEvents()
        {
            var evnt = DbContext.CourseDatabase.ToList();
            var startDate = evnt.Select(p => new { p.StartDate }.StartDate.GetDateTimeFormats()[3]);
            var endDate = evnt.Select(p => new { p.EndDate }.EndDate.GetDateTimeFormats()[3]);
            var events = evnt.Select(p => new { p.Name, startDate, endDate, p.StartTime, p.EndTime, p.DailyHours });

            return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}