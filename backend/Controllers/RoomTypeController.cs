using backend.Dtos.Request;
using backend.Service.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class RoomTypeController : ControllerBase
    {
        private readonly IRoomTypeService _roomTypeService;

        public RoomTypeController(IRoomTypeService roomTypeService)
        {
            _roomTypeService = roomTypeService;
        }


        [HttpPut("{roomTypeId}")]
        public IActionResult update(int roomTypeId, [FromBody] updateRoomTypeRequest request)
        {
            Console.WriteLine("controller roomtype and in update " + request);
            var response = _roomTypeService.updateRoomType(roomTypeId,request);

            return StatusCode(response.statusCode, response);
        }

        [HttpPost]
        public IActionResult create([FromBody] CreateRoomTypeRequest request)
        {
            var response = _roomTypeService.createNewRoomType(request);
            return StatusCode(response.statusCode, response);
        }

        [HttpDelete("{roomTypeId}")]
        public IActionResult delete( int roomTypeId)
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

        [HttpGet]
        [AllowAnonymous]
        public IActionResult getAllRoomType()
        {
            var result = _roomTypeService.getAllRoomType();
            return StatusCode(result.statusCode, result);
        }
    }
}
