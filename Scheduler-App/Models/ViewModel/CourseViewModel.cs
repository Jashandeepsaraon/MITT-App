using Scheduler_App.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Scheduler_App.Models.ViewModel
{
    public class CourseViewModel
    {
        public int Id { get; set; }
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Please Enter Only Alphabets in Name Field")]
        public string Name { get; set; }
        public string ProgramName { get; set; }
        public List<SelectListItem> ProgramList { get; set; }
        public virtual Instructor Instructor { get; set; }
        public int? InstructorId { get; set; }
        public int Hours { get; set; }
        public int? ProgramId { get; set; }       
    }
}