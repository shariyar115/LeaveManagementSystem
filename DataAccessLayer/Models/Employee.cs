

namespace DataAccessLayer.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }

        // Navigation property (IMPORTANT)
        public ICollection<LeaveRequest> LeaveRequests { get; set; }
            = new List<LeaveRequest>();
    }
}
