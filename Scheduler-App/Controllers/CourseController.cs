using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNet.Identity;
using Scheduler_App.Models;
using Scheduler_App.Models.Domain;
using Scheduler_App.Models.ViewModel;
using System;
using System.Linq;
using System.Net;
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

            var model = new CreateEditCourseViewModel();
            model.ProgramList = allProgram;
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

            //var allInstructor = DbContext.InstructorDatabase
            //    .Select(p => new SelectListItem()
            //    {
            //        Text = p.FirstName,
            //        Value = p.Id.ToString(),
            //    }).ToList();

            if (!ModelState.IsValid)
            {
                formData.ProgramList = allProgram;
                //formData.InstructorList = allInstructor;
                //return View(formData);
                return View(formData);
            }

            if (DbContext.CourseDatabase.Any(p =>
            p.Name == formData.Name &&
            (!id.HasValue || p.Id != id.Value)))
            {
                ModelState.AddModelError(nameof(CreateEditCourseViewModel.Name),
                    "Course Name should be unique");
                //formData.InstructorList = allInstructor;
                formData.ProgramList = allProgram;
                return View(formData);
            }

            var course = Mapper.Map<Course>(formData);
            if (course.InstructorId == 0 || course.Instructor == null)
            {
                //course.Program.Courses.Add(course);
                DbContext.CourseDatabase.Add(course);
                DbContext.SaveChanges();
            }
            else
            {
                if (!id.HasValue)
                {
                    //course.Program.Courses.Add(course);
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
                //course.Instructor = DbContext.InstructorDatabase.FirstOrDefault(p => p.Id == formData.InstructorsId);
                course.Program.Name = DbContext.ProgramDatabase.FirstOrDefault(p => p.Id == formData.ProgramId).Name;
                //course.Instructor.FirstName = DbContext.InstructorDatabase.FirstOrDefault(p => p.Id == formData.InstructorsId).FirstName;

                DbContext.SaveChanges();
            }

            return RedirectToAction(nameof(CourseController.Index));
        }

        //GET: EditCourse
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditCourse(int? id, DateTime dateTime)
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

            //var allInstructor = DbContext.InstructorDatabase
            //    .Select(p => new SelectListItem()
            //    {
            //        Text = p.FirstName,
            //        Value = p.Id.ToString(),
            //    }).ToList();
            var model = new CreateEditCourseViewModel();
            model.Name = course.Name;
            model.Hours = course.Hours;
            model.ProgramList = allProgram;
            //model.InstructorList = allInstructor;
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
            //var allInstructor = DbContext.InstructorDatabase
            //    .Select(p => new SelectListItem()
            //    {
            //        Text = p.FirstName,
            //        Value = p.Id.ToString(),
            //    }).ToList();
            if (!id.HasValue)
                return RedirectToAction(nameof(CourseController.Index));

            var userId = User.Identity.GetUserId();

            var course = DbContext.CourseDatabase.FirstOrDefault(p =>
            p.Id == id.Value);

            if (course == null)
                return RedirectToAction(nameof(CourseController.Index));

            var courseDetail = new CourseViewModel();
            courseDetail.Name = course.Name;
            //courseDetail.InstructorName = course.Instructor.FirstName;
            courseDetail.ProgramName = course.Program.Name;
            return View(courseDetail);
        }

        [HttpGet]
        public ActionResult AddInstructor(int? id)
        {
            var viewModel = DbContext.InstructorDatabase
            .Select(user => new InstructorViewModel
            {
                FirstName = user.FirstName,
                Email = user.Email,
                ///// <summary>
                /////     select the roles from the users
                ///// </summary>
                //Roles = (from role in user.Roles
                //         join r in DbContext.Roles
                //         on role.RoleId equals r.Id
                //         select r.Name).ToList(),
                Id = user.Id

            }).ToList();
            return View(viewModel);
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
            var course = DbContext.CourseDatabase.Where(p => p.ProgramId == 1).Select(c => new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            }).ToList();
            //if (instructor.Courses != null)
            //{
            //    var courses = DbContext.CourseDatabase.Where(p => p.ProgramId == 0).Select(c => new SelectListItem()
            //    {
            //        Text = c.Name,
            //        Value = c.Id.ToString(),
            //    }).ToList();
            //    return View(courses);
            //}
            //var developerRoleId = roleManager.Roles.First(role => role.Name == "developer").Id;
            //var developer = DbContext.Users.Where(user => user.Roles.Any(role => role.RoleId == developerRoleId)).ToList();

            model.AddCourses = course;
            model.ProgramList = program;
            return View(model);
        }

        public JsonResult GetCourses(int ProgramId)
        {
            var courseList = DbContext.CourseDatabase.Where(c => c.ProgramId == ProgramId).Select(c => new
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
                return RedirectToAction(nameof(InstructorController.Detail));
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
            return RedirectToAction("CreateCourse");

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
            if (course != null)
            {
                
            var AssignedCourse = instructor.Courses.Remove(course);
                course.Instructor = null;
                DbContext.SaveChanges();
            }
            return RedirectToAction(nameof(CourseController.Details));
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