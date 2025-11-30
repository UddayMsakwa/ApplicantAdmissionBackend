using FluentValidation;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Document;

namespace ApplicantAdmission.BusinessLogic.Validators;

public class DocumentDtoValidator : AbstractValidator<DocumentDto>
{
    public DocumentDtoValidator()
    {
        RuleFor(x => x.FileId).NotEmpty();
        RuleFor(x => x.DocumentKind).NotEmpty();
    }
}
