using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Scheduler_App.Models.ViewModel
{
    public class AssignCourseToStudentViewModel
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int ProgramId { get; set; }
        public List<SelectListItem> ProgramList { get; set; }
        public List<SelectListItem> AddCourses { get; set; }
        public string AddSelectedCourses { get; set; }
    }
}