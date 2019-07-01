using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Scheduler_App.Models;
using Scheduler_App.Models.Domain;

namespace Scheduler_App.Controllers
{
    public class SchoolProgramsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SchoolPrograms
        public ActionResult Index()
        {
            return View(db.SchoolPrograms.ToList());
        }

        // GET: SchoolPrograms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SchoolProgram schoolProgram = db.SchoolPrograms.Find(id);
            if (schoolProgram == null)
            {
                return HttpNotFound();
            }
            return View(schoolProgram);
        }

        // GET: SchoolPrograms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SchoolPrograms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,StartDate")] SchoolProgram schoolProgram)
        {
            if (ModelState.IsValid)
            {
                db.SchoolPrograms.Add(schoolProgram);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(schoolProgram);
        }
      
        
            
           
        // GET: SchoolPrograms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SchoolProgram schoolProgram = db.SchoolPrograms.Find(id);
            if (schoolProgram == null)
            {
                return HttpNotFound();
            }
            return View(schoolProgram);
        }

        // POST: SchoolPrograms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,StartDate")] SchoolProgram schoolProgram)
        {
            if (ModelState.IsValid)
            {
                db.Entry(schoolProgram).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(schoolProgram);
        }

        // GET: SchoolPrograms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SchoolProgram schoolProgram = db.SchoolPrograms.Find(id);
            if (schoolProgram == null)
            {
                return HttpNotFound();
            }
            return View(schoolProgram);
        }

        // POST: SchoolPrograms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SchoolProgram schoolProgram = db.SchoolPrograms.Find(id);
            db.SchoolPrograms.Remove(schoolProgram);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
