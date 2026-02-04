using backend.Data;
using backend.Models;
using backend.Repository.interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository.implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;
        public BookingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> createBooking(Booking booking)
        {
            var createdBooking = await _context.Bookings.AddAsync(booking);
            _context.SaveChanges();
            return createdBooking.Entity;
        }

        public async Task<IEnumerable<Booking>> GetAllWithDetails()
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.Client)
                .ToListAsync();
        }

        public async Task<Booking?> GetById(int bookingId)
        {
            return await _context.Bookings
                            .Include(b => b.Room)
                            .Include(b => b.Client)
                            .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }

        public async Task<Booking?> GetByIdWithDetails(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.Client)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }

        public async Task<bool> IsRoomAvailable( int roomId, DateTime checkInDatetime, DateTime checkOutDatetime)
        {
            var hasOverlap = await _context.Bookings.AnyAsync(b =>
            b.RoomId == roomId &&
            (
                b.Status == "PENDING" ||
                b.Status == "CONFIRMED" ||
                b.Status == "CHECKED_IN"
            ) &&
            b.CheckInDatetime < checkOutDatetime &&
            b.CheckOutDatetime > checkInDatetime
            );

            return !hasOverlap;
        }

        public void Update(Booking booking)
        {
            _context.Bookings.Update(booking);
        }

        public async Task<bool> UpdateDeposit(int bookingId, decimal deposti)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
            {
                return false;
            }
            booking.DepositAmount = deposti;
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusById(int bookingId, string status)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
            {
                return false;
            }
            booking.Status = status;
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
