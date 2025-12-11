using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Education;
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.BusinessLogic.Services;

public class EducationDocumentService : IEducationDocumentService
{
    private readonly ApplicantDbContext _context;
    private readonly IMapper _mapper;

    public EducationDocumentService(ApplicantDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<EducationDocumentDto> CreateAsync(EducationDocumentCreateDto dto)
    {
        var entity = _mapper.Map<EducationDocument>(dto);
        entity.Id = Guid.NewGuid();

        
        entity.DocumentKind = "Education";

        _context.EducationDocuments.Add(entity);
        await _context.SaveChangesAsync();

        return _mapper.Map<EducationDocumentDto>(entity);
    }

    public async Task<List<EducationDocumentDto>> GetByApplicantAsync(Guid applicantId)
    {
        var docs = await _context.EducationDocuments
            .Where(x => x.ApplicantId == applicantId)
            .ToListAsync();

        return _mapper.Map<List<EducationDocumentDto>>(docs);
    }
}
