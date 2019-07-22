using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Scheduler_App.Models.Domain
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual Program Program { get; set; }
        public int? ProgramId { get; set; }
        public virtual Instructor Instructor { get; set; }
        public int? InstructorId { get; set; }
        public virtual List<Student> Students { get; set; }
        public int Hours { get; set; }
        public Course()
        {
            Students = new List<Student>();
        }
    }
}