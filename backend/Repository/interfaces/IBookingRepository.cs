using backend.Models;

namespace backend.Repository.interfaces
{
    public interface IBookingRepository
    {
        Task<Booking> createBooking(Booking booking);
        Task<IEnumerable<Booking>> GetAllWithDetails();
        Task<Booking?> GetById(int bookingId);
        Task<Booking?> GetByIdWithDetails(int bookingId);
        Task<bool> IsRoomAvailable(int roomId, DateTime checkInDatetime, DateTime checkOutDatetime);
        void Update(Booking booking);
        Task<bool> UpdateDeposit(int bookingId, decimal deposti);

    }
}
