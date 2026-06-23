namespace DataAccessLayer.Models
{
    public class LeaveBalance
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public double Balance { get; set; }

        // Navigation properties
        public LeaveType? LeaveType { get; set; }
        public Employee? Employee { get; set; }
    }
}
