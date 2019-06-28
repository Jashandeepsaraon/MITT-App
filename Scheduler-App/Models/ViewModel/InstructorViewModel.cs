using Scheduler_App.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scheduler_App.Models.ViewModel
{
    public class InstructorViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int InstructorNumber { get; set; }
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
            InstructorNumber = instructor.InstructorNumber;
            SchoolProgramId = instructor.SchoolProgramId;
        }
    }
}