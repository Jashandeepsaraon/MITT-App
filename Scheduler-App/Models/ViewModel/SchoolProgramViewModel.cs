using Scheduler_App.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scheduler_App.Models.ViewModel
{
    public class SchoolProgramViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<InstructorViewModel> Instructors { get; set; }
        public virtual List<StudentViewModel> Students { get; set; }
        public DateTime StartDate { get; set; }

        public SchoolProgramViewModel()
        {

        }

        public SchoolProgramViewModel(SchoolProgram program)
        {
            Id = program.Id;
            Name = program.Name;
            Instructors = program.Instructors.Select(p => new InstructorViewModel(p)).ToList();
            Students = program.Students.Select(p => new StudentViewModel(p)).ToList();
        }
    }
}