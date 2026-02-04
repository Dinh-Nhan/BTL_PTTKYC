using backend.Service.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnpayService;
        private readonly IBookingService _bookingService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            IVnPayService vnpayService,
            IBookingService bookingService,
            ILogger<PaymentController> logger)
        {
            _vnpayService = vnpayService;
            _bookingService = bookingService;
            _logger = logger;
        }

        // Endpoint nhận callback từ VNPay
        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnpayReturn()
        {
            try
            {
                // Verify và parse payment result từ VNPay
                var paymentResult = _vnpayService.ProcessPaymentCallback(Request.Query);

                //  Xử lý cập nhật booking và room
                var result = await _bookingService.ProcessPaymentCallback(paymentResult);

                // 3. Redirect về frontend với kết quả
                // hiện tại chưa có frontend cho page success và fail nên trả về api trước
                var frontendUrl = result.statusCode == 200
                 ? $"http://localhost:5173?payment=success&bookingId={result.result.bookingId}"
                 : $"http://localhost:5173?payment=failed&message={Uri.EscapeDataString(result.message)}";

                return Redirect(frontendUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VNPay callback");
                return Redirect("http://localhost:5173/booking/error");
            }
        }

        // Endpoint cho IPN  - VNPay gọi để confirm
        [HttpGet("vnpay-ipn")]
        public async Task<IActionResult> VnpayIPN()
        {
            _logger.LogInformation("Controller Vnpay method vnpay ipn..........");
            try
            {
                var paymentResult = _vnpayService.ProcessPaymentCallback(Request.Query);

                await _bookingService.ProcessPaymentCallback(paymentResult);

                // Trả về response cho VNPay theo format yêu cầu
                return Ok(new { RspCode = "00", Message = "Confirm Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VNPay IPN");
                return Ok(new { RspCode = "99", Message = "Unknown error" });
            }
        }
    }
}
