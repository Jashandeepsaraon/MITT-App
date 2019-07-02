using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scheduler_App.Models.Domain
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }
        //public TimeSpan ClassStartTime { get; set; }
        //public TimeSpan ClassEndTime { get; set; }
        public virtual Program SchoolProgram { get; set; }
        public int SchoolProgramId { get; set; }
        //public virtual ClassRoom ClassRoom { get; set; }
        //public int ClassRoomId { get; set; }
        public virtual Instructor Instructor { get; set; }
        public string InstructorId { get; set; }
        public TimeSpan Hours { get; set; }
    }
}