namespace ApplicantAdmission.DataAccess.Entities;

public class Faculty
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<ProgramEntity> Programs { get; set; } = new List<ProgramEntity>();

}
