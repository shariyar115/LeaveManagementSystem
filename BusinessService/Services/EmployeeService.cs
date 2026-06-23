using BusinessService.DTOs;
using BusinessService.Interfaces;
using BusinessService.Mapping;
using DataAccessLayer.Interfaces;

namespace BusinessService.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ILeaveRepository _repo;

        public EmployeeService(ILeaveRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            var employees = await _repo.GetEmployeesAsync();
            return employees.Select(e => e.ToDto());
        }
    }
}
