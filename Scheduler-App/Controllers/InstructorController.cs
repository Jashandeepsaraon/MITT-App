using AutoMapper;
using AutoMapper.QueryableExtensions;
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

        //Method to get list of Instructors
        public ActionResult Index()
        {
            var model = DbContext
                .ProgramDatabase
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
            return SaveProgram(null, formData);
        }

        private ActionResult SaveProgram(int? id, InstructorViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var instructor = Mapper.Map<Program>(formData);
            if (!id.HasValue)
            {
                DbContext..Add(program);
                DbContext.SaveChanges();
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
            DbContext.SaveChanges();
            return RedirectToAction(nameof(ProgramController.Index));
        }

        //GET: EditProgram
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditProgram(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(ProgramController.Index));
            }
            var program = DbContext.ProgramDatabase.FirstOrDefault(p => p.Id == id);

            if (program == null)
            {
                return RedirectToAction(nameof(ProgramController.Index));
            }

            var model = new CreateEditSchoolProgramViewModel();
            model.Name = program.Name;
            return View(model);
        }
        //POST:
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult EditProgram(int id, CreateEditSchoolProgramViewModel formData)
        {
            return SaveProgram(id, formData);
        }
    }
}