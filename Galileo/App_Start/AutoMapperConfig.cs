using Galileo.Models;
using Galileo.ViewModels;
using System;

namespace Galileo
{
    public class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            AutoMapper.Mapper.CreateMap<Course, Module>()
                .ForMember(dest => dest.name, opts => opts.MapFrom(src => src.course_name))
                .ForMember(dest => dest.id, opts => opts.MapFrom(src => src.course_id.ToString()))
                .ForMember(dest => dest.total_hours, opts => opts.MapFrom(src => GetTotalHours(src.course_total_time)))
                .ForMember(dest => dest.hours_per_day, opts => opts.MapFrom(src => GetHoursPerDay(src.course_begin_date, src.course_end_date, src.course_total_time)))
                .ForMember(dest => dest.hours_per_week, opts => opts.MapFrom(src => GetHoursPerDay(src.course_begin_date, src.course_end_date, src.course_total_time) * 7.0f));

            AutoMapper.Mapper.CreateMap<Project, Module>()
                .ForMember(dest => dest.name, opts => opts.MapFrom(src => src.project_name))
                .ForMember(dest => dest.id, opts => opts.MapFrom(src => src.project_id.ToString()))
                .ForMember(dest => dest.total_hours, opts => opts.MapFrom(src => GetTotalHours(src.project_total_time)))
                .ForMember(dest => dest.hours_per_day, opts => opts.MapFrom(src => GetHoursPerDay(src.project_begin_date, src.project_end_date, src.project_total_time)))
                .ForMember(dest => dest.hours_per_week, opts => opts.MapFrom(src => GetHoursPerDay(src.project_begin_date, src.project_end_date, src.project_total_time) * 7.0f));

            AutoMapper.Mapper.CreateMap<User, Module>()
                .ForMember(dest => dest.name, opts => opts.MapFrom(src => src.user_first_name + " " + src.user_last_name))
                .ForMember(dest => dest.id, opts => opts.MapFrom(src => src.user_id))
                .ForMember(dest => dest.total_hours, opts => opts.MapFrom(src => GetTotalHours(src.user_total_time)))
                .ForMember(dest => dest.hours_per_day, opts => opts.MapFrom(src => GetHoursPerDay(src.user_begin_date, src.user_end_date, src.user_total_time)))
                .ForMember(dest => dest.hours_per_week, opts => opts.MapFrom(src => GetHoursPerDay(src.user_begin_date, src.user_end_date, src.user_total_time) * 7.0f));
        }

        private static float GetTotalHours (int totalTime)
        {
            return totalTime / 60.0f;
        }

        private static float GetHoursPerDay (DateTime beginDate, DateTime endDate, int totalTime)
        {
            if (endDate > DateTime.Now)
                return (totalTime / 60.0f) / (DateTime.Now - beginDate).Days;
            else
                return (totalTime / 60.0f) / (endDate - beginDate).Days;
        }
    }
}