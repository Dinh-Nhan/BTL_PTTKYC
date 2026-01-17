using backend.Models;

namespace backend.Repository.interfaces
{
    public interface IRoomTypeRepository
    {
        bool updateRoomType(RoomType roomType);

        RoomType createNewRoomType(RoomType roomType);

        bool deleteRoomType(RoomType roomType);

        RoomType? getByRoomTypeId(int roomTypeId);
    }
}
