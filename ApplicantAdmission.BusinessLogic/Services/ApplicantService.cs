using ApplicantAdmission.BusinessLogic.Exceptions;
using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;
using ApplicantAdmission.DataAccess;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.BusinessLogic.Services;

public class ApplicantService : IApplicantService
{
    private readonly ApplicantDbContext _context;
    private readonly IMapper _mapper;

    public ApplicantService(ApplicantDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApplicantDto?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Applicants
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id);

        return entity == null ? null : _mapper.Map<ApplicantDto>(entity);
    }

    public async Task<List<ApplicantDto>> GetAllAsync()
    {
        var list = await _context.Applicants
            .Include(a => a.User)
            .ToListAsync();

        return _mapper.Map<List<ApplicantDto>>(list);
    }

    public async Task<ApplicantDto> GetMeAsync(Guid userId)
    {
        var applicant = await _context.Applicants
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.UserId == userId);

        if (applicant == null) throw new NotFoundException("Applicant profile not found.");
        return _mapper.Map<ApplicantDto>(applicant);
    }

    public async Task<ApplicantDto> UpdateMeAsync(Guid userId, ApplicantUpdateDto dto)
    {
        var applicant = await _context.Applicants
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.UserId == userId);

        if (applicant == null) throw new NotFoundException("Applicant profile not found.");

        
        if (!string.IsNullOrWhiteSpace(dto.FullName))
            applicant.User.FullName = dto.FullName;

        
        if (!string.IsNullOrWhiteSpace(dto.Phone))
            applicant.Phone = dto.Phone;

        if (dto.DateOfBirth.HasValue)
            applicant.DateOfBirth = dto.DateOfBirth.Value;

        if (dto.Gender != null)
            applicant.Gender = dto.Gender;

        if (dto.Citizenship != null)
            applicant.Citizenship = dto.Citizenship;

        await _context.SaveChangesAsync();
        return _mapper.Map<ApplicantDto>(applicant);
    }
}
