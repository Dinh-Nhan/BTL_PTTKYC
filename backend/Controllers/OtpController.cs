using backend.Dtos.Request;
using backend.Service.implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly OtpCacheService _otpService;

        public OtpController(
            EmailService emailService,
            OtpCacheService otpService)
        {
            _emailService = emailService;
            _otpService = otpService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendOtp(SendOtpRequest request)
        {
            var otp = _otpService.GenerateOtp();

            _otpService.SaveOtp(request.Email, otp);
            await _emailService.SendOtpAsync(request.Email, otp);

            return Ok("OTP sent to email");
        }

        [HttpPost("verify")]
        public IActionResult VerifyOtp(VerifyOtpRequest request)
        {
            var isValid = _otpService.VerifyOtp(
                request.Email,
                request.Otp
            );

            if (!isValid)
                return BadRequest("Invalid or expired OTP");

            return Ok("OTP verified successfully");
        }

        //[HttpPost("Confirm-email")]

    }
}
