using FluentValidation;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Program;

public class ProgramDtoValidator : AbstractValidator<ProgramDto>
{
    public ProgramDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.FacultyName).NotEmpty();
        RuleFor(x => x.LevelName).NotEmpty();
    }
}
