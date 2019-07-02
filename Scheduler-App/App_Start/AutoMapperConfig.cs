using AutoMapper;
using Scheduler_App.Models.Domain;
using Scheduler_App.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scheduler_App.App_Start
{
    public class AutoMapperConfig
    {
        public static void Init()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Program, CreateEditSchoolProgramViewModel>().ReverseMap();
                cfg.CreateMap<CreateEditSchoolProgramViewModel, Program>().ReverseMap();
                cfg.CreateMap<Instructor, InstructorViewModel>().ReverseMap();
            });
        }
    }
}