using ApplicantAdmission.BusinessLogic.Models.Applicant;
using ApplicantAdmission.BusinessLogic.Models.Manager;
using ApplicantAdmission.BusinessLogic.Models.Program;
using ApplicantAdmission.BusinessLogic.Models.Faculty;
using ApplicantAdmission.BusinessLogic.Models.Admission;
using ApplicantAdmission.BusinessLogic.Models.Document;
using ApplicantAdmission.DataAccess.Entities;
using AutoMapper;

namespace ApplicantAdmission.BusinessLogic.Mapping
{
    public class MainProfile : Profile
    {
        public MainProfile()
        {
            
            CreateMap<Applicant, ApplicantDto>();
            CreateMap<ApplicantCreateDto, Applicant>();

           
            CreateMap<Manager, ManagerDto>();

            
            CreateMap<Faculty, FacultyDto>();

            
            CreateMap<ProgramEntity, ProgramDto>();
            CreateMap<ProgramDto, ProgramEntity>();

           
            CreateMap<ApplicantAdmission, ApplicantAdmissionDto>();

           
            CreateMap<Document, DocumentDto>();
            CreateMap<EducationDocument, EducationDocumentDto>();
        }
    }
}
