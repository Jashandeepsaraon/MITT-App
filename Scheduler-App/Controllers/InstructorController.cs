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
        [Authorize(Roles = "Admin")]
        public ActionResult CreateInstructor()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
                DbContext.InstructorDatabase.Add(instructor);
                DbContext.SaveChanges();
                string code = userManager.GenerateEmailConfirmationToken(user.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                userManager.SendEmail(userId, "Notification",
                     "You are registered as an Instructor. Your Current Password is 'Password-1'. Please change your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
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
            DbContext.SaveChanges();
            return RedirectToAction(nameof(InstructorController.Index));
        }

        //GET: EditProgram
        [HttpGet]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public ActionResult EditInstructor(int id, InstructorViewModel formData)
        {
            return SaveInstructor(id, formData);
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(InstructorController.Index));
            }
            var instructor = DbContext.InstructorDatabase.FirstOrDefault(p => p.Id == id);
            if (instructor != null)
            {
                DbContext.InstructorDatabase.Remove(instructor);
                DbContext.SaveChanges();
                return RedirectToAction(nameof(InstructorController.Index));
            }
            return RedirectToAction(nameof(InstructorController.Index));
        }

        [HttpGet]
        public ActionResult ImportInstructor()
        {
            return View(new List<InstructorViewModel>());
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
                        instructor.Add(instructors);
                        DbContext.InstructorDatabase.Add(instructors);
                        DbContext.SaveChanges();
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}
