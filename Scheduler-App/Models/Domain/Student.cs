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
        public string Password { get; set; }
        public string ProgramName { get;set; }
        //public virtual Program Program { get; set; }
        //public int ProgramId { get; set; }
        //public string CourseName { get; set; }
        public virtual Course Course { get; set; }
        public int CourseId { get; set; }


        public Student()
        {
            Password = "Password-1";
        }

    }
}