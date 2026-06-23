using System.ComponentModel.DataAnnotations;

namespace BusinessService.DTOs
{
    public class LeaveSubmissionDto
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int LeaveTypeId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }
    }
}
