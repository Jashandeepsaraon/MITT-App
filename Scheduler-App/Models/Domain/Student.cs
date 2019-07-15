using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scheduler_App.Models.Domain
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get;set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ProgramName { get;set; }
        public virtual Program Program { get; set; }
        public int ProgramId { get; set; }
        public virtual List<Course> Courses { get; set; }
        public virtual List<Instructor> Instructors { get; set; }

        public Student()
        {
            Courses = new List<Course>();
            Instructors = new List<Instructor>();
        }
    }
}