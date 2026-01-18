using ApplicantAdmission.BusinessLogic.Exceptions;
using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admin;
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Entities;
using ApplicantAdmission.DataAccess.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.BusinessLogic.Services;

public class AdminService : IAdminService
{
    private readonly ApplicantDbContext _context;
    private readonly IMapper _mapper;

    public AdminService(ApplicantDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<UserDto>> GetStaffAsync()
    {
        var users = await _context.Users.AsNoTracking().ToListAsync();
        return _mapper.Map<List<UserDto>>(users);
    }

    public async Task<UserDto> CreateStaffAsync(CreateStaffDto dto)
    {
        
        if (dto.Role == UserRole.Applicant)
            throw new BusinessRuleException("Cannot create Applicant as staff.");

        var exists = await _context.Users.AnyAsync(x => x.Email == dto.Email);
        if (exists) throw new BusinessRuleException("Email already exists.");

        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            PasswordHash = PasswordHasher.Hash(dto.Password),
            Role = dto.Role,
            IsActive = true
        };

        _context.Users.Add(user);

        
        if (dto.Role == UserRole.Manager)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
                throw new BusinessRuleException("FullName is required for Manager.");

            _context.Managers.Add(new Manager
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName!,
                Email = dto.Email,
                PasswordHash = user.PasswordHash,
                Role = "Manager"
            });
        }

        await _context.SaveChangesAsync();
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateStaffAsync(Guid id, UpdateStaffDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user == null) throw new NotFoundException("User not found.");

        if (!string.IsNullOrWhiteSpace(dto.Email))
            user.Email = dto.Email!;

        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.PasswordHash = PasswordHasher.Hash(dto.Password!);

        if (dto.Role.HasValue)
        {
            if (dto.Role.Value == UserRole.Applicant)
                throw new BusinessRuleException("Cannot change staff into Applicant.");
            user.Role = dto.Role.Value;
        }

        if (dto.IsActive.HasValue)
            user.IsActive = dto.IsActive.Value;

        
        var manager = await _context.Managers.FirstOrDefaultAsync(m => m.Email == user.Email);
        if (manager != null)
        {
            if (!string.IsNullOrWhiteSpace(dto.FullName))
                manager.FullName = dto.FullName!;
            manager.PasswordHash = user.PasswordHash;
        }

        await _context.SaveChangesAsync();
        return _mapper.Map<UserDto>(user);
    }

    public async Task DeleteStaffAsync(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user == null) throw new NotFoundException("User not found.");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}
