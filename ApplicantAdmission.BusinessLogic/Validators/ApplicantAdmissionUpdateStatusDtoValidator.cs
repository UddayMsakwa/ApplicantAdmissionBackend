using FluentValidation;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;

public class ApplicantAdmissionUpdateStatusDtoValidator
    : AbstractValidator<ApplicantAdmissionUpdateStatusDto>
{
    private static readonly string[] AllowedStatuses =
    {
        "Submitted",
        "UnderReview",
        "Accepted",
        "Rejected"
    };

    public ApplicantAdmissionUpdateStatusDtoValidator()
    {
        RuleFor(x => x.Status)
    .IsInEnum()
    .WithMessage("Invalid admission status.");

    }
}
