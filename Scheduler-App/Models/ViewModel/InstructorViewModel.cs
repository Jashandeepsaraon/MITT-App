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
        public string Email { get; set; }

        public virtual SchoolProgramViewModel SchoolProgram { get; set; }
        public int SchoolProgramId { get; set; }

        public InstructorViewModel()
        {

        }

        public InstructorViewModel(Instructor instructor)
        {
            Id = instructor.Id;
            FirstName = instructor.FirstName;
            LastName = instructor.LastName;
            SchoolProgramId = instructor.SchoolProgramId;
        }
    }
}