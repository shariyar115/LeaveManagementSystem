using BusinessService.DTOs;
using DataAccessLayer.Models;

namespace BusinessService.Mapping
{
    /// <summary>Hand-written mappers keep the API contract (DTOs) decoupled from EF entities.</summary>
    public static class LeaveMappings
    {
        public static LeaveRequestDto ToDto(this LeaveRequest r) => new()
        {
            Id = r.Id,
            EmployeeId = r.EmployeeId,
            EmployeeName = r.Employee?.Name ?? string.Empty,
            LeaveTypeId = r.LeaveTypeId,
            LeaveTypeName = r.LeaveType?.Name ?? string.Empty,
            StartDate = r.StartDate,
            EndDate = r.EndDate,
            DaysRequested = r.DaysRequested,
            Status = r.Status.ToString(),
            Reason = r.Reason,
            RejectionComment = r.RejectionComment,
            CreatedAt = r.CreatedAt
        };

        public static LeaveTypeDto ToDto(this LeaveType t) => new()
        {
            Id = t.Id,
            Name = t.Name,
            DefaultDays = t.DefaultDays,
            IsAccrued = t.IsAccrued,
            AccrualRatePerMonth = t.AccrualRatePerMonth
        };

        public static EmployeeDto ToDto(this Employee e) => new()
        {
            Id = e.Id,
            Name = e.Name,
            HireDate = e.HireDate
        };
    }
}
