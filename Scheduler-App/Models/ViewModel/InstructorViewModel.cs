using Scheduler_App.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Scheduler_App.Models.ViewModel
{
    public class InstructorViewModel
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
       
        public int ProgramId { get; set; }
        public List<SelectListItem> ProgramList { get; set; }
        public int CourseId { get; set; }
        public List<SelectListItem> CourseList { get; set; }
        public virtual List<Course> Courses { get; set; }

        public InstructorViewModel()
        {
            Password = "Password-1";
            CourseList = new List<SelectListItem>();
            ProgramList = new List<SelectListItem>();
            Courses = new List<Course>();
        }

    }
}