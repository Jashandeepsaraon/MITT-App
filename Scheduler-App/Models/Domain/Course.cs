using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Scheduler_App.Models.Domain
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProgramName { get; set; }
        public string InstructorName { get; set; }
        //public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }
        //public TimeSpan ClassStartTime { get; set; }
        //public TimeSpan ClassEndTime { get; set; }
        public virtual Program Program { get; set; }
        public int ProgramId { get; set; }
        //public virtual ClassRoom ClassRoom { get; set; }
        //public int ClassRoomId { get; set; }
        public virtual Instructor Instructor { get; set; }
        //public string InstructorId { get; set; }
        public TimeSpan Hours { get; set; }
    }
}