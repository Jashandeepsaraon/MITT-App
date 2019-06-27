﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scheduler_App.Models.Domain
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get;set; }
        public string LastName { get; set; }
        public int StudentNumber { get; set; }

        public string Email { get; set; }
        public virtual SchoolProgram SchoolProgram { get; set; }
        public int SchoolProgramId { get; set; }

    }
}