using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scheduler_App.Models.Domain
{
    public class SchoolProgram
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<Instructor> Instructors { get; set; }
        public virtual List<Student> Students { get; set; }
        public DateTime StartDate { get; set; }

        public SchoolProgram()
        {
            Instructors = new List<Instructor>();
            Students = new List<Student>();
        }
    }
}