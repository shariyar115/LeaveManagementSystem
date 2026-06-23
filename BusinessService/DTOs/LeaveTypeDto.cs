using System.ComponentModel.DataAnnotations;

namespace BusinessService.DTOs
{
    /// <summary>Read model for a leave type.</summary>
    public class LeaveTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DefaultDays { get; set; }
        public bool IsAccrued { get; set; }
        public double? AccrualRatePerMonth { get; set; }
    }

    /// <summary>Create / update payload for a leave type (used by HR management screen).</summary>
    public class UpsertLeaveTypeDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Range(0, 366)]
        public int DefaultDays { get; set; }

        public bool IsAccrued { get; set; }

        [Range(0, 31)]
        public double? AccrualRatePerMonth { get; set; }
    }
}
