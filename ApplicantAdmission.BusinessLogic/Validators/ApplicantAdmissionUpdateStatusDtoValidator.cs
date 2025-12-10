using FluentValidation;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;

public class ApplicantAdmissionUpdateStatusDtoValidator
    : AbstractValidator<ApplicantAdmissionUpdateStatusDto>
{
    public ApplicantAdmissionUpdateStatusDtoValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Status cannot be empty.")
            .Must(s => new[] { "Submitted", "InReview", "Accepted", "Rejected" }.Contains(s))
            .WithMessage("Invalid status value.");
    }
}
