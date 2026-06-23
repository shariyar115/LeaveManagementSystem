

namespace BusinessService.DTOs
{
    public class LeaveApprovalDto
    {
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
        public string? RejectionComment { get; set; }
    }
}
