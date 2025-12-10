using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Document;
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.BusinessLogic.Services;

public class DocumentService : IDocumentService
{
    private readonly ApplicantDbContext _context;
    private readonly IMapper _mapper;

    public DocumentService(ApplicantDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DocumentDto> CreateAsync(DocumentCreateDto dto)
    {
        var entity = _mapper.Map<Document>(dto);
        entity.Id = Guid.NewGuid();

        _context.Documents.Add(entity);
        await _context.SaveChangesAsync();

        return _mapper.Map<DocumentDto>(entity);
    }

    public async Task<DocumentDto?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Documents
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity == null ? null : _mapper.Map<DocumentDto>(entity);
    }

    public async Task<List<DocumentDto>> GetByApplicantAsync(Guid applicantId)
    {
        var docs = await _context.Documents
            .Where(x => x.ApplicantId == applicantId)
            .ToListAsync();

        return _mapper.Map<List<DocumentDto>>(docs);
    }
}
