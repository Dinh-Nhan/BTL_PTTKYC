using backend.Dtos.Request;
using backend.Service.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "ADMIN")]
    public class RoomTypeController : ControllerBase
    {
        private readonly IRoomTypeService _roomTypeService;
        private readonly ILogger<RoomTypeController> _logger;

        public RoomTypeController(
            IRoomTypeService roomTypeService,
            ILogger<RoomTypeController> logger)
        {
            _roomTypeService = roomTypeService;
            _logger = logger;
        }

        // GET api/RoomType
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAllRoomType()
        {
            var result = _roomTypeService.getAllRoomType();
            return StatusCode(result.statusCode, result);
        }

        // GET api/RoomType/5
        [HttpGet("{roomTypeId}")]
        public IActionResult GetRoomTypeById(int roomTypeId)
        {
            var response = _roomTypeService.getByRoomTypeId(roomTypeId);
            return StatusCode(response.statusCode, response);
        }

        // POST api/RoomType
        [HttpPost]
        public IActionResult CreateRoomType([FromBody] CreateRoomTypeRequest request)
        {
            var response = _roomTypeService.createNewRoomType(request);
            return StatusCode(response.statusCode, response);
        }

        // PUT api/RoomType/5
        [HttpPut("{roomTypeId}")]
        public IActionResult UpdateRoomType(
            int roomTypeId,
            [FromBody] updateRoomTypeRequest request)
        {
            var response = _roomTypeService.updateRoomType(roomTypeId, request);
            return StatusCode(response.statusCode, response);
        }

        // DELETE api/RoomType/5
        [HttpDelete("{roomTypeId}")]
        public IActionResult DeleteRoomType(int roomTypeId)
        {
            var response = _roomTypeService.deleteRoomType(roomTypeId);
            return StatusCode(response.statusCode, response);
        }
    }
}
