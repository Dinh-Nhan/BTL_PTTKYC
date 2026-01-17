using backend.Data;
using backend.Models;
using backend.Repository.interfaces;

namespace backend.Repository.implementations
{
    public class RoomTypeRepository : IRoomTypeRepository
    {
        private readonly AppDbContext _context;

        public RoomTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public RoomType createNewRoomType(RoomType roomType)
        {
            _context.RoomType.Add(roomType);
            _context.SaveChanges();
            return roomType;
        }

        public bool deleteRoomType(RoomType roomType)
        {
            _context.RoomType.Remove(roomType);
            return _context.SaveChanges() > 0;
            
        }

        public RoomType? getByRoomTypeId(int roomTypeId)
        {
            return _context.RoomType.Find(roomTypeId);

        }

        public bool updateRoomType(RoomType roomType)
        {
           _context.RoomType.Update(roomType);
           return _context.SaveChanges() > 0;

        }
    }
}
