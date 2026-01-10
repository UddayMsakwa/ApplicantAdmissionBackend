using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;
using ApplicantAdmission.BusinessLogic.Models.Pagination;
using ApplicantAdmission.BusinessLogic.Exceptions;
using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Entities;
using ApplicantAdmission.DataAccess.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ApplicantAdmission.BusinessLogic.Services
{
    public class AdmissionService : IAdmissionService
    {
        private readonly ApplicantDbContext _context;
        private readonly IMapper _mapper;

        public AdmissionService(ApplicantDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private async Task<ApplicantAdmissionEntity?> LoadFullAdmission(Guid id)
        {
            return await _context.ApplicantAdmissions
                .Include(x => x.Applicant)
                .Include(x => x.Manager)
                .Include(x => x.AdmissionProgram)
                    .ThenInclude(x => x.Program)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ApplicantAdmissionDto?> GetByIdAsync(Guid id)
        {
            var entity = await LoadFullAdmission(id);
            return entity == null ? null : _mapper.Map<ApplicantAdmissionDto>(entity);
        }

        public async Task<List<ApplicantAdmissionDto>> GetByApplicantAsync(Guid applicantId)
        {
            var list = await _context.ApplicantAdmissions
                .Where(x => x.ApplicantId == applicantId)
                .Include(x => x.AdmissionProgram)
                .Include(x => x.Manager)
                .ToListAsync();

            return _mapper.Map<List<ApplicantAdmissionDto>>(list);
        }

        public async Task<List<ApplicantAdmissionDto>> GetByManagerAsync(Guid managerId)
        {
            var list = await _context.ApplicantAdmissions
                .Where(x => x.ManagerId == managerId)
                .Include(x => x.Applicant)
                .Include(x => x.AdmissionProgram)
                .ToListAsync();

            return _mapper.Map<List<ApplicantAdmissionDto>>(list);
        }

        public async Task<List<ApplicantAdmissionDto>> GetAllAsync()
        {
            var list = await _context.ApplicantAdmissions
                .Include(x => x.Applicant)
                .Include(x => x.Manager)
                .Include(x => x.AdmissionProgram)
                .ToListAsync();

            return _mapper.Map<List<ApplicantAdmissionDto>>(list);
        }

        public async Task<PagedResult<ApplicantAdmissionDto>> GetPagedAsync(int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var query = _context.ApplicantAdmissions
                .Include(x => x.Applicant)
                .Include(x => x.Manager)
                .Include(x => x.AdmissionProgram);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ApplicantAdmissionDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = _mapper.Map<List<ApplicantAdmissionDto>>(items)
            };
        }

        public async Task<ApplicantAdmissionDto> CreateAsync(ApplicantAdmissionCreateDto dto)
        {
            var entity = _mapper.Map<ApplicantAdmissionEntity>(dto);

            entity.Id = Guid.NewGuid();
            entity.Status = AdmissionStatus.Submitted;
            entity.CreatedAt = DateTime.UtcNow;

            _context.ApplicantAdmissions.Add(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ApplicantAdmissionDto>(entity);
        }

        public async Task<ApplicantAdmissionDto> AssignManagerAsync(Guid admissionId, Guid managerId)
        {
            var entity = await _context.ApplicantAdmissions.FindAsync(admissionId)
                ?? throw new NotFoundException("Admission not found.");

            if (!await _context.Managers.AnyAsync(x => x.Id == managerId))
                throw new NotFoundException("Manager not found.");

            
            if (entity.ManagerId != null)
                throw new NotFoundException("Manager already assigned"); 

            entity.ManagerId = managerId;
            await _context.SaveChangesAsync();

            return _mapper.Map<ApplicantAdmissionDto>(entity);
        }

        public async Task<ApplicantAdmissionDto> UpdateStatusAsync(Guid admissionId, AdmissionStatus status)
        {
            var entity = await _context.ApplicantAdmissions.FindAsync(admissionId)
                ?? throw new NotFoundException("Admission not found.");

           
            if (entity.Status == AdmissionStatus.Accepted ||
                entity.Status == AdmissionStatus.Rejected)
            {
                throw new NotFoundException("Final status cannot be changed"); 
            }

            entity.Status = status;
            await _context.SaveChangesAsync();

            return _mapper.Map<ApplicantAdmissionDto>(entity);
        }
    }
}