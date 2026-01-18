using ApplicantAdmission.BusinessLogic.Exceptions;
using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Manager;
using ApplicantAdmission.BusinessLogic.Models.Pagination;
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Entities;
using ApplicantAdmission.DataAccess.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.BusinessLogic.Services;

public class ManagerService : IManagerService
{
    private readonly ApplicantDbContext _context;
    private readonly IMapper _mapper;

    public ManagerService(ApplicantDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    
    public async Task<List<ManagerDto>> GetAllAsync()
    {
        var managers = await _context.Managers.ToListAsync();
        return _mapper.Map<List<ManagerDto>>(managers);
    }

    
    public async Task<PagedResult<ApplicantAdmissionDto>> GetApplicationsAsync(
        string? status, int page, int pageSize)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var query = _context.ApplicantAdmissions
            .Include(x => x.Applicant)
            .Include(x => x.Manager)
            .Include(x => x.AdmissionProgram)
                .ThenInclude(x => x.Program)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            
            if (Enum.TryParse<AdmissionStatus>(status, true, out var parsed))
                query = query.Where(x => x.Status == parsed);
            else if (int.TryParse(status, out var num) && Enum.IsDefined(typeof(AdmissionStatus), num))
                query = query.Where(x => (int)x.Status == num);
            else
                throw new BusinessRuleException("Invalid status filter.");
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        
        return new PagedResult<ApplicantAdmissionDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = _mapper.Map<List<ApplicantAdmissionDto>>(items)
        };
    }

    
    public async Task<ApplicantDto> GetApplicantAsync(Guid applicantId)
    {
        var applicant = await _context.Applicants
            .FirstOrDefaultAsync(x => x.Id == applicantId);

        if (applicant == null)
            throw new NotFoundException("Applicant not found.");

        return _mapper.Map<ApplicantDto>(applicant);
    }

    
    public async Task<ApplicantAdmissionDto> TakeAdmissionAsync(Guid admissionId, Guid managerId)
    {
        var admission = await _context.ApplicantAdmissions
            .Include(x => x.Applicant)
            .Include(x => x.Manager)
            .Include(x => x.AdmissionProgram).ThenInclude(x => x.Program)
            .FirstOrDefaultAsync(x => x.Id == admissionId);

        if (admission == null)
            throw new NotFoundException("Admission not found.");

        
        var managerExists = await _context.Managers.AnyAsync(x => x.Id == managerId);
        if (!managerExists)
            throw new NotFoundException("Manager not found.");

        
        if (admission.ManagerId != null && admission.ManagerId != managerId)
            throw new BusinessRuleException("Admission already owned by another manager.");

        admission.ManagerId = managerId;

        
        if (admission.Status == AdmissionStatus.Submitted)
            admission.Status = AdmissionStatus.InReview;

        await _context.SaveChangesAsync();

        return _mapper.Map<ApplicantAdmissionDto>(admission);
    }

    
    public async Task<ApplicantAdmissionDto> ReleaseAdmissionAsync(Guid admissionId, Guid managerId)
    {
        var admission = await _context.ApplicantAdmissions
            .Include(x => x.Applicant)
            .Include(x => x.Manager)
            .Include(x => x.AdmissionProgram).ThenInclude(x => x.Program)
            .FirstOrDefaultAsync(x => x.Id == admissionId);

        if (admission == null)
            throw new NotFoundException("Admission not found.");

        if (admission.ManagerId != managerId)
            throw new BusinessRuleException("You can only release admissions you own.");

        admission.ManagerId = null;

        await _context.SaveChangesAsync();

        return _mapper.Map<ApplicantAdmissionDto>(admission);
    }

    
    public async Task<ApplicantDto> UpdateApplicantAsync(Guid applicantId, ApplicantUpdateDto dto)
    {
        var applicant = await _context.Applicants
            .FirstOrDefaultAsync(x => x.Id == applicantId);

        if (applicant == null)
            throw new NotFoundException("Applicant not found.");

        
        if (!string.IsNullOrWhiteSpace(dto.FullName))
            applicant.FullName = dto.FullName;

        if (!string.IsNullOrWhiteSpace(dto.Phone))
            applicant.Phone = dto.Phone;

        if (!string.IsNullOrWhiteSpace(dto.Citizenship))
            applicant.Citizenship = dto.Citizenship;

        if (dto.DateOfBirth.HasValue)
            applicant.DateOfBirth = dto.DateOfBirth.Value;

        if (!string.IsNullOrWhiteSpace(dto.Gender))
            applicant.Gender = dto.Gender;

        await _context.SaveChangesAsync();

        return _mapper.Map<ApplicantDto>(applicant);
    }
}
