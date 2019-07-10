using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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

            var program = DbContext.ProgramDatabase
               .Select(p => new SelectListItem()
               {
                   Text = p.Name,
                   Value = p.Id.ToString(),
               }).ToList();
            var model = new StudentViewModel();
            model.Programs = program;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateStudent(StudentViewModel formData)
        {
            return SaveStudent(null, formData);
        }

        private ActionResult SaveStudent(int? id, StudentViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = new ApplicationUser { UserName = formData.Email, Email = formData.Email };
            var result = userManager.CreateAsync(user, formData.Password);
            var userId = user.Id;
            var program = DbContext.ProgramDatabase.ToList();
            var student = Mapper.Map<Student>(formData);
            formData.Programs = program
                .Select(p => new SelectListItem()
                {
                    Text = p.Name,
                    Value = p.Id.ToString(),
                }).ToList();
            if (!id.HasValue)
            {
                student.ProgramName = program.FirstOrDefault(p => p.Id == formData.ProgramId).Name;
                student.Program.StartDate = program.FirstOrDefault(p => p.Id == formData.ProgramId).StartDate;
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
                if (student
                    == null)
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

            var model = new StudentViewModel();
            model.FirstName = student.FirstName;
            model.LastName = student.LastName;
            model.Email = student.Email;

            return View(model);
        }
        //POST:
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult EditStudent(int id, StudentViewModel formData)
        {
            return SaveStudent(id, formData);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = DbContext.StudentDatabase.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST:Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Student student = DbContext.StudentDatabase.Find(id);
            DbContext.StudentDatabase.Remove(student);
            DbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        //Get:
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction(nameof(StudentsController.Index));

            var userId = User.Identity.GetUserId();

            var allstudent = DbContext.StudentDatabase.FirstOrDefault(p =>
            p.Id == id.Value);

            if (allstudent == null)
                return RedirectToAction(nameof(StudentsController.Index));

            var student = new StudentViewModel();
            student.FirstName = student.FirstName;
            student.LastName = student.LastName;
            student.Email = student.Email;
            student.ProgramName = student.ProgramName;

            return View(student);
        }
    }
}


