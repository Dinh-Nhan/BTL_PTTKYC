using backend.Dtos.Request;
using backend.Service.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            var response = await _userService.GetAllUser();
            return StatusCode(response.statusCode, response);
        }

        [HttpGet("search-user")]
        public async Task<IActionResult> SearchUserByFullName([FromQuery] string name)
        {
            var response = await _userService.SearchUserByFullName(name);
            return StatusCode(response.statusCode, response);
        }

        [HttpGet("search-employee")]
        public async Task<IActionResult> SearchEmployeeByFullName([FromQuery] string name)
        {
            var response = await _userService.SearchEmployeeByFullName(name);
            return StatusCode(response.statusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var response = await _userService.DeleteEmployee(id);
            return StatusCode(response.statusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest user)
        {
            var response = await _userService.CreateEmployee(user);
            return StatusCode(response.statusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeRequest user)
        {
            var response = await _userService.UpdateEmployee(id, user);
            return StatusCode(response.statusCode, response);
        }

        [HttpPut("deactive/{id}")]
        public async Task<IActionResult> DeactiveUser(int id)
        {
            var response = await _userService.DeactiveUser(id);
            return StatusCode(response.statusCode, response);
        }

        [HttpPut("active/{id}")]
        public async Task<IActionResult> ActiveUser(int id)
        {
            var response = await _userService.ActiveUser(id);
            return StatusCode(response.statusCode, response);
        }
    }
}
