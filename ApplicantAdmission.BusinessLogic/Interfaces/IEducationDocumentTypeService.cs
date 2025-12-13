using ApplicantAdmission.BusinessLogic.Models.Dtos.Education;

namespace ApplicantAdmission.BusinessLogic.Interfaces;

public interface IEducationDocumentTypeService
{
    Task<List<EducationDocumentTypeDto>> GetAllAsync();
}
