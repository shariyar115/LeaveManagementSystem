using BusinessService.DTOs;
using BusinessService.Interfaces;
using BusinessService.Mapping;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;

namespace BusinessService.Services
{
    public class LeaveTypeService : ILeaveTypeService
    {
        private readonly ILeaveRepository _repo;

        public LeaveTypeService(ILeaveRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<LeaveTypeDto>> GetAllAsync()
        {
            var types = await _repo.GetLeaveTypesAsync();
            return types.Select(t => t.ToDto());
        }

        public async Task<LeaveTypeDto?> GetByIdAsync(int id)
        {
            var type = await _repo.GetLeaveTypeByIdAsync(id);
            return type?.ToDto();
        }

        public async Task<LeaveTypeDto> CreateAsync(UpsertLeaveTypeDto dto)
        {
            if (await _repo.LeaveTypeNameExistsAsync(dto.Name))
                throw new InvalidOperationException($"A leave type named '{dto.Name}' already exists.");

            var type = new LeaveType
            {
                Name = dto.Name.Trim(),
                DefaultDays = dto.DefaultDays,
                IsAccrued = dto.IsAccrued,
                AccrualRatePerMonth = dto.AccrualRatePerMonth
            };

            var created = await _repo.AddLeaveTypeAsync(type);
            return created.ToDto();
        }

        public async Task<LeaveTypeDto> UpdateAsync(int id, UpsertLeaveTypeDto dto)
        {
            var type = await _repo.GetLeaveTypeByIdAsync(id)
                ?? throw new KeyNotFoundException("Leave type not found.");

            if (await _repo.LeaveTypeNameExistsAsync(dto.Name, id))
                throw new InvalidOperationException($"A leave type named '{dto.Name}' already exists.");

            type.Name = dto.Name.Trim();
            type.DefaultDays = dto.DefaultDays;
            type.IsAccrued = dto.IsAccrued;
            type.AccrualRatePerMonth = dto.AccrualRatePerMonth;

            await _repo.UpdateLeaveTypeAsync(type);
            return type.ToDto();
        }

        public async Task DeleteAsync(int id)
        {
            var type = await _repo.GetLeaveTypeByIdAsync(id)
                ?? throw new KeyNotFoundException("Leave type not found.");

            if (await _repo.LeaveTypeInUseAsync(id))
                throw new InvalidOperationException(
                    "This leave type is in use by existing requests or balances and cannot be deleted.");

            await _repo.DeleteLeaveTypeAsync(type);
        }
    }
}
