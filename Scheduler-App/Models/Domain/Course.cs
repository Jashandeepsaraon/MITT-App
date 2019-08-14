using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public double DailyHours { get; set; }
        public int? PrerequisiteOf { get; set; }
        public int? PrerequisiteFor { get; set; }

        public Course()
        {
            Students = new List<Student>();
            DailyHours = 5.5;
            //StartTime = new DateTime(1, 1, 1, 8, 45, 00);
        }
    }
}