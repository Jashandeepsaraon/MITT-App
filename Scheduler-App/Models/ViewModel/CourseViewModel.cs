using Scheduler_App.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Scheduler_App.Models.ViewModel
{
    public class CourseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public TimeSpan ClassStartTime { get; set; }
        //public TimeSpan ClassEndTime { get; set; }
        //public virtual ClassRoom ClassRoom { get; set; }
        //public int ClassRoomId { get; set; }
        public List<SelectListItem> ProgramList { get; set; }
        public List<SelectListItem> InstructorList { get; set; }
        public TimeSpan Hours { get; set; }
        public int? ProgramId { get; set; }
        public int? InstructorId { get; set; }
    }
}