using Scheduler_App.Models.Domain;
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
        public List<SelectListItem> Programs { get; set; }
        public StudentViewModel()
        {
            Password = "Password-1";
        }
    }
}