using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNet.Identity;
using Scheduler_App.Models;
using Scheduler_App.Models.Domain;
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
        //[Authorize(Roles = "Admin")]
        public ActionResult CreateCourse()
        {
            var allProgram = DbContext.ProgramDatabase
               .Select(p => new SelectListItem()
               {
                   Text = p.Name,
                   Value = p.Id.ToString(),
               }).ToList();

            var model = new CreateEditCourseViewModel();
            model.ProgramList = allProgram;
            return View(model);
        }

        //POST : CreateCourse
        [HttpPost]
        //[Authorize(Roles = "Admin")]
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
                //var program = DbContext.ProgramDatabase.FirstOrDefault(p => p.Id == formData.ProgramId);
                //if(course == null)
                // {
                course.StartDate = course.Program.StartDate;
                //}
                //else
                //{
                //    course.StartDate = 
                //}

                DbContext.CourseDatabase.Add(course);
                DbContext.SaveChanges();
            }
            else
            {
                if (!id.HasValue)
                {
                    //course.Program.Courses.Add(course);

                    course.Program = DbContext.ProgramDatabase.FirstOrDefault(p => p.Id == formData.ProgramId);
                    course.Program.Name = DbContext.ProgramDatabase.FirstOrDefault(p => p.Id == formData.ProgramId).Name;
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
            }
            course.Name = formData.Name;
            course.Hours = formData.Hours;
            DbContext.SaveChanges();
            return RedirectToAction(nameof(CourseController.Index));
        }



        //GET: EditCourse
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult EditCourse(int? id)
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
                //return RedirectToAction(nameof(CourseController.Index));
            }

            var allProgram = DbContext.ProgramDatabase
                .Select(p => new SelectListItem()
                {
                    Text = p.Name,
                    Value = p.Id.ToString(),
                }).ToList();

            var model = new CreateEditCourseViewModel();
            model.Name = course.Name;
            model.Hours = course.Hours;
            model.ProgramList = allProgram;
            return View(model);
        }

        //POST: Edit Course
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult EditCourse(int id, CreateEditCourseViewModel formData)
        {
            return SaveCourse(id, formData);
        }

        //GET: Details of the Course
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(CourseController.Index));
            }

            var userId = User.Identity.GetUserId();
            var course = DbContext.CourseDatabase.FirstOrDefault(p =>
            p.Id == id.Value);

            if (course == null)
            {
                ModelState.AddModelError("", "Course is not found.");
                return View("Error");
                //return RedirectToAction(nameof(CourseController.Index));
            }
            var courseDetail = new CourseViewModel();
            courseDetail.Name = course.Name;
            courseDetail.Instructor = course.Instructor;
            courseDetail.StartDate = course.StartDate;
            //courseDetail.EndDate = course.EndDate;
            if (courseDetail.Instructor != null)
            {
                courseDetail.Instructor.FirstName = course.Instructor.FirstName;
                courseDetail.Instructor.LastName = course.Instructor.LastName;
            }
            courseDetail.ProgramName = course.Program.Name;
            ViewBag.id = id;
            return View(courseDetail);
        }

        //Method to get the Courses List and Program List
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
            var course = DbContext.CourseDatabase.Where(p => p.ProgramId == 1).Select(c => new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            }).ToList();
            //if (instructor.Courses != null)
            //{
            //    var courses = DbContext.CourseDatabase.Where(p => p.ProgramId == 0).Select(c => new SelectListItem()
            //    {
            //        Text = c.Name,
            //        Value = c.Id.ToString(),
            //    }).ToList();
            //    return View(courses);
            //}
            //var developerRoleId = roleManager.Roles.First(role => role.Name == "developer").Id;
            //var developer = DbContext.Users.Where(user => user.Roles.Any(role => role.RoleId == developerRoleId)).ToList();
            model.AddCourses = course;
            model.ProgramList = program;
            return View(model);
        }

        //Method to get the Courses List in DropdownList
        public JsonResult GetCourses(int ProgramId)
        {
            var courseList = DbContext.CourseDatabase.Where(c => c.ProgramId == ProgramId).Select(c => new
            {
                Name = c.Name,
                Id = c.Id,
            }).ToList();
            return Json(courseList, JsonRequestBehavior.AllowGet);
        }

        // Method for the Assign Course to the Instructor
        [HttpPost]
        public ActionResult AssignCourse(AssignCourseViewModel model)
        {
            var instructor = DbContext.InstructorDatabase.FirstOrDefault(p => p.Id == model.InstructorId);
            if (instructor == null)
            {
                ModelState.AddModelError("", "Instructor is not found.");
                return View("Error");
                //return RedirectToAction(nameof(InstructorController.Detail));
            }
            if (model.RemoveSelectedCourses != null)
            {
                //var removecourse = DbContext.CourseDatabase.First(course => instructor.Id == course.InstructorId).Instructor.Remove(instructor);
                var instId = instructor.Id.ToString();
                instId = null;
                //ticket.AssignedToId = null;
            }
            if (model.AddSelectedCourses != null)
            {
                var assigncourse = DbContext.CourseDatabase.FirstOrDefault(p => p.Id.ToString() == model.AddSelectedCourses);
                assigncourse.InstructorId = instructor.Id;
                assigncourse.Instructor = instructor;
                instructor.Courses.Add(assigncourse);
                //var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                //userManager.SendEmailAsync(assignedUser.Id, "Notification", "You are assigned to a new Ticket.");
                DbContext.SaveChanges();
            }
            return RedirectToAction("Detail","Instructor", new { id = instructor.Id });
        }

        // Method for the Remove Course to the Instructor
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
                //return RedirectToAction(nameof(CourseController.Index));
            }
            if (course != null)
            {

                var AssignedCourse = instructor.Courses.Remove(course);
                course.Instructor = null;
                DbContext.SaveChanges();
            }
            return RedirectToAction("Detail", "Instructor", new { id = instructor.Id });
        }

        // Delete Method for course
        // GET:
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(CourseController.Index));
            }
            Course course = DbContext.CourseDatabase.Find(id);
            if (course == null)
            {
                ModelState.AddModelError("", "Course is not found.");
                return View("Error");
                //return RedirectToAction(nameof(CourseController.Index));
            }
            return View(course);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = DbContext.CourseDatabase.Find(id);
            course.Instructor = null;
            DbContext.CourseDatabase.Remove(course);
            DbContext.SaveChanges();
            TempData["Message"] = "You Successfully deleted the Course";
            return RedirectToAction(nameof(CourseController.Index));
        }


        public ActionResult Cal()
        {
            return View();
        }

        public JsonResult GetEvents()
        {
            var eve = DbContext.CourseDatabase.ToList();
            var events = eve.Select(p => p.Name).ToList();
            return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}

