using Microsoft.AspNet.Identity;
using Scheduler_App.Models;
using Scheduler_App.Models.Domain;
using Scheduler_App.Models.ViewModel;
using System;
using System.Collections.Generic;
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
            return View();
        }
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
            return SaveProgram(null, formData);
        }

        private ActionResult SaveProgram(int? id, InstructorViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Instructor instructor;
            var userId = User.Identity.GetUserId();
            if (!id.HasValue)
            {
                instructor = new Instructor();
                //var applicationUser = DbContext.Users.FirstOrDefault(user => user.Id == userId);

                //if (applicationUser == null)

                //{

                //    return RedirectToAction(nameof(HomeController.Index));

                //}

                // project.Users.Add(applicationUser);

                DbContext.Instructors.Add(instructor);
            }
            else
            {
                instructor = DbContext.Instructors.FirstOrDefault(p => p.Id == id);
                if (instructor == null)
                {
                    return RedirectToAction(nameof(ProgramController.Index));
                }
            }
            instructor.FirstName = formData.FirstName;
            instructor.LastName = formData.LastName;
            DbContext.SaveChanges();
            return RedirectToAction(nameof(ProgramController.Index));
        }

    }
}