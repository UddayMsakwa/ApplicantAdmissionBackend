using FluentValidation;
using ApplicantAdmission.BusinessLogic.Models;

public class ProgramDtoValidator : AbstractValidator<ProgramDto>
{
    public ProgramDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.FacultyId).NotEmpty();
        RuleFor(x => x.LevelId).NotEmpty();
    }
}
