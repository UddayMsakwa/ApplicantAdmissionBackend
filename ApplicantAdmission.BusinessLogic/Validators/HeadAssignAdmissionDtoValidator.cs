using ApplicantAdmission.BusinessLogic.Models.Dtos.Head;
using FluentValidation;

namespace ApplicantAdmission.BusinessLogic.Validators;

public class HeadAssignAdmissionDtoValidator : AbstractValidator<HeadAssignAdmissionDto>
{
    public HeadAssignAdmissionDtoValidator()
    {
        RuleFor(x => x.ManagerId).NotEmpty();
    }
}
