using backend.Models;

namespace backend.Repository.interfaces
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllRoomsAsync();

        // cập nhật trạng thái phòng thành không hoạt động
        bool deactiveRoom(int roomId);

        bool activeRoom(int roomId);

        Room? getById(int roomId);

        Room updateRoom(int roomId, Room update);
    }
}
