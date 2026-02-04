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
        private readonly ILogger<BookingController> _logger;
        public BookingController(IBookingService bookingService, ILogger<BookingController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }


        [HttpPost("create-booking")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            // lấy địa chỉ IP của client
            var ipAddress = GetClientIPv4Address();

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


        [HttpGet("query-transaction/{bookingId}")]
        [AllowAnonymous]
        public async Task<IActionResult> QueryTransactionStatus(int bookingId)
        {
            try
            {
                var ipAddress = GetClientIPv4Address();

                _logger.LogInformation(
                    "Querying transaction status for booking {BookingId} from IP {IpAddress}",
                    bookingId,
                    ipAddress
                );

                var response = await _bookingService.QueryTransactionStatus(bookingId, ipAddress);

                return StatusCode(response.statusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in QueryTransactionStatus endpoint for booking {BookingId}", bookingId);
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Internal server error: {ex.Message}"
                });
            }
        }


        [HttpPost("refund")]
        [AllowAnonymous] // Có thể thay bằng [Authorize] nếu cần xác thực
        public async Task<IActionResult> RefundBooking([FromBody] RefundRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid request body"
                    });
                }

                var ipAddress = GetClientIPv4Address();

                _logger.LogInformation(
                    "Processing refund request for booking {BookingId}. Requested by: {RequestedBy}, Reason: {Reason}",
                    request.BookingId,
                    request.RequestedBy,
                    request.Reason
                );

                var response = await _bookingService.RefundBooking(request, ipAddress);

                return StatusCode(response.statusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RefundBooking endpoint for booking {BookingId}", request?.BookingId);
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpGet("check-refund-eligibility/{bookingId}")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckRefundEligibility(int bookingId)
        {
            try
            {
                var booking = await _bookingService.GetBookingById(bookingId);

                if (booking.statusCode != 200 || booking.result == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Booking {bookingId} not found"
                    });
                }

                var bookingData = booking.result;
                var daysUntilCheckIn = (bookingData.CheckInDatetime.Date - DateTime.UtcNow.Date).Days;

                bool isEligible = daysUntilCheckIn >= 3
                    && bookingData.Status != "REFUNDED"
                    && bookingData.Status != "CANCELLED"
                    && bookingData.Status != "CHECKED_OUT"
                    && (bookingData.PaymentStatus == "PARTIAL" || bookingData.PaymentStatus == "SUCCESS");

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        bookingId = bookingId,
                        isEligible = isEligible,
                        daysUntilCheckIn = daysUntilCheckIn,
                        currentStatus = bookingData.Status,
                        paymentStatus = bookingData.PaymentStatus,
                        message = isEligible
                            ? "This booking is eligible for 100% refund"
                            : $"This booking is not eligible for refund. Days until check-in: {daysUntilCheckIn}"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking refund eligibility for booking {BookingId}", bookingId);
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpPatch("{id}/deposit")]
        public async Task<IActionResult> UpdateDeposit(
            int id,
            [FromBody] UpdateDepositRequest request
        )
        {
                    var response = await _bookingService.UpdateDeposit(
                        id,
                        request.DepositAmount
                    );

                    return StatusCode(response.statusCode, response);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateBookingStatus(
            int id,
            [FromBody] UpdateBookingStatusRequest request
        )
        {
            var response = await _bookingService.UpdateStatusById(
                id,
                request.Status
            );
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
