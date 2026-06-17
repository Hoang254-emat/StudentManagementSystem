using AutoMapper;
using MyWebApi.Entities;

namespace MyWebApi.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Class != null ? src.Class.ClassName : "No Class"));
            CreateMap<CreateStudentDto, Student>();
            CreateMap<UpdateStudentDto, Student>();

            CreateMap<Teacher, TeacherDto>();
            CreateMap<CreateTeacherDto, Teacher>();
            CreateMap<UpdateTeacherDto, Teacher>();

            CreateMap<Subject, SubjectDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SubjectId))
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher != null ? src.Teacher.Name : "No Teacher"));
            CreateMap<CreateSubjectDto, Subject>()
                .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.Id));
            CreateMap<UpdateSubjectDto, Subject>()
                .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.Id));

            CreateMap<Class, ClassDto>();
            CreateMap<CreateClassDto, Class>();

            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Class != null ? src.Class.ClassName : "No Class"))
                .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.SubjectId : "No Subject"))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.SubjectName : "No Subject"))
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher != null ? src.Teacher.Name : "No Teacher"));
            CreateMap<CreateCourseDto, Course>();
        }
    }
}
