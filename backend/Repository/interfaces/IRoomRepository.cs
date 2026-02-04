using backend.Models;

namespace backend.Repository.interfaces
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllRoomsAsync();

        // cập nhật trạng thái phòng thành không hoạt động
        bool deactiveRoom(int roomId);

        // cập nhật trạng thái phòng thành hoạt động
        bool activeRoom(int roomId);

        Room? getById(int roomId);

        Room? updateRoom(Room update);

        Room? GetByIdWithRoomType(int roomId);

        Task<IEnumerable<Room>> listRoomAvailable();

        Task<IEnumerable<Room>> RoomAvailableByDate(DateTime checkInDate,
            DateTime checkOutDate,
            int adult,
            int children);

        Task<bool> ChangeStatusRoom(int roomId, string status);
    }
}
