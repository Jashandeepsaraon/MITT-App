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
        public int InstructorNumber { get; set; }
        public virtual Program SchoolProgram { get; set; }
        public int SchoolProgramId { get; set; }
    }
}