using BusinessService.DTOs;
using BusinessService.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRLeavesManagement.Controllers
{
    /// <summary>Create, query and action employee leave requests.</summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        public LeaveRequestsController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        /// <summary>Returns leave requests, optionally filtered by employee, status, type and date range.</summary>
        /// <response code="200">List of matching leave requests.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LeaveRequestDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetHistory(
            [FromQuery] int? employeeId,
            [FromQuery] LeaveStatus? status,
            [FromQuery] int? leaveTypeId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var filter = new LeaveFilterDto
            {
                EmployeeId = employeeId,
                Status = status,
                LeaveTypeId = leaveTypeId,
                FromDate = fromDate,
                ToDate = toDate
            };
            return Ok(await _leaveService.GetRequestsAsync(filter));
        }

        /// <summary>Returns all requests currently awaiting approval.</summary>
        [HttpGet("pending")]
        [ProducesResponseType(typeof(IEnumerable<LeaveRequestDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetPending()
            => Ok(await _leaveService.GetPendingAsync());

        /// <summary>Submits a new leave request after validating dates, overlaps and balance.</summary>
        /// <response code="201">The created leave request.</response>
        /// <response code="400">Validation failed (e.g. "Insufficient leave balance").</response>
        [HttpPost]
        [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LeaveRequestDto>> CreateRequest([FromBody] LeaveSubmissionDto submission)
        {
            var created = await _leaveService.SubmitLeaveAsync(submission);
            return CreatedAtAction(nameof(GetHistory), new { id = created.Id }, created);
        }

        /// <summary>Approves or rejects a single pending request. Deducts balance on approval.</summary>
        /// <response code="204">Decision applied.</response>
        /// <response code="400">Invalid status or insufficient balance.</response>
        /// <response code="404">Request not found.</response>
        [HttpPost("{id:int}/approval")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetApprovalStatus(int id, [FromBody] LeaveApprovalDto statusUpdate)
        {
            await _leaveService.ProcessApprovalAsync(id, statusUpdate);
            return NoContent();
        }

        /// <summary>Approves or rejects multiple pending requests in one call.</summary>
        [HttpPost("bulk-approval")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BulkApproval([FromBody] BulkApprovalDto dto)
        {
            await _leaveService.BulkProcessApprovalAsync(dto);
            return NoContent();
        }

        /// <summary>Cancels a request, restoring the balance if it had already been approved.</summary>
        [HttpPost("{id:int}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel(int id)
        {
            await _leaveService.CancelLeaveAsync(id);
            return NoContent();
        }
    }
}
