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
    public class InstructorController : Controller
    {
        private ApplicationDbContext DbContext;
        public InstructorController()

        {
            DbContext = new ApplicationDbContext();
        }

        //Method to get list of Instructors
        public ActionResult Index()
        {
            var model = DbContext
                .InstructorDatabase
                .ProjectTo<InstructorViewModel>()
                .ToList();
            return View(model);
        }

        //GET : Create Instructor
        [HttpGet]
        ////[Authorize(Roles = "Admin")]
        public ActionResult CreateInstructor()
        {
            return View();
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult CreateInstructor(InstructorViewModel formData)
        {

            return SaveInstructor(null, formData);
        }

        private ActionResult SaveInstructor(int? id, InstructorViewModel formData)

        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = new ApplicationUser { UserName = formData.Email, Email = formData.Email };
            var result = userManager.CreateAsync(user, formData.Password);
            var userId = user.Id;

            var instructor = Mapper.Map<Instructor>(formData);

            //var Instructor = formData.Instructor;
            if (!id.HasValue)
            {
                    DbContext.Users.Add(user);
                    DbContext.InstructorDatabase.Add(instructor);
                    DbContext.SaveChanges();
                //if (!userManager.IsInRole(user.Id, "Instructor"))
                //{
                //    userManager.AddToRole(user.Id, "Instructor");
                //}
                string code = userManager.GenerateEmailConfirmationToken(user.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                userManager.SendEmail(userId, "Notification",
                     "You are registered as an Instructor. Your Current Password is 'Password-1'. Please change your password by clicking <a href=\"" + callbackUrl + "\">here</a>");

                return RedirectToAction("Index");
            }

            else
            {
                instructor = DbContext.InstructorDatabase.FirstOrDefault(p => p.Id == id);
                if (instructor == null)
                {
                    return RedirectToAction(nameof(InstructorController.Index));
                }
            }
            instructor.FirstName = formData.FirstName;
            instructor.LastName = formData.LastName;
            instructor.Email = formData.Email;
            instructor.Courses.Find(p => p.Id == formData.CourseId);
            DbContext.SaveChanges();
            return RedirectToAction(nameof(InstructorController.Detail), new { id = instructor.Id });
        }

        //GET: EditProgram
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult EditInstructor(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(InstructorController.Index));
            }
            var instructor = DbContext.InstructorDatabase.FirstOrDefault(p => p.Id == id);

            if (instructor == null)
            {
                return RedirectToAction(nameof(InstructorController.Index));
            }

            var model = new InstructorViewModel();
            model.FirstName = instructor.FirstName;
            model.LastName = instructor.LastName;
            model.Email = instructor.Email;
            return View(model);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public ActionResult EditInstructor(int id, InstructorViewModel formData)
        {
            return SaveInstructor(id, formData);
        }

        // GET:
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(InstructorController.Index));
            }
            Instructor instructor = DbContext.InstructorDatabase.Find(id);
            if (instructor == null)
            {
                return RedirectToAction(nameof(InstructorController.Index));
            }
            return View(instructor);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]

        public ActionResult DeleteConfirmed(int id)
        {
            Instructor instructor = DbContext.InstructorDatabase.Find(id);
            var courses = DbContext.CourseDatabase.FirstOrDefault(p => p.InstructorId == instructor.Id);
             if (instructor.Courses == null /*||courses.Instructor != null*/) 
            {
                courses.Instructor = null;
            }
            instructor.Courses = null;
            DbContext.InstructorDatabase.Remove(instructor);
            DbContext.SaveChanges();
            TempData["Message"] = "You Successfully deleted the Instructor.";
            return RedirectToAction(nameof(InstructorController.Index));
        }

        [HttpGet]
        public ActionResult ImportInstructor()
        {
            return View(new List<InstructorViewModel>());
        }

        [HttpGet]
        public ActionResult Detail(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction(nameof(InstructorController.Index));
            var instructor = DbContext.InstructorDatabase.FirstOrDefault(p => p.Id == id);

            if (instructor == null)
                return RedirectToAction(nameof(InstructorController.Index));

            var allInstructor = new InstructorViewModel();
            allInstructor.FirstName = instructor.FirstName;
            allInstructor.LastName = instructor.LastName;
            allInstructor.Email = instructor.Email;
            allInstructor.Courses = instructor.Courses;
            ViewBag.id = id;
            return View(allInstructor);
        }


        [HttpPost]
        public ActionResult ImportInstructor(HttpPostedFileBase postedFile)
        {

            List<Instructor> instructor = new List<Instructor>();
            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);

                //Read the contents of CSV file.
                string csvData = System.IO.File.ReadAllText(filePath);

                //Execute a loop over the rows.
                foreach (string row in csvData.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        var instructors = (new Instructor
                        {
                            //Id = Convert.ToInt32(row.Split(',')[0]),
                            FirstName = row.Split(',')[0],
                            LastName = row.Split(',')[1],
                            Email = row.Split(',')[2]
                        });
                        var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                        var user = new ApplicationUser { UserName = instructors.Email, Email = instructors.Email };
                        var result = userManager.CreateAsync(user, instructors.Password);
                        var userId = user.Id;
                        DbContext.Users.Add(user);
                        //instructor.Add(instructors);
                        DbContext.InstructorDatabase.Add(instructors);
                        DbContext.SaveChanges();
                    }
                }
            }
            return RedirectToAction("Index");
        }

        //Method to get the Instructors List 
        [HttpGet]
        public ActionResult AssignInstructor(int id)
        {
            var course = DbContext.CourseDatabase.FirstOrDefault(p => p.Id == id);
            var instructorList = DbContext.InstructorDatabase
              .Select(p => new SelectListItem()
              {
                  Text = p.FirstName + " " + p.LastName,
                  Value = p.Id.ToString(),
              }).ToList();
            if (instructorList == null)
            {
                ModelState.AddModelError("", "Instructor is not found.");
                return View("Error");
                //return RedirectToAction(nameof(InstructorController.Detail));
            }
            var model = new AssignInstructorViewModel();
            model.InstructorList = instructorList;
            model.CourseId = id;
            return View(model);
        }

        // Method for the Assign Instrutor to the Course
        [HttpPost]
        public ActionResult AssignInstructor(AssignInstructorViewModel model)
        {
            var course = DbContext.CourseDatabase.FirstOrDefault(p => p.Id == model.CourseId);
            if (course == null)
            {
                return RedirectToAction(nameof(CourseController.Details));
            }

            if (model.AddSelectedInstructor != null)
            {
                var assignInstructor = DbContext.InstructorDatabase.FirstOrDefault(p => p.Id.ToString() == model.AddSelectedInstructor);
                //instructorId = model.InstructorId;
                var courseId = assignInstructor.Courses.FirstOrDefault(p => p.Id == course.Id);
                //courseId = model.CourseId.;
                course.Instructor = assignInstructor;
                assignInstructor.Courses.Add(course);
                DbContext.SaveChanges();
            }
            return RedirectToAction("Details", "Course", new { id = model.CourseId });

        }

        // Method for the Remove Instrutor to the Course
        [HttpPost]
        public ActionResult RemoveInstructor(int? courseId)
        {
            var course = DbContext.CourseDatabase.FirstOrDefault(p => p.Id == courseId);
            if (!courseId.HasValue)
            {
                return RedirectToAction(nameof(CourseController.Details));
            }
            var instructor = course.Instructor;
            if (instructor != null)
            {
                course.Instructor = null;
                DbContext.SaveChanges();
            }
            return RedirectToAction("Details", "Course", new { id = courseId });
        }
    }
}
