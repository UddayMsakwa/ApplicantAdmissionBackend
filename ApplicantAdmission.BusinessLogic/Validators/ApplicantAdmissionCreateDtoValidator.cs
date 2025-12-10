using FluentValidation;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;

public class ApplicantAdmissionCreateDtoValidator : AbstractValidator<ApplicantAdmissionCreateDto>
{
    public ApplicantAdmissionCreateDtoValidator()
    {
        RuleFor(x => x.ApplicantId).NotEmpty();
        RuleFor(x => x.AdmissionProgramId).NotEmpty();
    }
}
