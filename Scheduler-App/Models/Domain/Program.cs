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
        public DateTime StartDate { get; set; }
        public virtual List<Course> Courses { get; set; }

        public Program()
        {   
            Courses = new List<Course>();
        }
    }
}