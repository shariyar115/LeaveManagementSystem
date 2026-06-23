using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly LeaveDbContext _context;

        public LeaveRepository(LeaveDbContext context)
        {
            _context = context;
        }

        // ---- Leave requests ----

        public async Task<IEnumerable<LeaveRequest>> GetAllRequestsAsync() =>
            await _context.LeaveRequests
                .Include(r => r.LeaveType)
                .Include(r => r.Employee)
                .AsNoTracking()
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<LeaveRequest>> GetRequestsAsync(
            int? employeeId, LeaveStatus? status, int? leaveTypeId, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.LeaveRequests
                .Include(r => r.LeaveType)
                .Include(r => r.Employee)
                .AsNoTracking()
                .AsQueryable();

            if (employeeId.HasValue)
                query = query.Where(r => r.EmployeeId == employeeId.Value);
            if (status.HasValue)
                query = query.Where(r => r.Status == status.Value);
            if (leaveTypeId.HasValue)
                query = query.Where(r => r.LeaveTypeId == leaveTypeId.Value);
            if (fromDate.HasValue)
                query = query.Where(r => r.EndDate >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(r => r.StartDate <= toDate.Value);

            return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<LeaveRequest>> GetPendingRequestsAsync() =>
            await _context.LeaveRequests
                .Include(r => r.LeaveType)
                .Include(r => r.Employee)
                .AsNoTracking()
                .Where(r => r.Status == LeaveStatus.Pending)
                .OrderBy(r => r.StartDate)
                .ToListAsync();

        public async Task<LeaveRequest?> GetRequestByIdAsync(int id) =>
            await _context.LeaveRequests
                .Include(r => r.LeaveType)
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<IEnumerable<LeaveRequest>> GetRequestsByIdsAsync(IEnumerable<int> ids) =>
            await _context.LeaveRequests
                .Where(r => ids.Contains(r.Id))
                .ToListAsync();

        public async Task<LeaveRequest> AddRequestAsync(LeaveRequest request)
        {
            await _context.LeaveRequests.AddAsync(request);
            await _context.SaveChangesAsync();
            // reload navigation properties for a complete response
            await _context.Entry(request).Reference(r => r.LeaveType).LoadAsync();
            await _context.Entry(request).Reference(r => r.Employee).LoadAsync();
            return request;
        }

        public async Task UpdateRequestAsync(LeaveRequest request)
        {
            _context.LeaveRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRequestsAsync(IEnumerable<LeaveRequest> requests)
        {
            _context.LeaveRequests.UpdateRange(requests);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasOverlapAsync(int employeeId, DateTime start, DateTime end, int? excludeRequestId = null)
        {
            return await _context.LeaveRequests.AnyAsync(r =>
                r.EmployeeId == employeeId &&
                (excludeRequestId == null || r.Id != excludeRequestId.Value) &&
                (r.Status == LeaveStatus.Approved || r.Status == LeaveStatus.Pending) &&
                start <= r.EndDate && end >= r.StartDate);
        }

        // ---- Balances ----

        public async Task<LeaveBalance?> GetBalanceAsync(int employeeId, int leaveTypeId) =>
            await _context.LeaveBalances
                .FirstOrDefaultAsync(b => b.EmployeeId == employeeId && b.LeaveTypeId == leaveTypeId);

        public async Task<IEnumerable<LeaveBalance>> GetBalancesByEmployeeAsync(int employeeId) =>
            await _context.LeaveBalances
                .Include(b => b.LeaveType)
                .AsNoTracking()
                .Where(b => b.EmployeeId == employeeId)
                .ToListAsync();

        public async Task UpdateBalanceAsync(LeaveBalance balance)
        {
            _context.LeaveBalances.Update(balance);
            await _context.SaveChangesAsync();
        }

        // ---- Leave types ----

        public async Task<IEnumerable<LeaveType>> GetLeaveTypesAsync() =>
            await _context.LeaveTypes.AsNoTracking().OrderBy(t => t.Name).ToListAsync();

        public async Task<LeaveType?> GetLeaveTypeByIdAsync(int id) =>
            await _context.LeaveTypes.FindAsync(id);

        public async Task<bool> LeaveTypeNameExistsAsync(string name, int? excludeId = null) =>
            await _context.LeaveTypes.AnyAsync(t =>
                t.Name.ToLower() == name.ToLower() &&
                (excludeId == null || t.Id != excludeId.Value));

        public async Task<bool> LeaveTypeInUseAsync(int leaveTypeId) =>
            await _context.LeaveRequests.AnyAsync(r => r.LeaveTypeId == leaveTypeId) ||
            await _context.LeaveBalances.AnyAsync(b => b.LeaveTypeId == leaveTypeId);

        public async Task<LeaveType> AddLeaveTypeAsync(LeaveType type)
        {
            await _context.LeaveTypes.AddAsync(type);
            await _context.SaveChangesAsync();
            return type;
        }

        public async Task UpdateLeaveTypeAsync(LeaveType type)
        {
            _context.LeaveTypes.Update(type);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLeaveTypeAsync(LeaveType type)
        {
            _context.LeaveTypes.Remove(type);
            await _context.SaveChangesAsync();
        }

        // ---- Employees ----

        public async Task<IEnumerable<Employee>> GetEmployeesAsync() =>
            await _context.Employees.AsNoTracking().OrderBy(e => e.Name).ToListAsync();

        public async Task<Employee?> GetEmployeeByIdAsync(int id) =>
            await _context.Employees.FindAsync(id);
    }
}
