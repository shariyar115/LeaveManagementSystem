using BusinessService.DTOs;

namespace BusinessService.Interfaces
{
    public interface ILeaveService
    {
        Task<IEnumerable<LeaveRequestDto>> GetRequestsAsync(LeaveFilterDto filter);
        Task<IEnumerable<LeaveRequestDto>> GetPendingAsync();
        Task<LeaveRequestDto> SubmitLeaveAsync(LeaveSubmissionDto dto);
        Task ProcessApprovalAsync(int requestId, LeaveApprovalDto approvalDto);
        Task BulkProcessApprovalAsync(BulkApprovalDto dto);
        Task CancelLeaveAsync(int requestId);
    }
}
