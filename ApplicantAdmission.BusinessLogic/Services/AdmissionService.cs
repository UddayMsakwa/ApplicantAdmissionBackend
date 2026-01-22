using ApplicantAdmission.BusinessLogic.Exceptions;
using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;
using ApplicantAdmission.BusinessLogic.Models.Pagination;
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Entities;
using ApplicantAdmission.DataAccess.Enums;
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

    private IQueryable<ApplicantAdmissionEntity> BaseQuery()
        => _context.ApplicantAdmissions
            .Include(x => x.Applicant).ThenInclude(a => a.User)
            .Include(x => x.ManagerUser)
            .Include(x => x.AdmissionPrograms)
                .ThenInclude(ap => ap.Program)
                    .ThenInclude(p => p.Faculty)
            .Include(x => x.AdmissionPrograms)
                .ThenInclude(ap => ap.Program)
                    .ThenInclude(p => p.Level);

    private Task<ApplicantAdmissionEntity?> LoadFullAdmission(Guid id)
        => BaseQuery().FirstOrDefaultAsync(x => x.Id == id);

    private async Task<Applicant> GetApplicantByUserIdOrThrow(Guid userId)
    {
        var applicant = await _context.Applicants
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.UserId == userId);

        if (applicant == null)
            throw new NotFoundException("Applicant profile not found.");

        return applicant;
    }

    public async Task<ApplicantAdmissionDto?> GetByIdAsync(Guid id)
    {
        var entity = await LoadFullAdmission(id);
        return entity == null ? null : _mapper.Map<ApplicantAdmissionDto>(entity);
    }

    public async Task<List<ApplicantAdmissionDto>> GetMyAsync(Guid userId)
    {
        var applicant = await GetApplicantByUserIdOrThrow(userId);

        var list = await BaseQuery()
            .Where(x => x.ApplicantId == applicant.Id)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return _mapper.Map<List<ApplicantAdmissionDto>>(list);
    }

    public async Task<PagedResult<ApplicantAdmissionDto>> GetMyPagedAsync(Guid userId, int page, int pageSize)
    {
        var applicant = await GetApplicantByUserIdOrThrow(userId);

        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var query = BaseQuery().Where(x => x.ApplicantId == applicant.Id);

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

    public async Task<ApplicantAdmissionDto> CreateMyAsync(Guid userId, ApplicantAdmissionCreateDto dto)
    {
        var applicant = await GetApplicantByUserIdOrThrow(userId);

        var admission = new ApplicantAdmissionEntity
        {
            Id = Guid.NewGuid(),
            ApplicantId = applicant.Id,
            Status = AdmissionStatus.Created,
            CreatedAt = DateTime.UtcNow
        };

        _context.ApplicantAdmissions.Add(admission);
        await _context.SaveChangesAsync();

        var full = await LoadFullAdmission(admission.Id)
            ?? throw new NotFoundException("Admission not found.");

        return _mapper.Map<ApplicantAdmissionDto>(full);
    }

    public async Task<List<ApplicantAdmissionDto>> GetByApplicantAsync(Guid applicantId)
    {
        var applicantExists = await _context.Applicants.AnyAsync(a => a.Id == applicantId);
        if (!applicantExists)
            throw new NotFoundException("Applicant not found.");

        var list = await BaseQuery()
            .Where(x => x.ApplicantId == applicantId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return _mapper.Map<List<ApplicantAdmissionDto>>(list);
    }

    public async Task<List<ApplicantAdmissionDto>> GetByManagerAsync(Guid managerUserId)
    {
        var list = await BaseQuery()
            .Where(x => x.ManagerUserId == managerUserId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return _mapper.Map<List<ApplicantAdmissionDto>>(list);
    }

    public async Task<PagedResult<ApplicantAdmissionDto>> GetPagedAsync(int page, int pageSize)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var query = BaseQuery();

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

    public async Task<ApplicantAdmissionEntity> GetEntityForOwnershipCheckAsync(Guid admissionId)
    {
        return await _context.ApplicantAdmissions
            .FirstOrDefaultAsync(a => a.Id == admissionId)
            ?? throw new NotFoundException("Admission not found.");
    }

    public async Task AssignManagerAsync(Guid admissionId, Guid managerUserId)
    {
        var entity = await _context.ApplicantAdmissions.FindAsync(admissionId)
            ?? throw new NotFoundException("Admission not found.");

        var staffExists = await _context.Users.AnyAsync(u =>
            u.Id == managerUserId &&
            (u.Role == UserRole.Manager || u.Role == UserRole.HeadManager || u.Role == UserRole.Admin));

        if (!staffExists)
            throw new NotFoundException("Manager not found.");

        if (entity.ManagerUserId != null)
            throw new BusinessRuleException("Manager already assigned.");

        entity.ManagerUserId = managerUserId;
        await _context.SaveChangesAsync();
    }

    public async Task<ApplicantAdmissionDto> UpdateStatusAsync(Guid admissionId, AdmissionStatus status)
    {
        var entity = await _context.ApplicantAdmissions
            .Include(a => a.Applicant).ThenInclude(ap => ap.User)
            .FirstOrDefaultAsync(a => a.Id == admissionId)
            ?? throw new NotFoundException("Admission not found.");

        if (entity.Status == AdmissionStatus.Closed)
            throw new BusinessRuleException("Admission is Closed and cannot be changed.");

        if ((entity.Status == AdmissionStatus.Confirmed || entity.Status == AdmissionStatus.Rejected)
            && status != AdmissionStatus.Closed)
        {
            throw new BusinessRuleException("Only transition allowed from Confirmed/Rejected is to Closed.");
        }

        entity.Status = status;
        entity.LastModifiedAt = DateTime.UtcNow;

        
        var applicantEmail = entity.Applicant?.User?.Email;
        if (!string.IsNullOrWhiteSpace(applicantEmail))
        {
            _context.Notifications.Add(new NotificationEntity
            {
                Id = Guid.NewGuid(),
                ToEmail = applicantEmail!,
                Subject = "Admission status changed",
                Body = $"Your admission status is now: {status}",
                Status = "Queued",
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        var full = await LoadFullAdmission(entity.Id)
            ?? throw new NotFoundException("Admission not found.");

        return _mapper.Map<ApplicantAdmissionDto>(full);
    }
}
