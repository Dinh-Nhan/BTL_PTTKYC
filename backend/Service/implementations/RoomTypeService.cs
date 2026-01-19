using AutoMapper;
using backend.Dtos.Request;
using backend.Dtos.Response;
using backend.Models;
using backend.Repository.interfaces;
using backend.Service.interfaces;

namespace backend.Service.implementations
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IApiResponseFactory _apiResponseFactory;
        private readonly IMapper _mapper;
        private readonly IRoomTypeRepository _roomTypeRepository;

        public RoomTypeService(IApiResponseFactory apiResponseFactory, IMapper mapper, IRoomTypeRepository roomTypeRepository)
        {
            _apiResponseFactory = apiResponseFactory;
            _mapper = mapper;
            _roomTypeRepository = roomTypeRepository;
        }

        public ApiResponse<RoomTypeResponse> createNewRoomType(CreateRoomTypeRequest roomType)
        {
            if(roomType == null)
            {
                return _apiResponseFactory.Fail<RoomTypeResponse>(
                        StatusCodes.Status400BadRequest,
                        "room is invalid!"
                    );
            }

            if(roomType.MaxAdult <= 0 || roomType.BasePrice <= 0 
                || roomType.MaxChildren <= 0 || roomType.RoomArea <= 0)
            {
                return _apiResponseFactory.Fail<RoomTypeResponse>(
                        StatusCodes.Status400BadRequest,
                        "MaxAdult and BasePrice must be greater than 0!"
                    );
            }

            var result = _roomTypeRepository.createNewRoomType(_mapper.Map<RoomType>(roomType));
            if (result != null) 
            {
                return _apiResponseFactory.Success(
                    _mapper.Map<RoomTypeResponse>(result),
                    "Create new room type successfully!"
                    
                );
            }
            else
            {
                return _apiResponseFactory.Fail<RoomTypeResponse>(
                    StatusCodes.Status500InternalServerError,
                    "Create new room type failed!"
                );
            }

        }

        public ApiResponse<bool> deleteRoomType(int roomTypeId)
        {
            if(roomTypeId <= 0)
            {
                return _apiResponseFactory.Fail<bool>(
                        StatusCodes.Status400BadRequest,
                        "roomTypeId is invalid!"
                    );
            }

            var existingRoomType = _roomTypeRepository.getByRoomTypeId(roomTypeId);

            if(existingRoomType == null)
            {
                return _apiResponseFactory.Fail<bool>(
                        StatusCodes.Status404NotFound,
                        "Room type not found!"
                    );
            }

            bool result = _roomTypeRepository.deleteRoomType(existingRoomType);

            if(result) 
                return _apiResponseFactory.Success(
                    result,
                    "Delete room type successfully!"
                    
                );
            else
                return _apiResponseFactory.Fail<bool>(
                    StatusCodes.Status500InternalServerError,
                    "Delete room type failed!"
                );
        }

        public ApiResponse<List<RoomTypeResponse>> getAllRoomType()
        {
            try
            {
                return _apiResponseFactory.Success(
                        _roomTypeRepository
                        .getAllRoomType()
                        .Select(r => _mapper.Map<RoomTypeResponse>(r))
                        .ToList()
                    );
            }catch(Exception ex)
            {
                return _apiResponseFactory.Fail<List<RoomTypeResponse>>(
                        StatusCodes.Status400BadRequest,
                        "Room Type is empty!"
                    );
            }
        }

        public ApiResponse<RoomTypeResponse> getByRoomTypeId(int roomTypeId)
        {
            if (roomTypeId <= 0)
            {
                return _apiResponseFactory.Fail<RoomTypeResponse>(
                        StatusCodes.Status400BadRequest,
                        "roomTypeId is invalid!"
                    );
            }
            var existingRoomType = _roomTypeRepository.getByRoomTypeId(roomTypeId);

            if (existingRoomType == null)
            {
                return _apiResponseFactory.Fail<RoomTypeResponse>(
                        StatusCodes.Status404NotFound,
                        "Room type not found!"
                    );
            }

            return _apiResponseFactory.Success(
                    _mapper.Map<RoomTypeResponse>(existingRoomType)
                );

        }

        public ApiResponse<RoomTypeResponse> updateRoomType(int roomTypeId, updateRoomTypeRequest request)
        {
            Console.WriteLine("object request to update " + request);

            var existingRoomType = _roomTypeRepository.getByRoomTypeId(roomTypeId);

            if (existingRoomType == null)
            {
                return _apiResponseFactory.Fail<RoomTypeResponse>(
                        StatusCodes.Status404NotFound,
                        "Room type not found!"
                    );
            }

            _mapper.Map(request, existingRoomType);

            existingRoomType.UpdatedAt = DateTime.Now;

            var result = _roomTypeRepository.updateRoomType(existingRoomType);

            if (result == null) 
            {
                return _apiResponseFactory.Fail<RoomTypeResponse>(
                    StatusCodes.Status500InternalServerError,
                    "Update room type failed!"
                    
                );
            }
            else
            {
                return _apiResponseFactory.Success(
                    _mapper.Map<RoomTypeResponse>(result),
                    "Update room type successfully!"
                );
            }
        }
    }
}
