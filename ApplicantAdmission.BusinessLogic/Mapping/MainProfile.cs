using AutoMapper;
using ApplicantAdmission.DataAccess.Entities;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Document;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Education;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Manager;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Program;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Faculty;

public class MainProfile : Profile
{
    public MainProfile()
    {
        CreateMap<Applicant, ApplicantDto>();
        CreateMap<ApplicantCreateDto, Applicant>();

        CreateMap<ApplicantAdmission, ApplicantAdmissionDto>();

        CreateMap<Document, DocumentDto>();
        CreateMap<EducationDocument, EducationDocumentDto>();

        CreateMap<Manager, ManagerDto>();

        CreateMap<ProgramEntity, ProgramDto>();

        CreateMap<Faculty, FacultyDto>();
    }
}
