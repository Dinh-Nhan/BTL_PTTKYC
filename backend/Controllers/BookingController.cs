using backend.Dtos.Request;
using backend.Service.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "ADMIN")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }


        [HttpPost("create-booking")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            // lấy địa chỉ IP của client
            var ipAddress = GetClientIPv4Address();
            Console.WriteLine("ip address: " + ipAddress);

            var response = await _bookingService.CreateBookingForClient(request, ipAddress);
            
            return StatusCode(response.statusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> getAllBooking()
        {
            var response = await _bookingService.GetAllBookings();
            return StatusCode(response.statusCode, response);
        }

        [HttpGet("{bookingId}")]
        [AllowAnonymous]
        public async Task<IActionResult> getBookingById(int bookingId)
        {
            var response = await _bookingService.GetBookingById(bookingId);
            return StatusCode(response.statusCode, response);
        }

        [HttpPatch("cancel-booking/{bookingId}")]
        public async Task<IActionResult> cancelBooking(int bookingId, CancelBookingRequest request)
        {
            var response = await _bookingService.CancelBooking(bookingId, request);
            return StatusCode(response.statusCode, response);
        }

        [HttpPost("querydr/{bookingId}")]
        public async Task<IActionResult> querydrTransaction(int bookingId)
        {
            var ipAddress = GetClientIPv4Address();
            var response = await _bookingService.createQuerydrTransaction(bookingId, ipAddress);
            
            return StatusCode(response.statusCode, response);
        }

        #region helper methods
        private string GetClientIPv4Address()
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress;

            if (ipAddress == null)
            {
                return "127.0.0.1";
            }

            // Xử lý IPv6 loopback (::1) thành IPv4 loopback
            if (ipAddress.IsIPv4MappedToIPv6)
            {
                ipAddress = ipAddress.MapToIPv4();
            }
            else if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                // Nếu là ::1 hoặc IPv6 khác
                if (System.Net.IPAddress.IsLoopback(ipAddress))
                {
                    return "127.0.0.1";
                }
            }

            return ipAddress.ToString();
        }
        #endregion
    }
}
