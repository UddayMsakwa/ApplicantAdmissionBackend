using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Manager;
using ApplicantAdmission.DataAccess;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.BusinessLogic.Services;

public class ManagerService : IManagerService
{
    private readonly ApplicantDbContext _context;
    private readonly IMapper _mapper;

    public ManagerService(ApplicantDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ManagerDto?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Managers.FirstOrDefaultAsync(x => x.Id == id);
        return entity == null ? null : _mapper.Map<ManagerDto>(entity);
    }

    public async Task<List<ManagerDto>> GetAllAsync()
    {
        var list = await _context.Managers.ToListAsync();
        return _mapper.Map<List<ManagerDto>>(list);
    }
}
