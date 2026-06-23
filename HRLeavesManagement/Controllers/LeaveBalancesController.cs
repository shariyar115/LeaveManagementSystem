using BusinessService.DTOs;
using BusinessService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRLeavesManagement.Controllers
{
    /// <summary>Leave balances and accrual information per employee.</summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class LeaveBalancesController : ControllerBase
    {
        private readonly IBalanceService _balanceService;

        public LeaveBalancesController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        /// <summary>
        /// Returns the per-type balances and total balance for an employee,
        /// including days accrued to date for accruing leave types.
        /// </summary>
        /// <response code="200">Balance summary for the employee.</response>
        /// <response code="404">Employee not found.</response>
        [HttpGet]
        [ProducesResponseType(typeof(LeaveBalanceSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LeaveBalanceSummaryDto>> GetSummary([FromQuery] int employeeId)
            => Ok(await _balanceService.GetSummaryAsync(employeeId));
    }
}
