using Scheduler_App.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Scheduler_App.Models.ViewModel
{
    public class CreateEditSchoolProgramViewModel
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public virtual List<Instructor> Instructors { get; set; }
        public virtual List<StudentViewModel> Students { get; set; }

        public CreateEditSchoolProgramViewModel()
        {
            StartDate = DateTime.Now;
        }

    }
}