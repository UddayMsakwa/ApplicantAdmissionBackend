using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Education;
using ApplicantAdmission.DataAccess;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.BusinessLogic.Services;

public class EducationDocumentTypeService : IEducationDocumentTypeService
{
    private readonly ApplicantDbContext _context;
    private readonly IMapper _mapper;

    public EducationDocumentTypeService(ApplicantDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<EducationDocumentTypeDto>> GetAllAsync()
    {
        var types = await _context.EducationDocumentTypes.ToListAsync();
        return _mapper.Map<List<EducationDocumentTypeDto>>(types);
    }
}
