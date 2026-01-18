using ApplicantAdmission.BusinessLogic.Exceptions;
using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Manager;
using ApplicantAdmission.DataAccess;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.BusinessLogic.Services;

public class HeadService : IHeadService
{
    private readonly ApplicantDbContext _context;
    private readonly IMapper _mapper;

    public HeadService(ApplicantDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ManagerDto>> GetAllManagersAsync()
    {
        var managers = await _context.Managers.AsNoTracking().ToListAsync();
        return _mapper.Map<List<ManagerDto>>(managers);
    }

    public async Task<ApplicantAdmissionDto> AssignManagerAsync(Guid admissionId, Guid managerId)
    {
        var admission = await _context.ApplicantAdmissions
            .FirstOrDefaultAsync(x => x.Id == admissionId);

        if (admission == null)
            throw new NotFoundException("Admission not found.");

        var managerExists = await _context.Managers.AnyAsync(x => x.Id == managerId);
        if (!managerExists)
            throw new NotFoundException("Manager not found.");

        admission.ManagerId = managerId;
        await _context.SaveChangesAsync();

        var full = await _context.ApplicantAdmissions
            .Include(x => x.Applicant)
            .Include(x => x.Manager)
            .Include(x => x.AdmissionProgram)
                .ThenInclude(x => x.Program)
            .FirstAsync(x => x.Id == admissionId);

        return _mapper.Map<ApplicantAdmissionDto>(full);
    }
}
