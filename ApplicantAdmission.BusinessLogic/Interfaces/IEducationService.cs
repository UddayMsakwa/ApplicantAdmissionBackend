using ApplicantAdmission.BusinessLogic.Models.Dtos.Education;

namespace ApplicantAdmission.BusinessLogic.Interfaces;

public interface IEducationLevelService
{
    Task<List<EducationLevelDto>> GetAllAsync();
}
