using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Scheduler_App.Models.Domain
{
    public class Program
    {
        public int Id { get; set; }
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Please Enter Only Alphabets in ProgramName Field")]
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public virtual List<Course> Courses { get; set; }

        public Program()
        {   
            Courses = new List<Course>();
        }
    }
}