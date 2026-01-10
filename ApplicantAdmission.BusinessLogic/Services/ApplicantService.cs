using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;
using ApplicantAdmission.BusinessLogic.Services;
using ApplicantAdmission.DataAccess.Enums; 
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

public class ApplicantService : IApplicantService
{
    private readonly ApplicantDbContext _context;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public ApplicantService(
        ApplicantDbContext context,
        IMapper mapper,
        INotificationService notificationService)
    {
        _context = context;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<ApplicantDto?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Applicants.FirstOrDefaultAsync(x => x.Id == id);
        return entity == null ? null : _mapper.Map<ApplicantDto>(entity);
    }

    public async Task<List<ApplicantDto>> GetAllAsync()
    {
        var list = await _context.Applicants.ToListAsync();
        return _mapper.Map<List<ApplicantDto>>(list);
    }

    public async Task<ApplicantDto> CreateAsync(ApplicantCreateDto dto)
    {
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            PasswordHash = PasswordHasher.Hash(dto.Password),
            Role = UserRole.Applicant,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var applicant = new Applicant
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            FullName = dto.FullName,
            Phone = dto.Phone,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            Citizenship = dto.Citizenship
        };

        _context.Applicants.Add(applicant);
        await _context.SaveChangesAsync();

        await _notificationService.NotifyApplicantAsync(
            applicant.Id,
            "Your applicant account has been created"
        );

        return _mapper.Map<ApplicantDto>(applicant);
    }
}
