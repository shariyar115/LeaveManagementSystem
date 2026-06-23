using BusinessService.DTOs;
using BusinessService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRLeavesManagement.Controllers
{
    /// <summary>Employee lookup (single-user demo, but exposed for the UI selector).</summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeesController(IEmployeeService service)
        {
            _service = service;
        }

        /// <summary>Lists all employees.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
            => Ok(await _service.GetAllAsync());
    }
}
