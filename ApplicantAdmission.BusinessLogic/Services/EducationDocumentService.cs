using AutoMapper;
using ApplicantAdmission.DataAccess;
using Microsoft.EntityFrameworkCore;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Education;

public class EducationDocumentService : IEducationDocumentService
{
    private readonly ApplicantDbContext _context;
    private readonly IMapper _mapper;

    public EducationDocumentService(ApplicantDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<EducationDocumentDto>> GetByApplicantAsync(Guid applicantId)
    {
        var list = await _context.EducationDocuments
            .Where(x => x.Id == applicantId)
            .ToListAsync();

        return _mapper.Map<List<EducationDocumentDto>>(list);
    }
}
