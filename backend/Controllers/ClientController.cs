using backend.Service.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetClientByFullNameOrEmailAsync([FromQuery] string information)
        {
            var response = await _clientService.GetClientByFullNameOrEmailAsync(information);
            return StatusCode(response.statusCode, response);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteClient(int id)
        {
            var response = _clientService.DeleteClient(id);
            return StatusCode(response.statusCode, response);
        }
    }
}
