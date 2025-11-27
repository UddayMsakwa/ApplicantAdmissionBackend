using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Program;
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.BusinessLogic.Services;

public class ProgramService : IProgramService
{
    private readonly ApplicantDbContext _context;
    private readonly IMapper _mapper;

    public ProgramService(ApplicantDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ProgramDto>> GetAllAsync()
    {
        var programs = await _context.Programs
            .Include(x => x.Faculty)
            .Include(x => x.Level)
            .ToListAsync();

        return _mapper.Map<List<ProgramDto>>(programs);
    }

    public async Task<ProgramDto?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Programs
            .Include(x => x.Faculty)
            .Include(x => x.Level)
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity == null ? null : _mapper.Map<ProgramDto>(entity);
    }
}
