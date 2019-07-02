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
           
            var instructor = Mapper.Map<Instructor>(formData);
            //var Instructor = formData.Instructor;
            if (!id.HasValue)
            {
                DbContext.InstructorDatabase.Add(instructor);
                DbContext.SaveChanges();
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
        public ActionResult EditProgram(int id, InstructorViewModel formData)
        {
            return SaveInstructor(id, formData);
        }
    }
}