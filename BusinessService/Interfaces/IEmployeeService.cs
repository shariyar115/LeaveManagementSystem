using BusinessService.DTOs;

namespace BusinessService.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllAsync();
    }
}
