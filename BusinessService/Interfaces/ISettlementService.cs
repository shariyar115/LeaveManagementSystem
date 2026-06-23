using BusinessService.DTOs;

namespace BusinessService.Interfaces
{
    public interface ISettlementService
    {
        Task<LeaveBalanceDto> AdjustBalanceAsync(SettlementDto dto);
        IEnumerable<SettlementHistoryDto> GetHistory(int? employeeId);
    }
}
