using BusinessService.DTOs;
using BusinessService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRLeavesManagement.Controllers
{
    /// <summary>Manual balance settlements (e.g. annual reset of unused vacation days) and their audit log.</summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SettlementsController : ControllerBase
    {
        private readonly ISettlementService _service;

        public SettlementsController(ISettlementService service)
        {
            _service = service;
        }

        /// <summary>Applies a manual balance adjustment and records it in the settlement history.</summary>
        /// <response code="200">The updated balance.</response>
        /// <response code="404">Employee, leave type or balance not found.</response>
        [HttpPost]
        [ProducesResponseType(typeof(LeaveBalanceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LeaveBalanceDto>> Adjust([FromBody] SettlementDto dto)
            => Ok(await _service.AdjustBalanceAsync(dto));

        /// <summary>Returns the (in-memory) settlement history, optionally filtered by employee.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SettlementHistoryDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<SettlementHistoryDto>> GetHistory([FromQuery] int? employeeId)
            => Ok(_service.GetHistory(employeeId));
    }
}
