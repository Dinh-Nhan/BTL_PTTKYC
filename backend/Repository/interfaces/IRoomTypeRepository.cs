using backend.Models;

namespace backend.Repository.interfaces
{
    public interface IRoomTypeRepository
    {
        RoomType updateRoomType(RoomType roomType);

        RoomType createNewRoomType(RoomType roomType);

        bool deleteRoomType(RoomType roomType);

        RoomType? getByRoomTypeId(int roomTypeId);

        List<RoomType> getAllRoomType();
    }
}
