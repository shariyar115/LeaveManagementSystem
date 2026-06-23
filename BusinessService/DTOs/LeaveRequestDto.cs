namespace BusinessService.DTOs
{
    /// <summary>Flattened, serialization-friendly view of a leave request.</summary>
    public class LeaveRequestDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysRequested { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string? RejectionComment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
