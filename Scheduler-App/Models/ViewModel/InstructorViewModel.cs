using Scheduler_App.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
        public virtual List<CourseViewModel> Courses { get; set; }

        public InstructorViewModel()
        {
            Password = "Password-1";
        }
    }
}