using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Request;
using backend.Service.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomTypeController : ControllerBase
    {
        private readonly ILogger<RoomTypeController> _logger;
        private readonly IRoomTypeService _roomTypeService;

        public RoomTypeController(ILogger<RoomTypeController> logger, IRoomTypeService roomTypeService)
        {
            _logger = logger;
            _roomTypeService = roomTypeService;
        }

        [HttpGet("{roomTypeId}")]
        public IActionResult GetRoomTypeById(int roomTypeId)
        {
            var response = _roomTypeService.getByRoomTypeId(roomTypeId);
            return StatusCode(response.statusCode, response);
        }

        [HttpDelete("{roomTypeId}")]
        public IActionResult DeleteRoomType(int roomTypeId)
        {
            var response = _roomTypeService.deleteRoomType(roomTypeId);
            return StatusCode(response.statusCode, response);
        }

        [HttpPut]
        public IActionResult UpdateRoomType([FromBody] updateRoomTypeRequest updateRoomTypeRequest)
        {
            var response = _roomTypeService.updateRoomType(updateRoomTypeRequest);
            return StatusCode(response.statusCode, response);
        }

    }
}