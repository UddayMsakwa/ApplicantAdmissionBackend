using FluentValidation;
using ApplicantAdmission.BusinessLogic.Models;

public class ApplicantAdmissionDtoValidator : AbstractValidator<ApplicantAdmissionDto>
{
    public ApplicantAdmissionDtoValidator()
    {
        RuleFor(x => x.ApplicantId).NotEmpty();
        RuleFor(x => x.AdmissionProgramId).NotEmpty();
        RuleFor(x => x.Status).NotEmpty();
    }
}
