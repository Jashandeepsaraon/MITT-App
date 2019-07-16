using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Scheduler_App.Models.ViewModel
{
    public class CreateEditCourseViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        //public List<SelectListItem> ProgramList { get; set; }
        //public List<SelectListItem> InstructorList { get; set; }
        public int Hours { get; set; }
        //public int ProgramId { get; set; }
        //public int? InstructorsId { get; set; }
    }
}