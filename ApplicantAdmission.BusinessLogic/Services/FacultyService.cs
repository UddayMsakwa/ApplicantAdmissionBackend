using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Faculty;
using ApplicantAdmission.DataAccess;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.BusinessLogic.Services;

public class FacultyService : IFacultyService
{
    private readonly ApplicantDbContext _context;
    private readonly IMapper _mapper;

    public FacultyService(ApplicantDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<FacultyDto>> GetAllAsync()
    {
        var faculties = await _context.Faculties.ToListAsync();
        return _mapper.Map<List<FacultyDto>>(faculties);
    }
}
