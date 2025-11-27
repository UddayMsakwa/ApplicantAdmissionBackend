using FluentValidation;
using ApplicantAdmission.BusinessLogic.Models;

public class DocumentDtoValidator : AbstractValidator<DocumentDto>
{
    public DocumentDtoValidator()
    {
        RuleFor(x => x.ApplicantId).NotEmpty();
        RuleFor(x => x.FileId).NotEmpty();
        RuleFor(x => x.DocumentKind).NotEmpty();
    }
}
