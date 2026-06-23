namespace BusinessService.Services
{
    /// <summary>Pure date/accrual helpers shared by the leave services.</summary>
    public static class DateCalculator
    {
        /// <summary>Number of working days (Mon-Fri) between two inclusive dates.</summary>
        public static int CalculateBusinessDays(DateTime start, DateTime end)
        {
            int days = 0;
            for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                    days++;
            }
            return days;
        }

        /// <summary>Whole months elapsed between two dates (used for accrual).</summary>
        public static int MonthsBetween(DateTime from, DateTime to)
        {
            if (to < from) return 0;
            int months = (to.Year - from.Year) * 12 + (to.Month - from.Month);
            if (to.Day < from.Day) months--;
            return Math.Max(0, months);
        }

        /// <summary>
        /// Days accrued to date for an accruing leave type, based on the employee's hire date.
        /// Rate defaults to DefaultDays / 12 when not explicitly configured.
        /// </summary>
        public static double AccruedToDate(DateTime hireDate, bool isAccrued, int defaultDays, double? ratePerMonth, DateTime asOf)
        {
            if (!isAccrued) return defaultDays;
            double rate = ratePerMonth ?? defaultDays / 12.0;
            double accrued = MonthsBetween(hireDate, asOf) * rate;
            return Math.Round(Math.Min(accrued, defaultDays), 2);
        }
    }
}
