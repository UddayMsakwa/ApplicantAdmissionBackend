using FluentValidation;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;

public class ApplicantAdmissionUpdateStatusDtoValidator
    : AbstractValidator<ApplicantAdmissionUpdateStatusDto>
{
    private static readonly string[] AllowedStatuses =
    {
        "Submitted",
        "InReview",
        "Accepted",
        "Rejected"
    };

    public ApplicantAdmissionUpdateStatusDtoValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Status is required.")
            .Must(status => AllowedStatuses.Contains(status))
            .WithMessage("Invalid admission status value.");
    }
}
