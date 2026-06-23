using BusinessService.DTOs;

namespace BusinessService.Interfaces
{
    public interface IBalanceService
    {
        Task<LeaveBalanceSummaryDto> GetSummaryAsync(int employeeId);
    }
}
