namespace DataAccessLayer.Models
{
    public class LeaveType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        /// <summary>Default annual allocation granted to a new balance (e.g. Maternity = 90).</summary>
        public int DefaultDays { get; set; }

        /// <summary>When true the balance accrues over time based on the employee's hire date.</summary>
        public bool IsAccrued { get; set; }

        /// <summary>
        /// Optional explicit monthly accrual rate. When null and <see cref="IsAccrued"/> is true,
        /// the rate is derived as DefaultDays / 12 (e.g. 15 vacation days => 1.25 days/month).
        /// </summary>
        public double? AccrualRatePerMonth { get; set; }
    }
}
