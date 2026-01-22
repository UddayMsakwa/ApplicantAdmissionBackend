
namespace ApplicantAdmission.BusinessLogic.Models.Dtos.SelectedPrograms;

public sealed class SelectedProgramDto
{
    public Guid Id { get; set; }              
    public Guid ProgramId { get; set; }      

    
    public string Name { get => ProgramName; set => ProgramName = value; }

    public string ProgramName { get; set; } = "";

    public Guid FacultyId { get; set; }       
    public string FacultyName { get; set; } = "";

    public int EducationLevelId { get; set; } 
    public string EducationLevelName { get; set; } = "";

    public int Priority { get; set; }
}
