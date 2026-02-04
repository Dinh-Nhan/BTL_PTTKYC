using AutoMapper;
using backend.Dtos.Request;
using backend.Dtos.Response;
using backend.Service.interfaces;
using System.Net;
using System.Net.Mail;
using static System.Net.Mime.MediaTypeNames;

namespace backend.Service.implementations
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;
        public EmailService(IConfiguration config, IBookingService bookingService, IMapper mapper)
        {
            _config = config;
            _bookingService = bookingService;
            _mapper = mapper;
        }

        public async Task SendOtpAsync(string toEmail, string otp)
        {
            var emailSettings = _config.GetSection("EmailSettings");

            var host = emailSettings["Host"];
            var fromEmail = emailSettings["From"];
            var displayName = emailSettings["DisplayName"];
            var password = emailSettings["Password"];
            var port = int.Parse(emailSettings["Port"]!);

            // 🛑 Check lỗi config sớm
            if (string.IsNullOrEmpty(host) ||
                string.IsNullOrEmpty(fromEmail) ||
                string.IsNullOrEmpty(password))
            {
                throw new Exception("EmailSettings configuration is missing");
            }

            var client = new SmtpClient(host, port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail, password)
            };

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, displayName),
                Subject = "Confirm Your Hotel Booking",
                Body = $"""
                <h2>English Center</h2>
                <p>Your verification code is:</p>
                <h1>{otp}</h1>
                <p>This code expires in 5 minutes.</p>
            """,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);
            await client.SendMailAsync(message);
        }

        //feature/trung
        //send url confirm email 
        public async Task SendConfirmationLinkAsync(string toEmail, string confirmationLink)
        {
            var emailSettings = _config.GetSection("EmailSettings");
            var host = emailSettings["Host"];
            var fromEmail = emailSettings["From"];
            var displayName = emailSettings["DisplayName"];
            var password = emailSettings["Password"];
            var port = int.Parse(emailSettings["Port"]!);

            if (string.IsNullOrEmpty(host) ||
                string.IsNullOrEmpty(fromEmail) ||
                string.IsNullOrEmpty(password))
            {
                throw new Exception("EmailSettings configuration is missing");
            }

            var client = new SmtpClient(host, port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail, password)
            };

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, displayName),
                Subject = "Xác nhận địa chỉ Email của bạn",
                Body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            line-height: 1.6;
                            color: #333;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                        }}
                        .header {{
                            background: linear-gradient(135deg, #ff6b35, #f7931e);
                            color: white;
                            padding: 30px;
                            text-align: center;
                            border-radius: 10px 10px 0 0;
                        }}
                        .content {{
                            background: #f9f9f9;
                            padding: 30px;
                            border-radius: 0 0 10px 10px;
                        }}
                        .button {{
                            display: inline-block;
                            padding: 15px 30px;
                            background: linear-gradient(135deg, #ff6b35, #f7931e);
                            color: white !important;
                            text-decoration: none;
                            border-radius: 8px;
                            font-weight: bold;
                            margin: 20px 0;
                        }}
                        .footer {{
                            text-align: center;
                            margin-top: 20px;
                            color: #666;
                            font-size: 12px;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Hogwart Hotel Booking</h1>
                        </div>
                        <div class='content'>
                            <h2>Xác nhận địa chỉ Email</h2>
                            <p>Xin chào,</p>
                            <p>Cảm ơn bạn đã đặt phòng tại hệ thống quản lý khách sạn Hogwart Hotel. Vui lòng nhấn vào nút bên dưới để xác nhận địa chỉ email của bạn:</p>
            
                            <div style='text-align: center;'>
                                <a href='{confirmationLink}' class='button'>
                                    Xác nhận Email
                                </a>
                            </div>
            
                            <p style='color: #666; font-size: 14px; margin-top: 20px;'>
                                Hoặc copy link sau vào trình duyệt:<br>
                                <a href='{confirmationLink}'>{confirmationLink}</a>
                            </p>
            
                            <p style='color: #999; font-size: 12px; margin-top: 30px;'>
                                Link này sẽ hết hạn sau 24 giờ.<br>
                                Nếu bạn không yêu cầu xác nhận này, vui lòng bỏ qua email.
                            </p>
                        </div>
                        <div class='footer'>
                            <p>© 2026 Hotel Booking. All rights reserved.</p>
                            <p>Email này được gửi tự động, vui lòng không reply.</p>
                        </div>
                    </div>
                </body>
                </html>
",
                IsBodyHtml = true
            };

            message.To.Add(toEmail);
            await client.SendMailAsync(message);
        }

        // gửi thông tin booking mà client vừa book
        public async Task SendBookingInfoEmailAsync(BookingEmailRequest request)
        {
            if(request == null || request.bookingId <= 0)
            {
                return;
            }
            var booking = await  _bookingService.GetBookingById(request.bookingId);

            var info = _mapper.Map<BookingResponse>(booking);

            var emailSettings = _config.GetSection("EmailSettings");

            var client = new SmtpClient(emailSettings["Host"], int.Parse(emailSettings["Port"]!))
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    emailSettings["From"],
                    emailSettings["Password"]
                )
            };

            var message = new MailMessage
            {
                From = new MailAddress(
                    emailSettings["From"]!,
                    emailSettings["DisplayName"]
                ),
                Subject = $"🛎️ Thông tin đặt phòng – Mã {info.bookingId}",
                IsBodyHtml = true,
                Body = $@"
                    <!DOCTYPE html>
                    <html>
                    <body style='font-family: Arial'>
                        <h2>Hogwart Hotel Booking</h2>

                        <p>Xin chào <b>{info.clientResponse.FullName}</b>,</p>
                        <p>Cảm ơn bạn đã đặt phòng tại <b>Hogwart Hotel</b>. Dưới đây là thông tin chi tiết:</p>

                        <table style='border-collapse: collapse; width: 100%;'>
                            <tr><td><b>Mã booking</b></td><td>{info.bookingId}</td></tr>
                            <tr><td><b>Loại phòng</b></td><td>{info.roomResponse.roomType.TypeName}</td></tr>
                            <tr><td><b>Check-in</b></td><td>{info.CheckInDatetime:dd/MM/yyyy}</td></tr>
                            <tr><td><b>Check-out</b></td><td>{info.CheckOutDatetime:dd/MM/yyyy}</td></tr>
                            <tr><td><b>Người lớn</b></td><td>{info.AdultCount}</td></tr>
                            <tr><td><b>Trẻ em</b></td><td>{info.ChildCount}</td></tr>
                            <tr><td><b>Tổng tiền</b></td><td><b>{info.DepositAmount:N0} VND</b></td></tr>
                        </table>

                        <p style='margin-top:20px'>
                            📌 Vui lòng mang theo email này khi check-in.
                        </p>

                        <p>
                            Nếu có thắc mắc, vui lòng liên hệ: <b>support@hogwarthotel.com</b>
                        </p>

                        <hr/>
                        <p style='font-size:12px;color:#777'>
                            Email này được gửi tự động – vui lòng không reply.
                        </p>
                    </body>
                    </html>"
            };

            message.To.Add(info.clientResponse.Email);
            await client.SendMailAsync(message);
        }

    }
}
