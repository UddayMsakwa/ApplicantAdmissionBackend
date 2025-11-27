using FluentValidation;
using ApplicantAdmission.BusinessLogic.Models;

public class FacultyDtoValidator : AbstractValidator<FacultyDto>
{
    public FacultyDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
