using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Scheduler_App.Models;
using Scheduler_App.Models.Domain;
using Scheduler_App.Models.Enum;
using Scheduler_App.Models.ViewModel;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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

        public ActionResult Index()
        {
            var model = DbContext
                .InstructorDatabase
                .ProjectTo<InstructorViewModel>()
                .ToList();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = nameof(UserRoles.Admin))]
        public ActionResult CreateInstructor()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRoles.Admin))]
        public ActionResult CreateInstructor(InstructorViewModel formData)
        {

            return SaveInstructor(null, formData);
        }

        public ActionResult EmailError()
        {
            return View();
        }

        private ActionResult SaveInstructor(int? id, InstructorViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = new ApplicationUser { UserName = formData.Email, Email = formData.Email };
            var result = userManager.Create(user, formData.Password);
            var userId = user.Id;
            var instructor = Mapper.Map<Instructor>(formData);

            if (!id.HasValue)
            {
                var allInstructors = DbContext.InstructorDatabase.ToList();
                if (allInstructors.Count != 0)
                {
                    if (allInstructors.Any(p => p.Email == instructor.Email))
                    {
                        var instructorEmail = DbContext.InstructorDatabase.FirstOrDefault(p => p.Email == instructor.Email).Email;
                        return RedirectToAction("EmailError");
                    }
                }

                DbContext.InstructorDatabase.Add(instructor);
                DbContext.SaveChanges();

                if (!userManager.IsInRole(user.Id, "Instructor"))
                {
                    userManager.AddToRoleAsync(user.Id, "Instructor");
                }

                string code = userManager.GenerateEmailConfirmationToken(user.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                userManager.SendEmailAsync(userId, "Notification",
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

        [HttpGet]
        [Authorize(Roles = nameof(UserRoles.Admin))]
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
        [Authorize(Roles = nameof(UserRoles.Admin))]
        public ActionResult EditInstructor(int id, InstructorViewModel formData)
        {
            return SaveInstructor(id, formData);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(InstructorController.Index));
            }

            var instructor = DbContext.InstructorDatabase.Find(id);
            if (instructor == null)
            {
                return RedirectToAction(nameof(InstructorController.Index));
            }

            return View(instructor);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var instructor = DbContext.InstructorDatabase.Find(id);
            var courses = DbContext.CourseDatabase.FirstOrDefault(p => p.InstructorId == instructor.Id);
            var applicationUser = DbContext.Users.FirstOrDefault(p => p.Email == instructor.Email);
            if (instructor.Courses == null)
            {
                courses.Instructor = null;
            }

            instructor.Courses = null;
            DbContext.InstructorDatabase.Remove(instructor);
            DbContext.Users.Remove(applicationUser);
            DbContext.SaveChanges();
            TempData["Message"] = "You Successfully deleted the Instructor.";

            return RedirectToAction(nameof(InstructorController.Index));
        }

        [HttpGet]
        public ActionResult Detail(int? id)
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

            var allInstructor = new InstructorViewModel();
            allInstructor.FirstName = instructor.FirstName;
            allInstructor.LastName = instructor.LastName;
            allInstructor.Email = instructor.Email;
            allInstructor.Courses = instructor.Courses;
            ViewBag.id = id;

            return View(allInstructor);
        }

        [HttpGet]
        public ActionResult ImportInstructor()
        {
            return View(new List<InstructorViewModel>());
        }

        [HttpPost]
        public ActionResult ImportInstructor(HttpPostedFileBase postedFile)
        {
            List<Instructor> instructors = new List<Instructor>();

            if (postedFile != null)
            {
                //Upload and save the file  
                string csvPath = Server.MapPath("~/Uploads/") + Path.GetFileName(postedFile.FileName);
                postedFile.SaveAs(csvPath);
                //Create a DataTable.  
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[3] {
                     new DataColumn("FirstName", typeof(string)),
                     new DataColumn("LastName", typeof(string)),
                     new DataColumn("Email", typeof(string)),
                      });
                string csvData = System.IO.File.ReadAllText(csvPath);
                //Execute a loop over the rows.  
                foreach (string row in csvData.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        dt.Rows.Add();
                        int i = 0;

                        //Execute a loop over the columns.  
                        foreach (string cell in row.Split(','))
                        {
                            //cell.Replace("\r\n", "");
                            if (cell == "\r" || cell == "\n")
                            {
                                return RedirectToAction(nameof(StudentsController.Index));
                            }
                            else
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = cell;
                                i++;
                            }

                        }

                        if (dt.Columns.Count == 3)
                        {
                            var instructor = (new Instructor
                            {
                                FirstName = row.Split(',')[0],
                                LastName = row.Split(',')[1],
                                Email = row.Split(',')[2],
                            });

                            var instr = DbContext.InstructorDatabase.ToList();
                            if (instr.Count != 0)
                            {
                                if (instr.Any(p => p.Email == instructor.Email))
                                {
                                    var instructorEmail = DbContext.InstructorDatabase.FirstOrDefault(p => p.Email == instructor.Email).Email;
                                    return RedirectToAction("EmailError");
                                }
                            }

                            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                            var user = new ApplicationUser { UserName = instructor.Email, Email = instructor.Email };
                            var result = userManager.Create(user, instructor.Password);

                            instructors.Add(instructor);
                            DbContext.InstructorDatabase.Add(instructor);

                            var model = new InstructorViewModel();
                            model.FirstName = instructor.FirstName;
                            model.LastName = instructor.LastName;
                            model.Email = instructor.Email;
                            char[] charsToTrim = { '\r' };
                            model.Email = instructor.Email.Trim(charsToTrim);
                            DbContext.SaveChanges();

                            var userId = user.Id;
                            user.Email = user.Email.Trim(charsToTrim);
                            DbContext.Users.Add(user);
                            DbContext.SaveChanges();

                            if (!userManager.IsInRole(user.Id, "Instructor"))
                            {
                                userManager.AddToRoleAsync(user.Id, "Instructor");
                            }

                            string code = userManager.GenerateEmailConfirmationToken(userId);
                            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                            userManager.SendEmailAsync(userId, "Notification",
                                "Hello, You are registered as student at MITT.Your current Password is Password-1.Please change your password by clicking <a href=\"" + callbackUrl + "\"> here</a>");
                        }
                    }
                }
            }

            return RedirectToAction("Index");
        }

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
            }

            var model = new AssignInstructorViewModel();
            model.InstructorList = instructorList;
            model.CourseId = id;

            return View(model);
        }

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
                var courseId = assignInstructor.Courses.FirstOrDefault(p => p.Id == course.Id);
                course.Instructor = assignInstructor;
                assignInstructor.Courses.Add(course);

                DbContext.SaveChanges();
            }

            return RedirectToAction("Details", "Course", new { id = model.CourseId });
        }

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