using AutoMapper;
using ApplicantAdmission.DataAccess.Entities;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Document;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Education;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Program;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Faculty;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admin;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Manager;

namespace ApplicantAdmission.BusinessLogic.Mapping;

public class MainProfile : Profile
{
    public MainProfile()
    {
        
        CreateMap<Applicant, ApplicantDto>()
            .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserId))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.User.Email))
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.User.FullName))
            .ForMember(d => d.Phone, o => o.MapFrom(s => s.Phone));

        
        CreateMap<ApplicantUpdateDto, Applicant>()
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));

        
        CreateMap<UserEntity, UserDto>()
            .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName))
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));

        
        CreateMap<UserEntity, ManagerDto>()
            .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role.ToString()));

        CreateMap<ApplicantAdmissionCreateDto, ApplicantAdmissionEntity>();

        CreateMap<ApplicantAdmissionEntity, ApplicantAdmissionDto>()
            .ForMember(d => d.Program, opt => opt.MapFrom(s => s.AdmissionProgram.Program))
            .ForMember(d => d.Manager, opt => opt.MapFrom(s => s.ManagerUser));

        CreateMap<Document, DocumentDto>();
        CreateMap<DocumentCreateDto, Document>();

        CreateMap<EducationDocument, EducationDocumentDto>();
        CreateMap<EducationDocumentCreateDto, EducationDocument>();

        CreateMap<EducationLevel, EducationLevelDto>();

        CreateMap<EducationDocumentType, EducationDocumentTypeDto>()
            .ForMember(d => d.LevelId, o => o.MapFrom(s => s.LevelId))
            .ForMember(d => d.NextLevelIds, o => o.MapFrom(s => s.NextLevels.Select(x => x.NextLevelId)));

        CreateMap<ProgramEntity, ProgramDto>()
            .ForMember(d => d.FacultyName, opt => opt.MapFrom(s => s.Faculty.Name))
            .ForMember(d => d.LevelName, opt => opt.MapFrom(s => s.Level.Name));

        CreateMap<Faculty, FacultyDto>();
    }
}
