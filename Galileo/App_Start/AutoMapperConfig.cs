using Galileo.Models;
using Galileo.ViewModels;

namespace Galileo
{
    public class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            AutoMapper.Mapper.CreateMap<Course, Module>()
                .ForMember(dest => dest.name, opts => opts.MapFrom(src => src.course_name))
                .ForMember(dest => dest.id, opts => opts.MapFrom(src => src.course_id.ToString()))
                .ForMember(dest => dest.total_hours, opts => opts.MapFrom(src => src.course_total_hours))
                .ForMember(dest => dest.hours_per_day, opts => opts.MapFrom(src => src.hours_per_day))
                .ForMember(dest => dest.hours_per_week, opts => opts.MapFrom(src => src.hours_per_week));

            AutoMapper.Mapper.CreateMap<Project, Module>()
                .ForMember(dest => dest.name, opts => opts.MapFrom(src => src.project_name))
                .ForMember(dest => dest.id, opts => opts.MapFrom(src => src.project_id.ToString()))
                .ForMember(dest => dest.total_hours, opts => opts.MapFrom(src => src.project_total_hours))
                .ForMember(dest => dest.hours_per_day, opts => opts.MapFrom(src => src.hours_per_day))
                .ForMember(dest => dest.hours_per_week, opts => opts.MapFrom(src => src.hours_per_week));

            AutoMapper.Mapper.CreateMap<User, Module>()
                .ForMember(dest => dest.name, opts => opts.MapFrom(src => src.user_first_name + " " + src.user_last_name))
                .ForMember(dest => dest.id, opts => opts.MapFrom(src => src.user_id))
                .ForMember(dest => dest.total_hours, opts => opts.MapFrom(src => src.user_total_hours))
                .ForMember(dest => dest.hours_per_day, opts => opts.MapFrom(src => src.user_total_hours))
                .ForMember(dest => dest.hours_per_week, opts => opts.MapFrom(src => src.user_total_hours));
        }
    }
}