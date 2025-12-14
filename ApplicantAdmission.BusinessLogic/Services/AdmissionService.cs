using ApplicantAdmission.BusinessLogic.Interfaces;
using ApplicantAdmission.BusinessLogic.Models.Dtos.Admission;
using ApplicantAdmission.BusinessLogic.Exceptions;

using ApplicantAdmission.DataAccess;
using ApplicantAdmission.DataAccess.Entities;
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
                    .ThenInclude(x => x.Program)
                .Include(x => x.Manager)
                .ToListAsync();

            return _mapper.Map<List<ApplicantAdmissionDto>>(list);
        }

        
        public async Task<ApplicantAdmissionDto> CreateAsync(ApplicantAdmissionCreateDto dto)
        {
            var entity = _mapper.Map<ApplicantAdmissionEntity>(dto);

            entity.Id = Guid.NewGuid();
            entity.Status = "Submitted";
            entity.CreatedAt = DateTime.UtcNow;

            _context.ApplicantAdmissions.Add(entity);
            await _context.SaveChangesAsync();

            var full = await LoadFullAdmission(entity.Id);
            return _mapper.Map<ApplicantAdmissionDto>(full!);
        }

        
        public async Task<ApplicantAdmissionDto> AssignManagerAsync(Guid admissionId, Guid managerId)
        {
            var entity = await _context.ApplicantAdmissions
                .FirstOrDefaultAsync(x => x.Id == admissionId);

            if (entity == null)
                throw new NotFoundException("Admission not found.");

            var managerExists = await _context.Managers
                .AnyAsync(x => x.Id == managerId);

            if (!managerExists)
                throw new NotFoundException("Manager not found.");

            entity.ManagerId = managerId;
            await _context.SaveChangesAsync();

            var full = await LoadFullAdmission(entity.Id);
            return _mapper.Map<ApplicantAdmissionDto>(full!);
        }

      
        public async Task<ApplicantAdmissionDto> UpdateStatusAsync(Guid admissionId, string status)
        {
            var entity = await _context.ApplicantAdmissions
                .FirstOrDefaultAsync(x => x.Id == admissionId);

            if (entity == null)
                throw new NotFoundException("Admission not found.");

            entity.Status = status;
            await _context.SaveChangesAsync();

            var full = await LoadFullAdmission(entity.Id);
            return _mapper.Map<ApplicantAdmissionDto>(full!);
        }
        public async Task<List<ApplicantAdmissionDto>> GetAllAsync()
        {
            var list = await _context.ApplicantAdmissions
                .Include(x => x.Applicant)
                .Include(x => x.Manager)
                .Include(x => x.AdmissionProgram)
                    .ThenInclude(x => x.Program)
                .ToListAsync();

            return _mapper.Map<List<ApplicantAdmissionDto>>(list);
        }

        public async Task<List<ApplicantAdmissionDto>> GetByManagerAsync(Guid managerId)
        {
            var list = await _context.ApplicantAdmissions
                .Where(x => x.ManagerId == managerId)
                .Include(x => x.Applicant)
                .Include(x => x.AdmissionProgram)
                    .ThenInclude(x => x.Program)
                .Include(x => x.Manager)
                .ToListAsync();

            return _mapper.Map<List<ApplicantAdmissionDto>>(list);
        }

    }
}

