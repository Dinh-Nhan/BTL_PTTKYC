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
            _context.RoomTypes.Add(roomType);
            _context.SaveChanges();
            return roomType;
        }

        public bool deleteRoomType(RoomType roomType)
        {
            _context.RoomTypes.Remove(roomType);
            return _context.SaveChanges() > 0;
            
        }

        public List<RoomType> getAllRoomType()
        {
            return _context.RoomTypes.ToList();
        }

        public RoomType? getByRoomTypeId(int roomTypeId)
        {
            return _context.RoomTypes.Find(roomTypeId);

        }

        public RoomType updateRoomType(RoomType roomType)
        {
           var updatedRoomType = _context.RoomTypes.Update(roomType);
           _context.SaveChanges();
           return updatedRoomType.Entity;

        }
    }
}
