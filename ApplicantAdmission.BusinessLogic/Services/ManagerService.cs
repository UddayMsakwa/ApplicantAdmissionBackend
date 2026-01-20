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

    private static bool TryParseStatus(string input, out AdmissionStatus status)
    {
        status = default;

        var normalized = input.Trim().Replace(" ", "");
        
        return Enum.TryParse(normalized, ignoreCase: true, out status);
    }


    public async Task<List<ManagerDto>> GetAllAsync()
    {
        var staff = await _context.Users
            .AsNoTracking()
            .Where(u => u.Role == UserRole.Manager || u.Role == UserRole.HeadManager)
            .ToListAsync();

        return _mapper.Map<List<ManagerDto>>(staff);
    }


    public async Task<PagedResult<ApplicantAdmissionDto>> GetApplicationsAsync(
        string? status, int page, int pageSize)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        
        var query = _context.ApplicantAdmissions
            .Include(x => x.Applicant)
            .Include(x => x.ManagerUser)
            .Include(x => x.AdmissionProgram).ThenInclude(x => x.Program)
            .AsQueryable();


        if (!string.IsNullOrWhiteSpace(status))
        {
            if (TryParseStatus(status, out var parsed))
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


    public async Task<ApplicantAdmissionDto> TakeAdmissionAsync(Guid admissionId, Guid managerUserId)
    {
        var admission = await _context.ApplicantAdmissions
            .Include(x => x.Applicant)
            .Include(x => x.ManagerUser)
            .Include(x => x.AdmissionProgram).ThenInclude(x => x.Program)
            .FirstOrDefaultAsync(x => x.Id == admissionId);

        if (admission == null)
            throw new NotFoundException("Admission not found.");

        var staffExists = await _context.Users.AnyAsync(u =>
            u.Id == managerUserId && (u.Role == UserRole.Manager || u.Role == UserRole.HeadManager || u.Role == UserRole.Admin));

        if (!staffExists)
            throw new NotFoundException("Manager not found.");

        if (admission.ManagerUserId != null && admission.ManagerUserId != managerUserId)
            throw new BusinessRuleException("Admission already owned by another manager.");

        if (admission.Status == AdmissionStatus.Closed)
            throw new BusinessRuleException("Cannot take a Closed admission.");

        admission.ManagerUserId = managerUserId;

        if (admission.Status == AdmissionStatus.Created)
            admission.Status = AdmissionStatus.UnderReview;

        await _context.SaveChangesAsync();

        return _mapper.Map<ApplicantAdmissionDto>(admission);
    }



    public async Task<ApplicantAdmissionDto> ReleaseAdmissionAsync(Guid admissionId, Guid managerUserId)
    {
        var admission = await _context.ApplicantAdmissions
            .Include(x => x.Applicant)
            .Include(x => x.ManagerUser)
            .Include(x => x.AdmissionProgram).ThenInclude(x => x.Program)
            .FirstOrDefaultAsync(x => x.Id == admissionId);

        if (admission == null)
            throw new NotFoundException("Admission not found.");

        if (admission.Status == AdmissionStatus.Closed)
            throw new BusinessRuleException("Cannot release a Closed admission.");

        if (admission.ManagerUserId != managerUserId)
            throw new BusinessRuleException("You can only release admissions you own.");

        admission.ManagerUserId = null;

        await _context.SaveChangesAsync();

        return _mapper.Map<ApplicantAdmissionDto>(admission);
    }



    public async Task<ApplicantDto> UpdateApplicantAsync(Guid applicantId, ApplicantUpdateDto dto)
    {
        var applicant = await _context.Applicants
            .Include(a => a.User)
            .FirstOrDefaultAsync(x => x.Id == applicantId);

        if (applicant == null)
            throw new NotFoundException("Applicant not found.");

        if (!string.IsNullOrWhiteSpace(dto.Citizenship))
            applicant.Citizenship = dto.Citizenship;

        if (!string.IsNullOrWhiteSpace(dto.Gender))
            applicant.Gender = dto.Gender;

        if (dto.DateOfBirth.HasValue)
            applicant.DateOfBirth = dto.DateOfBirth.Value;

        await _context.SaveChangesAsync();

        return _mapper.Map<ApplicantDto>(applicant);
    }

}
