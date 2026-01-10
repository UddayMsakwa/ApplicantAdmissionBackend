using AutoMapper;
using ApplicantAdmission.DataAccess.Entities;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Document;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Education;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Manager;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Program;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Faculty;


namespace ApplicantAdmission.BusinessLogic.Mapping
{
    public class MainProfile : Profile
    {
        public MainProfile()
        {
            
            CreateMap<Applicant, ApplicantDto>();
            CreateMap<ApplicantCreateDto, Applicant>();

            
            CreateMap<ApplicantAdmissionEntity, ApplicantAdmissionDto>()
                .ForMember(dest => dest.Program,
                           opt => opt.MapFrom(src => src.AdmissionProgram.Program));

            CreateMap<ApplicantAdmissionCreateDto, ApplicantAdmissionEntity>();

            
            CreateMap<Document, DocumentDto>();
            CreateMap<EducationDocument, EducationDocumentDto>();
            
            CreateMap<DocumentCreateDto, Document>();
            
            CreateMap<EducationDocumentCreateDto, EducationDocument>();


            CreateMap<EducationLevel, EducationLevelDto>();
            CreateMap<EducationDocumentType, EducationDocumentTypeDto>();

            CreateMap<Manager, ManagerDto>();


            CreateMap<ProgramEntity, ProgramDto>()
    .ForMember(dest => dest.FacultyName,
        opt => opt.MapFrom(src => src.Faculty.Name))
    .ForMember(dest => dest.LevelName,
        opt => opt.MapFrom(src => src.Level.Name));



            CreateMap<Faculty, FacultyDto>();
        }
    }
}
