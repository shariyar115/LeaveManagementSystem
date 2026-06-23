using BusinessService.DTOs;

namespace BusinessService.Interfaces
{
    public interface ILeaveTypeService
    {
        Task<IEnumerable<LeaveTypeDto>> GetAllAsync();
        Task<LeaveTypeDto?> GetByIdAsync(int id);
        Task<LeaveTypeDto> CreateAsync(UpsertLeaveTypeDto dto);
        Task<LeaveTypeDto> UpdateAsync(int id, UpsertLeaveTypeDto dto);
        Task DeleteAsync(int id);
    }
}
