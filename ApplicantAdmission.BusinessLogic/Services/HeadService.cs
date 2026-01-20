using ApplicantAdmission.BusinessLogic.Exceptions;
using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Manager;
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.BusinessLogic.Services;

public class HeadService : IHeadService
{
    private readonly ApplicantDbContext _context;
    private readonly IMapper _mapper;
    private readonly INotificationService _notification;

    public HeadService(ApplicantDbContext context, IMapper mapper, INotificationService notification)
    {
        _context = context;
        _mapper = mapper;
        _notification = notification;
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
    .Include(x => x.ManagerUser)
    .Include(x => x.AdmissionProgram).ThenInclude(x => x.Program)
    .FirstOrDefaultAsync(x => x.Id == admissionId);


        if (admission == null)
            throw new NotFoundException("Admission not found.");

        var manager = await _context.Users.FirstOrDefaultAsync(u =>
            u.Id == managerUserId && (u.Role == UserRole.Manager || u.Role == UserRole.HeadManager));

        if (manager == null)
            throw new NotFoundException("Manager not found.");

        if (admission.ManagerUserId != null)
            throw new BusinessRuleException("Manager already assigned.");

        admission.ManagerUserId = managerUserId;
        await _context.SaveChangesAsync();

        
        await _notification.NotifyApplicantAsync(
            admission.ApplicantId,
            $"A manager was assigned to your admission ({admission.AdmissionProgram.Program.Name})."
        );

        
        await _notification.NotifyEmailAsync(
            manager.Email,
            "New admission assigned",
            $"You were assigned to admission {admission.Id} for applicant {admission.Applicant.User.FullName}."
        );

        
        return _mapper.Map<ApplicantAdmissionDto>(admission);
    }
}
