using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Applicant;
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Entities;
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
        var entity = _mapper.Map<Applicant>(dto);
        entity.Id = Guid.NewGuid();
        entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        _context.Applicants.Add(entity);
        await _context.SaveChangesAsync();

        return _mapper.Map<ApplicantDto>(entity);
    }
}
