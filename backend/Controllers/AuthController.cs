using backend.Dtos.Request;
using backend.Models;
using backend.Service.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;

        public AuthController(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var response = _jwtService.GenerateToken(request);
            return StatusCode(response.statusCode, response);
        }

        [HttpPost("refresh")]
        public IActionResult RefreshToken()
        {
            var response = _jwtService.RefreshToken();
            return StatusCode(response.statusCode, response);
        }

        [HttpPost("introspect")]
        public IActionResult Introspect()
        {
            var response = _jwtService.Introspect();
            return StatusCode(response.statusCode, response);
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var response = _jwtService.Logout();
            return StatusCode(response.statusCode, response);
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var response = _jwtService.getMe();
            return StatusCode(response.statusCode, response);
        }
    }
}