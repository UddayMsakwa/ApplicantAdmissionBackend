using AutoMapper;
using ApplicantAdmission.DataAccess;
using Microsoft.EntityFrameworkCore;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Document;

public class DocumentService : IDocumentService
{
    private readonly ApplicantDbContext _context;
    private readonly IMapper _mapper;

    public DocumentService(ApplicantDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<DocumentDto>> GetByApplicantAsync(Guid applicantId)
    {
        var list = await _context.Documents
            .Where(x => x.ApplicantId == applicantId)
            .ToListAsync();

        return _mapper.Map<List<DocumentDto>>(list);
    }
}
