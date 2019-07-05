using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scheduler_App.Models.Domain
{
    public class Program
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<Instructor> Instructors { get; set; }
        public virtual List<Student> Students { get; set; }
        public int StudentId { get; set; }
        public DateTime StartDate { get; set; }

        public Program()
        {
            Instructors = new List<Instructor>();
            Students = new List<Student>();
        }
    }
}