using backend.Dtos.Request;
using backend.Service.interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace backend.Controllers
{

    [ApiController]
    [Route("rooms")]
    public class RoomController : ControllerBase
    {

        private readonly ILogger<RoomController> _logger;
        private readonly IRoomService _roomService;
        private readonly IRoomTypeService _roomTypeService;
        public RoomController(ILogger<RoomController> logger, IRoomService roomService, IRoomTypeService roomTypeService)
        {
            _logger = logger;
            _roomService = roomService;
            _roomTypeService = roomTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRooms()
        {
            var response = await _roomService.GetAllRoomsAsync();

            return StatusCode(response.statusCode, response);
        }


        [HttpPost("/Deactive/{roomId}")]
        public IActionResult DeactiveRoom(int roomId)
        {
            var response = _roomService.deactiveRoom(roomId);
            return StatusCode(response.statusCode, response);
        }

        [HttpPost("/Active/{roomId}")]
        public IActionResult ActiveRoom(int roomId)
        {
            var response = _roomService.activeRoom(roomId);
            return StatusCode(response.statusCode, response);
        }

        [HttpPut]
        public IActionResult update([FromBody] updateRoomTypeRequest request)
        {
            var response = _roomTypeService.updateRoomType(request);

            return StatusCode(response.statusCode, response);
        }

        [HttpPost]
        public IActionResult create([FromBody] CreateRoomTypeRequest request)
        {
            var response = _roomTypeService.createNewRoomType(request);
            return StatusCode(response.statusCode, response);
        }

        [HttpDelete]
        public IActionResult delete([FromQuery] int roomTypeId)
        {
            var response = _roomTypeService.deleteRoomType(roomTypeId);
            return StatusCode(response.statusCode, response);
        }


        [HttpGet("{roomTypeId}")]
        public IActionResult getRoomTypeById(int roomTypeId)
        {
            var response = _roomTypeService.getByRoomTypeId(roomTypeId);
            return StatusCode(response.statusCode, response);
        }
    }
}
