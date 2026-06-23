namespace BusinessService.DTOs
{
    /// <summary>Per leave-type balance for an employee, including accrual information.</summary>
    public class LeaveBalanceDto
    {
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;

        /// <summary>Currently available (deductible) balance.</summary>
        public double Balance { get; set; }

        /// <summary>Default annual allocation for this type.</summary>
        public int DefaultDays { get; set; }

        public bool IsAccrued { get; set; }

        /// <summary>Days accrued to date based on hire date (only meaningful when IsAccrued).</summary>
        public double AccruedToDate { get; set; }
    }

    /// <summary>Aggregate balance summary used by the dashboard summary card.</summary>
    public class LeaveBalanceSummaryDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public double TotalBalance { get; set; }
        public IEnumerable<LeaveBalanceDto> Balances { get; set; } = new List<LeaveBalanceDto>();
    }
}
