using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Scheduler_App.Models.Domain
{
    public class Student
    {
        public int Id { get; set; }
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Please Enter Only Alphabets in FirstName Field")]
        public string FirstName { get;set; }
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Please Enter Only Alphabets in LastName Field")]
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string ProgramName { get; set; }
        public string Password { get; set; }
        public virtual List<Course> Courses { get; set; }

        public Student()
        {
            Password = "Password-1";
            Courses = new List<Course>();
        }
    }
}