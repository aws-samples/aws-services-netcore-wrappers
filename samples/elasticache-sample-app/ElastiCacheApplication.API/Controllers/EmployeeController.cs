using ElasticCacheApplication.Entities;
using ElasticCacheApplication.Service.EmployeeService;
using Microsoft.AspNetCore.Mvc;

namespace ElasticCacheApplication.API.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;

        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }


        [HttpGet]
        [Route("list")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeeList()
        {
            var response = await _employeeService.GetEmployees();
            return Ok(response);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee(Guid id)
        {
            var response = await _employeeService.GetEmployeeById(id);

            if (response == null) { 
                return NotFound("No employee found");
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("add")]
        public async Task<ActionResult<IEnumerable<Employee>>> SaveEmployee(Employee employee)
        {
            await _employeeService.AddEmployee(employee);
            return Ok();
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ActionResult> DeleteEmployee(Guid id)
        {
            var response = await _employeeService.GetEmployeeById(id);

            if (response == null)
            {
                return NotFound("No employee found");
            }
            await _employeeService.DeleteEmployee(id);
            return Ok();
        }


        [HttpDelete]
        [Route("delete/all")]
        public async Task<ActionResult> DeleteAllEmployees()
        {
            await _employeeService.DeleteAllEmployee();
            return Ok();
        }
    }

 
}
