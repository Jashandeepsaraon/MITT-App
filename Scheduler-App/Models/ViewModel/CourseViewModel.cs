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
        public string Name { get; set; }
        public string ProgramName { get; set; }
        public List<SelectListItem> ProgramList { get; set; }
        public virtual Instructor Instructor { get; set; }
        public int? InstructorId { get; set; }
        public int Hours { get; set; }
        public int? ProgramId { get; set; }
        public DateTime StartDate { get; internal set; }
        public DateTime EndDate { get; internal set; }
        public TimeSpan StartTime { get; internal set; }
        public TimeSpan EndTime { get; internal set; }
        public int? PrerequisiteForId { get; set; }
        public int? PrerequisiteOfId { get; set; }
        public string PrerequisiteFor { get; set; }
        public string PrerequisiteOf { get; set; }
    }
}