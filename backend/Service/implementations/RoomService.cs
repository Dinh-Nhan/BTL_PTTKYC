using AutoMapper;
using Azure;
using backend.Dtos.Response;
using backend.Mappings;
using backend.Models;
using backend.Repository.interfaces;
using backend.Service.interfaces;

namespace backend.Service.implementations
{
    public class RoomService : IRoomService
    {

        private readonly IRoomRepository _roomRepository;

        private readonly IApiResponseFactory _apiResponseFactory;

        private readonly IMapper _mapper;

        private readonly ILogger<RoomService> _logger;
        public RoomService(IRoomRepository roomRepository,
            IApiResponseFactory apiResponseFactory,
            IMapper mapper,
            ILogger<RoomService> logger)
        {
            _roomRepository = roomRepository;
            _apiResponseFactory = apiResponseFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public ApiResponse<bool> activeRoom(int roomId)
        {
            if(roomId <= 0)
            {
                return _apiResponseFactory.Fail<bool>(
                    StatusCodes.Status404NotFound,
                    "Invalid room ID"
                    );
            }

            var result = _roomRepository.activeRoom(roomId);

            if(!result)
            {
                return _apiResponseFactory.Fail<bool>(
                    StatusCodes.Status500InternalServerError,
                    "Failed to activate room"
                    );
            }

            return _apiResponseFactory.Success(true);
        }

        public ApiResponse<bool> deactiveRoom(int roomId)
        {
            if (roomId <= 0)
            {
                return _apiResponseFactory.Fail<bool>(
                    StatusCodes.Status404NotFound,
                    "Invalid room ID"
                    );
            }

            var result = _roomRepository.deactiveRoom(roomId);

            if (!result)
            {
                return _apiResponseFactory.Fail<bool>(
                    StatusCodes.Status500InternalServerError,
                    "Failed to deactivate room"
                    );
            }

            return _apiResponseFactory.Success(true);
        }

        public async Task<ApiResponse<List<RoomResponse>>> GetAllRoomsAsync()
        {
            var rooms = await _roomRepository.GetAllRoomsAsync();

            if(rooms == null || !rooms.Any())
            {
                return _apiResponseFactory.Fail<List<RoomResponse>>(
                        StatusCodes.Status404NotFound,
                        "No rooms found"
                    );
            }

            var response = _mapper.Map<List<RoomResponse>>(rooms);
           

            return _apiResponseFactory.Success(response);
        }

        public ApiResponse<Room> getById(int roomId)
        {
            if(roomId <= 0)
            {
                return _apiResponseFactory.Fail<Room>(
                        StatusCodes.Status400BadRequest,
                        "Room id must be greater than 0"
                    );
            }

            var room = _roomRepository.getById(roomId);

            if(room == null)
            {
                return _apiResponseFactory.Fail<Room>(
                        StatusCodes.Status404NotFound,
                        "Room not found"
                    );
            }

            return _apiResponseFactory.Success(room);
        }

        public async Task<ApiResponse<List<RoomResponse>>> listRoomAvailable()
        {
            try
            {
                var result = await _roomRepository.listRoomAvailable();

                var response = result.Select(r => _mapper.Map<RoomResponse>(r)).ToList();
                return _apiResponseFactory.Success(response);
            }
            catch(Exception e)
            {
                _logger.LogError("Error Message: " + e.Message);

                return _apiResponseFactory.Fail<List<RoomResponse>>(
                    StatusCodes.Status400BadRequest,
                    "An error occurred while retrieving the data!"
                );
            }
        }
    }
}
