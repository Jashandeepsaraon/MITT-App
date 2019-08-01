﻿using Scheduler_App.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Scheduler_App.Models.ViewModel
{
    public class StudentViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ProgramName { get; set; }
        public int? ProgramId { get; set; }
        //public string CourseName { get; set; }
        public virtual List<Course> Courses { get; set; }
        public int? CourseId { get; set; }
        public List<SelectListItem> ProgramList { get; set; }
        public StudentViewModel()
        {
            Password = "Password-1";
        }
    }
}