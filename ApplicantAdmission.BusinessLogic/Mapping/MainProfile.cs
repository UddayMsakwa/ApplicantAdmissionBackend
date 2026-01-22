using AutoMapper;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Documents;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admin;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Education;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Faculty;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Manager;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Program;
using ApplicantAdmission.DataAccess.Entities;
using System.Linq;

namespace ApplicantAdmission.BusinessLogic.Mapping;

public class MainProfile : Profile
{
    public MainProfile()
    {
        CreateMap<Applicant, ApplicantDto>()
            .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserId))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.User.Email))
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.User.FullName))
            .ForMember(d => d.Phone, o => o.MapFrom(s => s.Phone))
            .ForMember(d => d.DateOfBirth, o => o.MapFrom(s => s.DateOfBirth))
            .ForMember(d => d.Gender, o => o.MapFrom(s => s.Gender))
            .ForMember(d => d.Citizenship, o => o.MapFrom(s => s.Citizenship));

        CreateMap<ApplicantUpdateDto, Applicant>()
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<UserEntity, UserDto>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));

        CreateMap<UserEntity, ManagerDto>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));

        CreateMap<ApplicantAdmissionCreateDto, ApplicantAdmissionEntity>();

        
        CreateMap<ApplicantAdmissionEntity, ApplicantAdmissionDto>()
            .ForMember(d => d.Manager, opt => opt.MapFrom(s => s.ManagerUser))
            .ForMember(d => d.Applicant, opt => opt.MapFrom(s => s.Applicant));

        CreateMap<PassportDocument, PassportDocumentDto>();
        CreateMap<EducationDocument, EducationDocumentDto>();
        CreateMap<DocumentScan, DocumentScanDto>();

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
