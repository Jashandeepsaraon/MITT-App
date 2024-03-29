﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Scheduler_App.Models.ViewModel
{
    public class CreateEditStudentViewModel
    {
        public int Id { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Please Enter Only Alphabets In FirstName Field")]
        public string FirstName { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Please Enter Only Alphabets In LastName Field")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }

        public CreateEditStudentViewModel()
        {
            Password = "Password-1";
        }
    }
}