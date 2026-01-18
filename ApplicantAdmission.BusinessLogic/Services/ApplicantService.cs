using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Entities;
using ApplicantAdmission.DataAccess.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.BusinessLogic.Services;

public class ApplicantService : IApplicantService
{
    private readonly ApplicantDbContext _context;
    private readonly IMapper _mapper;
    private readonly INotificationService _notification;

    public ApplicantService(
        ApplicantDbContext context,
        IMapper mapper,
        INotificationService notification)
    {
        _context = context;
        _mapper = mapper;
        _notification = notification;
    }

    public async Task<ApplicantDto?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Applicants.FindAsync(id);
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

        await _notification.NotifyApplicantAsync(applicant.Id, "Registration successful");

        return _mapper.Map<ApplicantDto>(applicant);
    }
}
