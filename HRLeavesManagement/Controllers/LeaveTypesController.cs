using BusinessService.DTOs;
using BusinessService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRLeavesManagement.Controllers
{
    /// <summary>HR management of leave types (custom types with configurable defaults and accrual).</summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class LeaveTypesController : ControllerBase
    {
        private readonly ILeaveTypeService _service;

        public LeaveTypesController(ILeaveTypeService service)
        {
            _service = service;
        }

        /// <summary>Lists all leave types (used to populate dropdowns and the HR management grid).</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LeaveTypeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LeaveTypeDto>>> GetAll()
            => Ok(await _service.GetAllAsync());

        /// <summary>Gets a single leave type by id.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(LeaveTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LeaveTypeDto>> GetById(int id)
        {
            var type = await _service.GetByIdAsync(id);
            return type == null ? NotFound(new { error = "Leave type not found." }) : Ok(type);
        }

        /// <summary>Creates a new leave type.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(LeaveTypeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LeaveTypeDto>> Create([FromBody] UpsertLeaveTypeDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>Updates an existing leave type (including the IsAccrued toggle).</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(LeaveTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LeaveTypeDto>> Update(int id, [FromBody] UpsertLeaveTypeDto dto)
            => Ok(await _service.UpdateAsync(id, dto));

        /// <summary>Deletes a leave type that is not referenced by any request or balance.</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
