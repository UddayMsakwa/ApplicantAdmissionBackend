using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Education;
using ApplicantAdmission.DataAccess;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.BusinessLogic.Services;

public class EducationLevelService : IEducationLevelService
{
    private readonly ApplicantDbContext _context;
    private readonly IMapper _mapper;

    public EducationLevelService(ApplicantDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<EducationLevelDto>> GetAllAsync()
    {
        var levels = await _context.EducationLevels.ToListAsync();
        return _mapper.Map<List<EducationLevelDto>>(levels);
    }
}
