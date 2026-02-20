using AutoMapper;
using HoneyDoThis.Server.Models.Subtask;
using HoneyDoThis.Server.Models.Task;

namespace HoneyDoThis.Server.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Task mappings
            CreateMap<TaskEntity, TaskDto>()
                .ForMember(dest => dest.SubtaskCount, opt => opt.Ignore())
                .ForMember(dest => dest.CompletedSubtaskCount, opt => opt.Ignore());

            CreateMap<CreateTaskDto, TaskEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Completed, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.Expanded, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Subtasks, opt => opt.Ignore());

            CreateMap<UpdateTaskDto, TaskEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Subtasks, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Subtask mappings
            CreateMap<SubtaskEntity, SubtaskDto>();

            CreateMap<CreateSubtaskDto, SubtaskEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Completed, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Task, opt => opt.Ignore());

            CreateMap<UpdateSubtaskDto, SubtaskEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TaskId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Task, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}