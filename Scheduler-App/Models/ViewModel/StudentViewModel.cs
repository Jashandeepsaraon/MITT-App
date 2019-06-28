using Scheduler_App.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scheduler_App.Models.ViewModel
{
    public class StudentViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int StudentNumber { get; set; }
        public string Email { get; set; }
        public virtual SchoolProgramViewModel SchoolProgram { get; set; }
        public int SchoolProgramId { get; set; }

        public StudentViewModel()
        {

        }

        public StudentViewModel(Student student)
        {
            Id = student.Id;
            FirstName = student.FirstName;
            LastName = student.LastName;
            StudentNumber = student.StudentNumber;
            Email = student.Email;
            SchoolProgramId = student.SchoolProgramId;
        }
    }
}