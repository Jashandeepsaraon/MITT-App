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
                cfg.CreateMap<Student, StudentViewModel>().ReverseMap();
                cfg.CreateMap<Student, CreateEditStudentViewModel>().ReverseMap();
                cfg.CreateMap<CreateEditStudentViewModel, Student>().ReverseMap();
                cfg.CreateMap<Course, CreateEditCourseViewModel>().ReverseMap();
                cfg.CreateMap<CreateEditCourseViewModel, Course>().ReverseMap();
                cfg.CreateMap<AssignCourseViewModel, Instructor>().ReverseMap();
            });
        }
    }
}