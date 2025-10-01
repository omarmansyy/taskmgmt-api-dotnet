using AutoMapper;
using TaskMgmt.Api.Domain.Entities;
using TaskMgmt.Api.Dtos.Projects;
using TaskMgmt.Api.Dtos.Tasks;
using TaskMgmt.Api.Dtos.Users;


namespace TaskMgmt.Api.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Project, ProjectReadDto>();
        CreateMap<ProjectCreateDto, Project>();

        CreateMap<TaskItem, TaskReadDto>();
        CreateMap<TaskCreateDto, TaskItem>();
        CreateMap<TaskUpdateDto, TaskItem>();
        CreateMap<User, UserReadDto>();
        CreateMap<UserCreateDto, User>();
    }
}
