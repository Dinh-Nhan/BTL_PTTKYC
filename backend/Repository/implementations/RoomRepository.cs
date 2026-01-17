using backend.Data;
using backend.Models;
using backend.Repository.interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository.implementations
{
    public class RoomRepository : IRoomRepository
    {
        private readonly AppDbContext _context;
        public RoomRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool activeRoom(int roomId)
        {
            var room = _context.Room.Find(roomId);

            if (room == null)
            {
                return false;
            }

            room.IsActive = true;
            _context.SaveChangesAsync();
            return true;
        }

        public  bool deactiveRoom(int roomId)
        {
            var room =  _context.Room.Find(roomId);
            
            if(room == null)
            {
                return false;
            }

            room.IsActive = false;
             _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await _context.Room
                        .Include(r => r.RoomType)   
                        .ToListAsync();
        }

        public Room? getById(int roomId)
        {
            return _context.Room.Find(roomId);
        }

        public Room updateRoom(int roomId, Room update)
        {
            return null;
        }
    }
}
