using BusinessService.DTOs;
using BusinessService.Interfaces;
using DataAccessLayer.Interfaces;

namespace BusinessService.Services
{
    public class SettlementService : ISettlementService
    {
        private readonly ILeaveRepository _repo;
        private readonly ISettlementStore _store;

        public SettlementService(ILeaveRepository repo, ISettlementStore store)
        {
            _repo = repo;
            _store = store;
        }

        public async Task<LeaveBalanceDto> AdjustBalanceAsync(SettlementDto dto)
        {
            var employee = await _repo.GetEmployeeByIdAsync(dto.EmployeeId)
                ?? throw new KeyNotFoundException("Employee not found.");
            var leaveType = await _repo.GetLeaveTypeByIdAsync(dto.LeaveTypeId)
                ?? throw new KeyNotFoundException("Leave type not found.");

            var balance = await _repo.GetBalanceAsync(dto.EmployeeId, dto.LeaveTypeId)
                ?? throw new KeyNotFoundException("This employee has no balance for the selected leave type.");

            double previous = balance.Balance;
            balance.Balance = dto.NewBalance;
            await _repo.UpdateBalanceAsync(balance);

            _store.Add(new SettlementHistoryDto
            {
                EmployeeId = employee.Id,
                EmployeeName = employee.Name,
                LeaveTypeId = leaveType.Id,
                LeaveTypeName = leaveType.Name,
                PreviousBalance = previous,
                NewBalance = dto.NewBalance,
                Reason = dto.Reason,
                AdjustedAt = DateTime.UtcNow
            });

            return new LeaveBalanceDto
            {
                LeaveTypeId = leaveType.Id,
                LeaveTypeName = leaveType.Name,
                Balance = balance.Balance,
                DefaultDays = leaveType.DefaultDays,
                IsAccrued = leaveType.IsAccrued
            };
        }

        public IEnumerable<SettlementHistoryDto> GetHistory(int? employeeId)
        {
            var all = _store.GetAll().OrderByDescending(e => e.AdjustedAt);
            return employeeId.HasValue
                ? all.Where(e => e.EmployeeId == employeeId.Value)
                : all;
        }
    }
}
