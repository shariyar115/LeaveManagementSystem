using BusinessService.DTOs;
using BusinessService.Interfaces;
using BusinessService.Mapping;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;

namespace BusinessService.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _repo;

        public LeaveService(ILeaveRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<LeaveRequestDto>> GetRequestsAsync(LeaveFilterDto filter)
        {
            var requests = await _repo.GetRequestsAsync(
                filter.EmployeeId, filter.Status, filter.LeaveTypeId, filter.FromDate, filter.ToDate);
            return requests.Select(r => r.ToDto());
        }

        public async Task<IEnumerable<LeaveRequestDto>> GetPendingAsync()
        {
            var requests = await _repo.GetPendingRequestsAsync();
            return requests.Select(r => r.ToDto());
        }

        public async Task<LeaveRequestDto> SubmitLeaveAsync(LeaveSubmissionDto dto)
        {
            // 1. Date validation
            if (dto.StartDate.Date > dto.EndDate.Date)
                throw new ArgumentException("Start date cannot be later than the end date.");

            // 2. Calculate working days
            int daysRequested = DateCalculator.CalculateBusinessDays(dto.StartDate, dto.EndDate);
            if (daysRequested <= 0)
                throw new InvalidOperationException("Leave must include at least one working day.");

            // 3. Referential checks
            if (await _repo.GetEmployeeByIdAsync(dto.EmployeeId) == null)
                throw new KeyNotFoundException("Employee not found.");
            if (await _repo.GetLeaveTypeByIdAsync(dto.LeaveTypeId) == null)
                throw new KeyNotFoundException("Leave type not found.");

            // 4. Conflict detection (overlapping pending/approved request)
            if (await _repo.HasOverlapAsync(dto.EmployeeId, dto.StartDate, dto.EndDate))
                throw new InvalidOperationException("This request overlaps an existing pending or approved leave.");

            // 5. Balance check
            var balance = await _repo.GetBalanceAsync(dto.EmployeeId, dto.LeaveTypeId);
            if (balance == null || balance.Balance < daysRequested)
                throw new InvalidOperationException("Insufficient leave balance.");

            var newRequest = new LeaveRequest
            {
                EmployeeId = dto.EmployeeId,
                LeaveTypeId = dto.LeaveTypeId,
                StartDate = dto.StartDate.Date,
                EndDate = dto.EndDate.Date,
                DaysRequested = daysRequested,
                Status = LeaveStatus.Pending,
                Reason = dto.Reason,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _repo.AddRequestAsync(newRequest);
            return saved.ToDto();
        }

        public async Task ProcessApprovalAsync(int requestId, LeaveApprovalDto approvalDto)
        {
            var request = await _repo.GetRequestByIdAsync(requestId);
            if (request == null)
                throw new KeyNotFoundException("Leave request not found.");
            if (request.Status != LeaveStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be approved or rejected.");

            await ApplyDecisionAsync(request, approvalDto.Status, approvalDto.RejectionComment);
            await _repo.UpdateRequestAsync(request);
        }

        public async Task BulkProcessApprovalAsync(BulkApprovalDto dto)
        {
            if (dto.RequestIds == null || dto.RequestIds.Count == 0)
                throw new ArgumentException("No request ids supplied.");

            var requests = (await _repo.GetRequestsByIdsAsync(dto.RequestIds)).ToList();
            if (requests.Count == 0)
                throw new KeyNotFoundException("No matching leave requests found.");

            foreach (var request in requests.Where(r => r.Status == LeaveStatus.Pending))
                await ApplyDecisionAsync(request, dto.Status, dto.RejectionComment);

            await _repo.UpdateRequestsAsync(requests);
        }

        public async Task CancelLeaveAsync(int requestId)
        {
            var request = await _repo.GetRequestByIdAsync(requestId);
            if (request == null)
                throw new KeyNotFoundException("Leave request not found.");
            if (request.Status == LeaveStatus.Cancelled)
                throw new InvalidOperationException("Request is already cancelled.");
            if (request.Status == LeaveStatus.Rejected)
                throw new InvalidOperationException("Rejected requests cannot be cancelled.");

            // Roll the balance back if it was previously deducted on approval.
            if (request.Status == LeaveStatus.Approved)
                await RestoreBalanceAsync(request);

            request.Status = LeaveStatus.Cancelled;
            await _repo.UpdateRequestAsync(request);
        }

        /// <summary>Applies an approve/reject decision to a pending request, adjusting balance on approval.</summary>
        private async Task ApplyDecisionAsync(LeaveRequest request, LeaveStatus status, string? rejectionComment)
        {
            if (status == LeaveStatus.Approved)
            {
                var balance = await _repo.GetBalanceAsync(request.EmployeeId, request.LeaveTypeId);
                if (balance == null || balance.Balance < request.DaysRequested)
                    throw new InvalidOperationException(
                        $"Cannot approve request #{request.Id}: insufficient leave balance.");

                balance.Balance -= request.DaysRequested;
                await _repo.UpdateBalanceAsync(balance);

                request.Status = LeaveStatus.Approved;
                request.RejectionComment = null;
            }
            else if (status == LeaveStatus.Rejected)
            {
                request.Status = LeaveStatus.Rejected;
                request.RejectionComment = rejectionComment;
            }
            else
            {
                throw new ArgumentException("Decision must be either Approved or Rejected.");
            }
        }

        private async Task RestoreBalanceAsync(LeaveRequest request)
        {
            var balance = await _repo.GetBalanceAsync(request.EmployeeId, request.LeaveTypeId);
            if (balance != null)
            {
                balance.Balance += request.DaysRequested;
                await _repo.UpdateBalanceAsync(balance);
            }
        }
    }
}
