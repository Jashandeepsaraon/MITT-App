using Scheduler_App.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scheduler_App.Models.ViewModel
{
    public class SchoolProgramViewModel
    {
        public string Name { get; set; }
        public virtual List<InstructorViewModel> Instructors { get; set; }
        public virtual List<StudentViewModel> Students { get; set; }
        public DateTime StartDate { get; set; }
    }
}