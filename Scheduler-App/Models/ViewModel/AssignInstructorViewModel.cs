using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Scheduler_App.Models.ViewModel
{
    public class AssignInstructorViewModel
    {
        public int CourseId { get; set; }
        public int? InstructorId { get; set; }
        public List<SelectListItem> InstructorList { get; set; }
        public string AddSelectedInstructor { get; set; }
    }
}