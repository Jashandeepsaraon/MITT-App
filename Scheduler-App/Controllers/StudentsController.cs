using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Scheduler_App.Models;
using Scheduler_App.Models.Domain;
using Scheduler_App.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Scheduler_App.Controllers
{
    public class StudentsController : Controller
    {
        private ApplicationDbContext DbContext;
        public StudentsController()

        {
            DbContext = new ApplicationDbContext();
        }

        //List of Students
        public ActionResult Index()
        {
            var model = DbContext
                .StudentDatabase
                .ProjectTo<StudentViewModel>()
                .ToList();
            return View(model);
        }

        //GET : CreateStudent
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateStudent()
        {
            //var program = DbContext.ProgramDatabase
            //   .Select(p => new SelectListItem()
            //   {
            //       Text = p.Name,
            //       Value = p.Id.ToString(),
            //   }).ToList();
            //ViewBag.program = program;
            //var course = DbContext.CourseDatabase.Where(p => p.ProgramId == 1).Select(c => new 
            //SelectListItem
            //{
            //    Text = c.Name,
            //    Value = c.Id.ToString(),
            //}).ToList();

            //ViewBag.course = course;
            //var model = new CreateEditStudentViewModel();
            //model.ProgramList = program;
            //model.CourseList = course;
            //;
            return View();
        }

        //public JsonResult GetCourses(int ProgramId, CreateEditStudentViewModel model)
        //{
        //    var courseList = DbContext.CourseDatabase.Where(c => c.ProgramId == ProgramId).Select(c => new
        //    {
        //        Name = c.Name,
        //        Id = c.Id,
        //    }).ToList();
            
        //    //ViewBag.c = new MultiSelectList(courseList,"Id", "Name");
        //    return Json(courseList, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateStudent(CreateEditStudentViewModel formData)
        {
            return SaveStudent(null, formData);
        }

        private ActionResult SaveStudent(int? id, CreateEditStudentViewModel formData)
        {
            //var program = DbContext.ProgramDatabase
            //   .Select(p => new SelectListItem()
            //   {
            //       Text = p.Name,
            //       Value = p.Id.ToString(),
            //   }).ToList();

            if (!ModelState.IsValid)
            {
                //formData.ProgramList = program;
                return View(formData);
            }
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = new ApplicationUser { UserName = formData.Email, Email = formData.Email };
            var result = userManager.CreateAsync(user, formData.Password);
            var userId = user.Id;
            var student = Mapper.Map<Student>(formData);

            if (!id.HasValue)
            {
                //student.ProgramName = DbContext.ProgramDatabase.FirstOrDefault(p => p.Id == formData.ProgramId).Name;
                //student.Program = DbContext.ProgramDatabase.FirstOrDefault(p => p.Id == formData.ProgramId);
                //student.CourseName = student.Program.Courses.FirstOrDefault(p => p.Id == formData.CourseId).Name;
                
                DbContext.Users.Add(user);
                DbContext.StudentDatabase.Add(student);
                DbContext.SaveChanges(); 
                //String code = userManager.GenerateEmailConfirmationToken(user.Id);
                //var callbackUrl = Url.Action("Changepassword", "Manage", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //userManager.SendEmail(userId, "Notification",
                //    "Hello, You are registered as student at MITT.Your current Password is Password-1.Please change your password by clicking <a href=\"" + callbackUrl + "\"> here</a>");
            }
            else
            {
                student = DbContext.StudentDatabase.FirstOrDefault(p => p.Id == id);
                if (student == null)
                {
                    return RedirectToAction(nameof(StudentsController.Index));
                }
            }
            student.FirstName = formData.FirstName;
            student.LastName = formData.LastName;
            student.Email = formData.Email;           
            DbContext.SaveChanges();
            return RedirectToAction(nameof(StudentsController.Index));
        }

        //GET: Edit
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditStudent(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(StudentsController.Index));
            }
            var student = DbContext.StudentDatabase.FirstOrDefault(p => p.Id == id);

            if (student == null)
            {
                return RedirectToAction(nameof(StudentsController.Index));
            }

            var model = new CreateEditStudentViewModel();
            model.FirstName = student.FirstName;
            model.LastName = student.LastName;
            model.Email = student.Email;

            return View(model);
        }
        //POST:
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult EditStudent(int id, CreateEditStudentViewModel formData)
        {
            return SaveStudent(id, formData);
        }

        // GET:
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(StudentsController.Index));
            }
            Student student = DbContext.StudentDatabase.Find(id);
            if (student == null)
            {
                return RedirectToAction(nameof(StudentsController.Index));
            }
            return View(student);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        
        public ActionResult DeleteConfirmed(int id)
        {
           Student students = DbContext.StudentDatabase.Find(id);
            DbContext.StudentDatabase.Remove(students);
            DbContext.SaveChanges();
            return RedirectToAction(nameof(StudentsController.Index));
        }

        
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction(nameof(StudentsController.Index));

            var userId = User.Identity.GetUserId();

            var student = DbContext.StudentDatabase.FirstOrDefault(p =>
            p.Id == id.Value);

            if (student == null)
                return RedirectToAction(nameof(StudentsController.Index));

            var allStudent = new StudentViewModel();
            allStudent.FirstName = student.FirstName;
            allStudent.LastName = student.LastName;
            allStudent.Email = student.Email;
            allStudent.Courses = student.Courses;
            //var programName = student.Courses.FirstOrDefault(p => p.Id == id).Program.Name;
            //allStudent.ProgramList.FirstOrDefault(p => p.Selected == programName.Contains(p.Selected.ToString()));
            //allStudent.CourseName = student.Course.Name;
            //var programName = allStudent.Courses.FirstOrDefault(p => p.Id == id).ProgramName;
            ViewBag.id = id;
            return View(allStudent);
        }
        [HttpGet]
        public ActionResult ImportStudent()
        {
            return View(new List<StudentViewModel>());
        }

        //[HttpPost]
        //public ActionResult ImportStudent(HttpPostedFileBase postedFile)
        //{

        //    List<Student> student  = new List<Student>();
        //    string filePath = string.Empty;
        //    if (postedFile != null)
        //    {
        //        string path = Server.MapPath("~/Uploads/");
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }

        //        filePath = path + Path.GetFileName(postedFile.FileName);
        //        string extension = Path.GetExtension(postedFile.FileName);
        //        postedFile.SaveAs(filePath);

        //        //Read the contents of CSV file.
        //        string csvData = System.IO.File.ReadAllText(filePath);

        //        //Execute a loop over the rows.
        //        foreach (string row in csvData.Split('\n'))
        //        {
        //            if (!string.IsNullOrEmpty(row))
        //            {
        //                var students = (new Student
        //                {
        //                    //Id = Convert.ToInt32(row.Split(',')[0]),
        //                    FirstName = row.Split(',')[0],
        //                    LastName = row.Split(',')[1],
        //                    Email = row.Split(',')[2],
        //                    ProgramName = row.Split(',')[3],
        //                });
        //                var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //                var user = new ApplicationUser { UserName = students.Email, Email = students.Email };
        //                var result = userManager.CreateAsync(user, students.Password);
        //                var userId = user.Id;
        //                DbContext.Users.Add(user);
        //                student.Add(students);
        //                DbContext.StudentDatabase.Add(students);
        //                DbContext.SaveChanges();
        //            }
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}

        [HttpGet]
        public ActionResult AssignCourse(int? studentId)
        {
            var allPrograms = DbContext.ProgramDatabase
                .Select(p => new SelectListItem()
                {
                    Text = p.Name,
                    Value = p.Id.ToString(),
                }).ToList();

            ViewBag.allProgram = allPrograms;

            if (!studentId.HasValue)
            {
                return RedirectToAction(nameof(StudentsController.Index));
            }

            var student = DbContext.StudentDatabase.FirstOrDefault(p => p.Id == studentId);
            var model = new AssignCourseToStudentViewModel();
            model.StudentId = student.Id;

            var courseList = DbContext.CourseDatabase.ToList();
            var course = DbContext.CourseDatabase.Where(p => p.ProgramId == 1).Select(k => new SelectListItem()
            {
                Text = k.Name,
                Value = k.Id.ToString(),
            }).ToList();

            model.AddCourses = course;
            model.ProgramList = allPrograms;

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
        public ActionResult AssignCourse(AssignCourseToStudentViewModel model)
        {
            var student = DbContext.StudentDatabase.FirstOrDefault(p => p.Id == model.StudentId);
            if (student == null)
            {
                return RedirectToAction(nameof(StudentsController.Details));
            }
            
            if (model.AddSelectedCourses != null)
            {
                var assigncourse = DbContext.CourseDatabase.FirstOrDefault(p => p.Id.ToString() == model.AddSelectedCourses);
                //var studentId = assigncourse.Students.FirstOrDefault(p => p.Id == model.StudentId).Id;
                //studentId = student.Id;
                assigncourse.Students.Add(student);
                student.Courses.Add(assigncourse);
                //var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                //userManager.SendEmailAsync(assignedUser.Id, "Notification", "You are assigned to a new Ticket.");
                DbContext.SaveChanges();
            }

            return RedirectToAction("Details", new { id = student.Id });
        }

        [HttpPost]
        public ActionResult RemoveCourse(int? id, int? studentId)
        {
            var student = DbContext.StudentDatabase.FirstOrDefault(p => p.Id == studentId);
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(CourseController.Details));
            }
            var course = student.Courses.FirstOrDefault(p => p.Id == id);
            if (course != null)
            {
                var AssignedCourse = student.Courses.Remove(course);
                course.Instructor = null;
                DbContext.SaveChanges();
            }
            return RedirectToAction(nameof(StudentsController.Details), new { id = student.Id });
        }
    }
}


