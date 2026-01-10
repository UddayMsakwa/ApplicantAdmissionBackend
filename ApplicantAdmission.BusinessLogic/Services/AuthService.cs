using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Auth;
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
            throw new Exception("User already exists");

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

        return CreateResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(AuthLoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
        if (user == null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Invalid credentials");

        return CreateResponse(user);
    }

    private AuthResponseDto CreateResponse(UserEntity user)
    {
        return new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role.ToString(),
            Token = _jwt.Generate(user)
        };
    }
}
