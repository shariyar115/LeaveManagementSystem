using BusinessService.DTOs;
using BusinessService.Interfaces;
using DataAccessLayer.Interfaces;

namespace BusinessService.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly ILeaveRepository _repo;

        public BalanceService(ILeaveRepository repo)
        {
            _repo = repo;
        }

        public async Task<LeaveBalanceSummaryDto> GetSummaryAsync(int employeeId)
        {
            var employee = await _repo.GetEmployeeByIdAsync(employeeId)
                ?? throw new KeyNotFoundException("Employee not found.");

            var balances = await _repo.GetBalancesByEmployeeAsync(employeeId);

            var balanceDtos = balances.Select(b => new LeaveBalanceDto
            {
                LeaveTypeId = b.LeaveTypeId,
                LeaveTypeName = b.LeaveType?.Name ?? string.Empty,
                Balance = b.Balance,
                DefaultDays = b.LeaveType?.DefaultDays ?? 0,
                IsAccrued = b.LeaveType?.IsAccrued ?? false,
                AccruedToDate = DateCalculator.AccruedToDate(
                    employee.HireDate,
                    b.LeaveType?.IsAccrued ?? false,
                    b.LeaveType?.DefaultDays ?? 0,
                    b.LeaveType?.AccrualRatePerMonth,
                    DateTime.UtcNow)
            }).ToList();

            return new LeaveBalanceSummaryDto
            {
                EmployeeId = employee.Id,
                EmployeeName = employee.Name,
                TotalBalance = balanceDtos.Sum(b => b.Balance),
                Balances = balanceDtos
            };
        }
    }
}
