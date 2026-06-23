using DataAccessLayer.Models;

namespace BusinessService.DTOs
{
    /// <summary>Optional filters for querying leave request history.</summary>
    public class LeaveFilterDto
    {
        public int? EmployeeId { get; set; }
        public LeaveStatus? Status { get; set; }
        public int? LeaveTypeId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
