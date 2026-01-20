using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Auth;
using ApplicantAdmission.BusinessLogic.Exceptions;
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Entities;
using ApplicantAdmission.DataAccess.Enums;
using Microsoft.EntityFrameworkCore;


namespace ApplicantAdmission.BusinessLogic.Services;

public class AuthService : IAuthService
{
    private readonly ApplicantDbContext _context;
    private readonly JwtTokenService _jwt;

    public AuthService(ApplicantDbContext context, JwtTokenService jwt)
    {
        _context = context;
        _jwt = jwt;
    }

    public async Task<AuthResponseDto> RegisterApplicantAsync(AuthRegisterDto dto)
    {
        if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
            throw new ConflictException("User with this email already exists");

        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            FullName = dto.FullName,
            PasswordHash = PasswordHasher.Hash(dto.Password),
            Role = UserRole.Applicant,
            IsActive = true
        };

        var applicant = new Applicant
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Phone = dto.Phone
            
        };

        _context.Users.Add(user);
        _context.Applicants.Add(applicant);

        await _context.SaveChangesAsync();

        return CreateResponse(user);
    }



    public async Task<AuthResponseDto> LoginAsync(AuthLoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
        if (user == null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password");

        return CreateResponse(user);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null) throw new NotFoundException("User not found");

        if (!PasswordHasher.Verify(dto.OldPassword, user.PasswordHash))
            throw new UnauthorizedException("Old password is incorrect");

        user.PasswordHash = PasswordHasher.Hash(dto.NewPassword);
        await _context.SaveChangesAsync();
    }


    private AuthResponseDto CreateResponse(UserEntity user)
    {
        return new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role.ToString(),
            AccessToken = _jwt.Generate(user)
        };
    }
}
