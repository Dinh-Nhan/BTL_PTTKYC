using backend.Dtos.Request;
using backend.Dtos.Response;
using backend.Models;

namespace backend.Service.interfaces
{
    public interface IRoomTypeService
    {
        ApiResponse<bool> updateRoomType(updateRoomTypeRequest request);
        ApiResponse<RoomTypeResponse> createNewRoomType(CreateRoomTypeRequest roomType);
    
        ApiResponse<RoomTypeResponse> getByRoomTypeId(int roomTypeId);

        ApiResponse<bool> deleteRoomType(int roomTypeId);
    }
}
