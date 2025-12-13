using ApplicantAdmission.BusinessLogic.Models.Dtos.Faculty;

namespace ApplicantAdmission.BusinessLogic.Interfaces;

public interface IFacultyService
{
    Task<List<FacultyDto>> GetAllAsync();
}
