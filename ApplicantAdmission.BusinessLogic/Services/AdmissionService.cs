using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Admission;
using ApplicantAdmission.DataAccess;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.BusinessLogic.Services;

public class AdmissionService : IAdmissionService
{
    private readonly ApplicantDbContext _context;
    private readonly IMapper _mapper;

    public AdmissionService(ApplicantDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApplicantAdmissionDto?> GetByIdAsync(Guid id)
    {
        var entity = await _context.ApplicantAdmissions
            .Include(x => x.Applicant)
            .Include(x => x.Manager)
            .Include(x => x.AdmissionProgram)
            .ThenInclude(x => x.Program)
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity == null ? null : _mapper.Map<ApplicantAdmissionDto>(entity);
    }

    public async Task<List<ApplicantAdmissionDto>> GetByApplicantAsync(Guid applicantId)
    {
        var list = await _context.ApplicantAdmissions
            .Where(x => x.ApplicantId == applicantId)
            .Include(x => x.AdmissionProgram)
            .ThenInclude(x => x.Program)
            .Include(x => x.Manager)
            .ToListAsync();

        return _mapper.Map<List<ApplicantAdmissionDto>>(list);
    }
}
