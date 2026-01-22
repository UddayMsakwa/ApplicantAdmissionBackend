using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;
using FluentValidation;

namespace ApplicantAdmission.BusinessLogic.Validators;

public class ApplicantAdmissionDtoValidator : AbstractValidator<ApplicantAdmissionDto>
{
    public ApplicantAdmissionDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ApplicantId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.CreatedAt).NotEmpty();

        RuleFor(x => x.Applicant).NotNull();

       
        RuleForEach(x => x.SelectedPrograms).ChildRules(sp =>
        {
            sp.RuleFor(p => p.ProgramId).NotEmpty();
            sp.RuleFor(p => p.Priority).GreaterThan(0);
            sp.RuleFor(p => p.Name).NotEmpty();
        });

        RuleFor(x => x.SelectedPrograms)
            .Must(list => list.Select(p => p.Priority).Distinct().Count() == list.Count)
            .When(x => x.SelectedPrograms != null && x.SelectedPrograms.Count > 0)
            .WithMessage("SelectedPrograms priorities must be unique.");

        RuleFor(x => x.SelectedPrograms)
            .Must(list => list.Select(p => p.ProgramId).Distinct().Count() == list.Count)
            .When(x => x.SelectedPrograms != null && x.SelectedPrograms.Count > 0)
            .WithMessage("SelectedPrograms must not contain duplicate programs.");
    }
}
