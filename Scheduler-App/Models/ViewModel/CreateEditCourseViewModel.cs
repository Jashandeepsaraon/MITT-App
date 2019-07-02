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
        public SelectList Programs { get; set; }
        public SelectList Instructors { get; set; }
        public TimeSpan Hours { get; set; }
    }
}