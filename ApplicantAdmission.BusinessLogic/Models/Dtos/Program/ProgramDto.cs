namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Program;

public class ProgramDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";

    public string Code { get; set; } = "";
    public string Language { get; set; } = "";
    public string StudyForm { get; set; } = "";

    public Guid FacultyId { get; set; }
    public string FacultyName { get; set; } = "";

    public int LevelId { get; set; }
    public string LevelName { get; set; } = "";
}
