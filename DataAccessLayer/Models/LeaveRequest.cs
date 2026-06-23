namespace DataAccessLayer.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysRequested { get; set; }
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
        public string? Reason { get; set; }
        public string? RejectionComment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Employee? Employee { get; set; }
        public LeaveType? LeaveType { get; set; }
    }
}
