using backend.Dtos.Request;
using backend.Service.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingController> _logger;
        private readonly IMemoryCache _cache;

        public BookingController(IBookingService bookingService,
            ILogger<BookingController> logger,
            IMemoryCache cache
            )
        {
            _cache = cache;
            _bookingService = bookingService;
            _logger = logger;
        }

        [HttpPost("create-booking")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
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
        [AllowAnonymous]
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

        [HttpPost("draft")]
        [AllowAnonymous]
        public IActionResult CreateDraft([FromBody] BookingDraftRequest req)
        {
            var draft = new BookingDraft
            {
                Id = Guid.NewGuid().ToString(),
                RoomId = req.RoomId,
                Email = req.Email,
                FullName = req.FullName,
                PhoneNumber = req.PhoneNumber,
                CheckIn = req.CheckIn,
                CheckOut = req.CheckOut,
                Adult = req.Adult,
                Child = req.Child,
                ExpireAt = DateTime.UtcNow.AddMinutes(15)
            };

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = draft.ExpireAt
            };

            _cache.Set(draft.Id, draft, cacheOptions);

            return Ok(new
            {
                draftId = draft.Id,
                expireAt = draft.ExpireAt
            });
        }

        [HttpGet("draft/{draftId}")]
        [AllowAnonymous]
        public IActionResult GetDraft(string draftId)
        {
            if (!_cache.TryGetValue(draftId, out BookingDraft draft))
                return NotFound("Draft expired or not found");

            return Ok(draft);
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

            if (ipAddress.IsIPv4MappedToIPv6)
            {
                ipAddress = ipAddress.MapToIPv4();
            }
            else if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
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

public class BookingDraft
{
    public string Id { get; set; }
    public int RoomId { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int Adult { get; set; }
    public int Child { get; set; }
    public DateTime ExpireAt { get; set; }
}