using backend.Dtos.Request;
using backend.Service.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace backend.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {

        private readonly IRoomService _roomService;
        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRooms()
        {
            var response = await _roomService.GetAllRoomsAsync();

            return StatusCode(response.statusCode, response);
        }


        [HttpPatch("{roomId}/deactivate")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult DeactiveRoom(int roomId)
        {
            var response = _roomService.deactiveRoom(roomId);
            return StatusCode(response.statusCode, response);
        }

        [HttpPatch("{roomId}/activate")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult ActiveRoom(int roomId)
        {
            var response = _roomService.activeRoom(roomId);
            return StatusCode(response.statusCode, response);
        }

        [HttpGet("available")]
        public async Task<IActionResult> ListRoomAvailble()
        {
            var response = await _roomService.listRoomAvailable();
            return StatusCode(response.statusCode, response);
        }

        [HttpGet("search-by-date")]
        public async Task<IActionResult> roomAvailableByDate([FromQuery] AvailableRoomRequest request)
        {

            var response = await _roomService.roomAvailableByDate(request);
            return StatusCode(response.statusCode, response);
        }

        [HttpGet("{roomId}")]
        public IActionResult getById(int roomId)
        {
            var response = _roomService.getById(roomId);
            return StatusCode(response.statusCode, response);
        }

        [HttpPatch("{roomId}/status")]
        public async Task<IActionResult> ChangeStatusRoom(
            int roomId,
            [FromBody] ChangeStatusRequest request
        )
        {
            if (request == null || string.IsNullOrEmpty(request.Status))
            {
                return BadRequest("Status is required");
            }

            var allowStatus = new[]
            {
                "available",
                "occupied",
                "booked",
                "cleaning",
                "maintenance",
                "inactive"
            };

            if (!allowStatus.Contains(request.Status))
            {
                return BadRequest("Invalid status");
            }

            var response = await _roomService.ChangeStatusRoom(
                roomId,
                request.Status
            );

            return StatusCode(response.statusCode, response);
            }
        }

    }
