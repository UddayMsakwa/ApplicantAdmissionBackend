using FluentValidation;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Faculty;

public class FacultyDtoValidator : AbstractValidator<FacultyDto>
{
    public FacultyDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
