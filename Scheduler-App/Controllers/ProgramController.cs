using Microsoft.AspNet.Identity;
using Scheduler_App.Models;
using Scheduler_App.Models.Domain;
using Scheduler_App.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
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
        // GET: Program
        public ActionResult Index()
        {
            return View();
        }

        //GET : CreateProgram
        [HttpGet]

        [Authorize(Roles = "Admin")]

        public ActionResult CreateProgram()

        {
            return View();
        }

        [HttpPost]

        [Authorize(Roles = "Admin")]

        public ActionResult CreateProgram (CreateEditSchoolProgramViewModel formData)

        {

            return SaveProgram(null, formData);

        }



        private ActionResult SaveProgram(int? id, CreateEditSchoolProgramViewModel formData)

        {
            if (!ModelState.IsValid)
            {

                return View();

            }
            Program program;

            var userId = User.Identity.GetUserId();

            if (!id.HasValue)

            {

                program = new Program();

                //var applicationUser = DbContext.Users.FirstOrDefault(user => user.Id == userId);

                //if (applicationUser == null)

                //{

                //    return RedirectToAction(nameof(HomeController.Index));

                //}

                // project.Users.Add(applicationUser);

                DbContext.ProgramDatabase.Add(program);
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

            program.StartDate = DateTime.Now;

            DbContext.SaveChanges();

            return RedirectToAction(nameof(ProgramController.Index));

        }
    }
}