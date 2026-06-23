using System.ComponentModel.DataAnnotations;

namespace BusinessService.DTOs
{
    /// <summary>Manual balance adjustment request (e.g. annual reset of unused vacation days).</summary>
    public class SettlementDto
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int LeaveTypeId { get; set; }

        /// <summary>The new balance value to set for this employee / leave type.</summary>
        [Range(0, 1000)]
        public double NewBalance { get; set; }

        [StringLength(250)]
        public string? Reason { get; set; }
    }

    /// <summary>An entry in the (in-memory) settlement audit log.</summary>
    public class SettlementHistoryDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public double PreviousBalance { get; set; }
        public double NewBalance { get; set; }
        public string? Reason { get; set; }
        public DateTime AdjustedAt { get; set; }
    }
}
