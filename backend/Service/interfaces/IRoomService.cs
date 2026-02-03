using backend.Dtos.Request;
using backend.Dtos.Response;
using backend.Models;

namespace backend.Service.interfaces
{
    public interface IRoomService
    {
        Task<ApiResponse<List<RoomResponse>>> GetAllRoomsAsync();
        ApiResponse<bool> deactiveRoom(int roomId);
        ApiResponse<bool> activeRoom(int roomId);

        ApiResponse<Room> getById(int roomId);

        Task<ApiResponse<List<RoomResponse>>> listRoomAvailable();

        Task<ApiResponse<List<RoomResponse>>> roomAvailableByDate(AvailableRoomRequest request);
    }
}
