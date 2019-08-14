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
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Please Enter Only Alphabets in Name Field")]
        public string Name { get; set; }
        public virtual Program Program { get; set; }
        public int? ProgramId { get; set; }
        public virtual Instructor Instructor { get; set; }
        public int? InstructorId { get; set; }
        public virtual List<Student> Students { get; set; }
        public int Hours { get; set; }
        public int PrerequisiteFor { get; internal set; }
        public int PrerequisiteOf { get; internal set; }
        public DateTime StartDate { get; internal set; }
        public DateTime EndDate { get; internal set; }
        public double DailyHours { get; internal set; }
        public TimeSpan StartTime { get; internal set; }
        public TimeSpan EndTime { get; internal set; }

        public Course()
        {
            Students = new List<Student>();
            DailyHours = 5.5;
            //StartTime = new DateTime(1, 1, 1, 8, 45, 00);
        }
    }
}