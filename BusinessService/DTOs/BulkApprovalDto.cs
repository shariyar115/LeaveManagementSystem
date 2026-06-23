namespace BusinessService.DTOs
{
    /// <summary>Approve or reject several pending requests in a single operation.</summary>
    public class BulkApprovalDto
    {
        public List<int> RequestIds { get; set; } = new();
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
        public string? RejectionComment { get; set; }
    }
}
