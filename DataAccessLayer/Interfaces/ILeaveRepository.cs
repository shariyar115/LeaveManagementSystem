using DataAccessLayer.Models;

namespace DataAccessLayer.Interfaces
{
    public interface ILeaveRepository
    {
        // ---- Leave requests ----
        Task<IEnumerable<LeaveRequest>> GetAllRequestsAsync();
        Task<IEnumerable<LeaveRequest>> GetRequestsAsync(
            int? employeeId, LeaveStatus? status, int? leaveTypeId, DateTime? fromDate, DateTime? toDate);
        Task<IEnumerable<LeaveRequest>> GetPendingRequestsAsync();
        Task<LeaveRequest?> GetRequestByIdAsync(int id);
        Task<IEnumerable<LeaveRequest>> GetRequestsByIdsAsync(IEnumerable<int> ids);
        Task<LeaveRequest> AddRequestAsync(LeaveRequest request);
        Task UpdateRequestAsync(LeaveRequest request);
        Task UpdateRequestsAsync(IEnumerable<LeaveRequest> requests);
        Task<bool> HasOverlapAsync(int employeeId, DateTime start, DateTime end, int? excludeRequestId = null);

        // ---- Balances ----
        Task<LeaveBalance?> GetBalanceAsync(int employeeId, int leaveTypeId);
        Task<IEnumerable<LeaveBalance>> GetBalancesByEmployeeAsync(int employeeId);
        Task UpdateBalanceAsync(LeaveBalance balance);

        // ---- Leave types ----
        Task<IEnumerable<LeaveType>> GetLeaveTypesAsync();
        Task<LeaveType?> GetLeaveTypeByIdAsync(int id);
        Task<bool> LeaveTypeNameExistsAsync(string name, int? excludeId = null);
        Task<bool> LeaveTypeInUseAsync(int leaveTypeId);
        Task<LeaveType> AddLeaveTypeAsync(LeaveType type);
        Task UpdateLeaveTypeAsync(LeaveType type);
        Task DeleteLeaveTypeAsync(LeaveType type);

        // ---- Employees ----
        Task<IEnumerable<Employee>> GetEmployeesAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
    }
}
