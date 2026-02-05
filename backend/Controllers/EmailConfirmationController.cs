using backend.Dtos.Request;
using backend.Service.implementations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailConfirmationController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly EmailConfirmationCacheService _confirmationService;
        private readonly IConfiguration _config;
        private readonly ILogger<EmailConfirmationController> _logger;

        public EmailConfirmationController(
            EmailService emailService,
            EmailConfirmationCacheService confirmationService,
            IConfiguration config,
            ILogger<EmailConfirmationController> logger
            )
        {
            _emailService = emailService;
            _confirmationService = confirmationService;
            _config = config;
            _logger = logger;
        }

        [HttpPost("send-confirmation")]
        public async Task<IActionResult> SendConfirmation([FromBody] SendEmailConfirmationRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.email))
                {
                    return BadRequest(new { message = "Email is required" });
                }

                // Token format: email|draftId|roomId
                var token = _confirmationService.GenerateConfirmationToken(
                    $"{request.email}|{request.draftId}|{request.roomId}"
                );
                var encodedToken = Uri.EscapeDataString(token);

                var backendUrl = "https://localhost:7097";
                var confirmationLink = $"{backendUrl}/api/EmailConfirmation/confirm?token={encodedToken}";

                await _emailService.SendConfirmationLinkAsync(request.email, confirmationLink);

                return Ok(new
                {
                    message = "Email sent",
                    email = request.email,
                    debug_token = token,
                    debug_link = confirmationLink
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in send-confirmation");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("confirm")]
        public IActionResult ConfirmEmail([FromQuery] string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    _logger.LogWarning("Token is null/empty");
                    return Content(GetSimpleErrorHtml("Token không hợp lệ"), "text/html");
                }

                _logger.LogInformation($"Token received: {token}");

                string decodedToken;
                try
                {
                    decodedToken = Uri.UnescapeDataString(token.Trim());
                    _logger.LogInformation($"Token decoded: {decodedToken}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error decoding token");
                    return Content(GetSimpleErrorHtml("Token không hợp lệ"), "text/html");
                }

                string? email;
                string draftId;
                string roomId;
                try
                {
                    var decoded = _confirmationService.VerifyConfirmationToken(decodedToken);
                    var parts = decoded.Split('|');
                    email = parts[0];
                    draftId = parts[1];
                    roomId = parts[2];
                    _logger.LogInformation($"Verification result: {email ?? "NULL"}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error verifying token");
                    return Content(GetSimpleErrorHtml("Lỗi xác thực"), "text/html");
                }

                if (email == null)
                {
                    _logger.LogWarning("Email is null - token not found");
                    return Content(GetSimpleErrorHtml("Token đã hết hạn hoặc không tồn tại"), "text/html");
                }

                // Redirect về trang home với params để auto-open modal
                var frontendUrl = "http://localhost:5173";
                return Redirect(
                    $"{frontendUrl}" +
                    $"?emailVerified=1" +
                    $"&draftId={draftId}" +
                    $"&roomId={roomId}" +
                    $"&email={Uri.EscapeDataString(email)}"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UNHANDLED ERROR in confirm");
                _logger.LogError($"Message: {ex.Message}");
                _logger.LogError($"StackTrace: {ex.StackTrace}");

                return Content(GetSimpleErrorHtml($"Lỗi: {ex.Message}"), "text/html");
            }
        }

        private string GetSimpleErrorHtml(string message)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <title>Lỗi</title>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        display: flex;
                        justify-content: center;
                        align-items: center;
                        min-height: 100vh;
                        margin: 0;
                        background: #f0f0f0;
                    }}
                    .box {{
                        text-align: center;
                        background: white;
                        padding: 40px;
                        border-radius: 10px;
                        box-shadow: 0 2px 10px rgba(0,0,0,0.1);
                        max-width: 500px;
                    }}
                    h1 {{ color: #dc2626; }}
                    button {{
                        background: #dc2626;
                        color: white;
                        border: none;
                        padding: 12px 30px;
                        border-radius: 5px;
                        cursor: pointer;
                        font-size: 16px;
                        margin-top: 20px;
                    }}
                </style>
            </head>
            <body>
                <div class='box'>
                    <h1>❌ Có lỗi xảy ra</h1>
                    <p>{message}</p>
                    <button onclick='window.close()'>Đóng trang này</button>
                </div>
            </body>
            </html>";
        }

        [HttpPost("send-booking-email")]
        public async Task<IActionResult> sendEmailBooking([FromBody] BookingEmailRequest request)
        {
            try
            {
                _logger.LogInformation("Send email....");
                await _emailService.SendBookingInfoEmailAsync(request);
                return Ok();
            }catch(Exception e)
            {
                _logger.LogError("Eror when send email booking for client {e}", e.Message);
                return BadRequest();
            }
        }
    }
}