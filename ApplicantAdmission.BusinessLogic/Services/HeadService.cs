using ApplicantAdmission.BusinessLogic.Exceptions;
using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Manager;
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Entities;
using ApplicantAdmission.DataAccess.Enums;
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
        var staff = await _context.Users
            .AsNoTracking()
            .Where(u => u.Role == UserRole.Manager || u.Role == UserRole.HeadManager)
            .ToListAsync();

        return _mapper.Map<List<ManagerDto>>(staff);
    }

    public async Task<ApplicantAdmissionDto> AssignManagerAsync(Guid admissionId, Guid managerUserId)
    {
        var admission = await _context.ApplicantAdmissions
            .Include(x => x.Applicant).ThenInclude(a => a.User)
            .Include(x => x.AdmissionPrograms).ThenInclude(ap => ap.Program)
            .FirstOrDefaultAsync(x => x.Id == admissionId);

        if (admission == null)
            throw new NotFoundException("Admission not found.");

        if (admission.ManagerUserId != null)
            throw new BusinessRuleException("Manager already assigned.");

        var manager = await _context.Users.FirstOrDefaultAsync(u =>
            u.Id == managerUserId &&
            (u.Role == UserRole.Manager || u.Role == UserRole.HeadManager));

        if (manager == null)
            throw new NotFoundException("Manager not found.");

        var applicantEmail = admission.Applicant?.User?.Email;
        if (string.IsNullOrWhiteSpace(applicantEmail))
            throw new BusinessRuleException("Applicant email missing.");

        if (string.IsNullOrWhiteSpace(manager.Email))
            throw new BusinessRuleException("Manager email missing.");

        
        admission.ManagerUserId = managerUserId;
        admission.LastModifiedAt = DateTime.UtcNow;

        var topProgramName = admission.AdmissionPrograms
            .OrderBy(x => x.Priority)
            .Select(x => x.Program.Name)
            .FirstOrDefault();

        var programInfo = string.IsNullOrWhiteSpace(topProgramName)
            ? $"admission {admission.Id}"
            : $"admission {admission.Id} ({topProgramName})";

        
        _context.Notifications.Add(new NotificationEntity
        {
            Id = Guid.NewGuid(),
            ToEmail = applicantEmail,
            Subject = "Manager assigned",
            Body =
                $"Your manager was assigned to {programInfo}.\n\n" +
                $"Manager: {manager.FullName} ({manager.Email})",
            Status = "Queued",
            CreatedAt = DateTime.UtcNow
        });

        
        _context.Notifications.Add(new NotificationEntity
        {
            Id = Guid.NewGuid(),
            ToEmail = manager.Email!,
            Subject = "New admission assigned",
            Body =
                $"You were assigned to {programInfo}.\n" +
                $"Applicant: {admission.Applicant?.User?.FullName} ({applicantEmail})",
            Status = "Queued",
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        var updated = await _context.ApplicantAdmissions
            .Include(x => x.Applicant).ThenInclude(a => a.User)
            .Include(x => x.ManagerUser)
            .Include(x => x.AdmissionPrograms).ThenInclude(ap => ap.Program)
            .FirstAsync(x => x.Id == admissionId);

        return _mapper.Map<ApplicantAdmissionDto>(updated);
    }
}
