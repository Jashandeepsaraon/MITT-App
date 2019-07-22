using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scheduler_App.Models.Domain
{
    public class Instructor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public virtual List<Course> Courses { get; set; }
        

        public Instructor()
        {
            Password = "Password-1";
            Courses = new List<Course>();
            
        }
    }
}