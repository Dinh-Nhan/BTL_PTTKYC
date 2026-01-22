using backend.Models;
using backend.Service.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace backend.Service.implementations
{
    public class VnpayService : IVnPayService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<VnpayService> _log;
        public VnpayService(IConfiguration config, ILogger<VnpayService> log)
        {
            _config = config;
            _log = log;
        }

        // tạo URL thanh toán và trả về cho frontend
        public string CreatePaymentUrl(long amount, string orderInfo, string ipAddress)
        {
            if (ipAddress == "::1") ipAddress = "127.0.0.1";

            string expireDate = DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss");

            var vnpParams = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                { "vnp_Amount", (amount * 100).ToString() },
                { "vnp_Command", "pay" },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", ipAddress },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", orderInfo },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", _config["Vnpay:ReturnUrl"]!.Trim() },
                { "vnp_TmnCode", _config["Vnpay:TmnCode"]!.Trim() },
                { "vnp_TxnRef", DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() },
                { "vnp_Version", "2.1.0" },
            };


            var hashBuilder = new StringBuilder();
            foreach (var p in vnpParams)
            {
                hashBuilder.Append(WebUtility.UrlEncode(p.Key))
                           .Append("=")
                           .Append(WebUtility.UrlEncode(p.Value))
                           .Append("&");
            }
            hashBuilder.Length--;

            string hashRaw = hashBuilder.ToString();

            string secureHash = HmacSHA512(
                _config["Vnpay:HashSecret"]!.Trim(),
                hashRaw
            );


            var query = $"{hashRaw}&vnp_SecureHash={secureHash}";

            // debug
            _log.LogInformation("HASH RAW:");
            _log.LogInformation(hashRaw);

            _log.LogInformation("SECURE HASH:");
            _log.LogInformation(secureHash);

            _log.LogInformation("QUERY URL:");
            _log.LogInformation(query);

            return $"{_config["Vnpay:BaseUrl"]}?{query}";
        }


        // xử lý callback từ VNPAY verify secure hash và trả về entity VnpayTransaction để
        // bookingService xử lý tiếp việc lưu transaction và cập nhật trạng thái thanh toán 
        public VnpayTransaction? ProcessPaymentCallback(IQueryCollection query)
        {
            var vnpParams = new SortedDictionary<string, string>(StringComparer.Ordinal);
            string vnpSecureHash = query["vnp_SecureHash"]!;

            foreach (var p in query)
            {
                if (p.Key.StartsWith("vnp_") && p.Key != "vnp_SecureHash")
                {
                    vnpParams[p.Key] = p.Value!;
                }
            }

            var hashData = new StringBuilder();
            foreach (var p in vnpParams)
            {
                hashData.Append($"{p.Key}={p.Value}&");
            }
            hashData.Length--;

            string calculatedHash = HmacSHA512(
                _config["Vnpay:HashSecret"]!,
                hashData.ToString()
            );
            // verify secure hash
            if (!calculatedHash.Equals(vnpSecureHash, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            // Parse BookingId từ vnp_OrderInfo
            string orderInfo = query["vnp_OrderInfo"].ToString();
            int bookingId = 0;
            if (!string.IsNullOrEmpty(orderInfo) && orderInfo.StartsWith("Booking"))
            {
                int.TryParse(orderInfo.Replace("Booking", ""), out bookingId);
            }

            if (query["vnp_ResponseCode"] == "00")
            {
                return new VnpayTransaction
                {
                    BookingId = bookingId,
                    VnpTxnRef = query["vnp_TxnRef"],
                    VnpTransactionNo = query["vnp_TransactionNo"],
                    VnpPayDate = query["vnp_PayDate"],
                    VnpAmount = long.Parse(query["vnp_Amount"]!) / 100,
                    VnpCurrencyCode = query["vnp_CurrCode"],
                    VnpResponseCode = query["vnp_ResponseCode"],
                    VnpTransactionStatus = query["vnp_TransactionStatus"],
                    VnpMessage = query["vnp_Message"],
                    VnpBankCode = query["vnp_BankCode"],
                    VnpCardType = query["vnp_CardType"],
                    VnpSecureHash = vnpSecureHash,
                    IsValidSignature = true,
                    PaymentStatus = "SUCCESS"
                };
            }

            return null;
        }

        

        // tạo url để truy vấn các giao dịch
        public string createQueryUrl(VnpayTransaction vnpayTransaction, string orderInfo, string ipAddress)
        {
            if (vnpayTransaction == null)
                throw new Exception("vnpay transaction is null cannot create query url");

            string requestId = GenerateRequestId();
            string transactionDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var vnpParams = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                {"vnp_RequestId", requestId },
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "querydr" },
                { "vnp_TmnCode", _config["Vnpay:TmnCode"]!.Trim() },
                { "vnp_TxnRef", vnpayTransaction.VnpTxnRef},
                { "vnp_OrderInfo", orderInfo},
                { "vnp_TransactionNo", vnpayTransaction.VnpTransactionNo},
                { "vnp_TransactionDate", transactionDate},
                { "vnp_CreateDate", transactionDate },
                { "vnp_IpAddr", ipAddress },
                
            };

            var hashBuilder = new StringBuilder();
            foreach (var p in vnpParams)
            {
                hashBuilder.Append(WebUtility.UrlEncode(p.Key))
                           .Append("=")
                           .Append(WebUtility.UrlEncode(p.Value))
                           .Append("&");
            }
            hashBuilder.Length--;

            string rawData = $"{requestId}|2.1.0|querydr|{_config["Vnpay:TmnCode"]!.Trim()}|{vnpayTransaction.VnpTxnRef}|{transactionDate}|{transactionDate}|{ipAddress}|{orderInfo}";


            string secureHash = HmacSHA512(
                _config["Vnpay:HashSecret"]!.Trim(),
                rawData
            );

            string query = $"{_config["Vnpay:querydrUrl"]!.Trim()}?{hashBuilder}&vnp_SecureHash={secureHash}";

            _log.LogInformation("Query: {query}", query);
            return query;
        }

        #region helpers method
        private string HmacSHA512(string key, string input)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        private string GenerateRequestId()
        {
            // yyyyMMddHHmmss
            string time = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);

            // Random 4 ký tự
            string random = Guid.NewGuid()
                .ToString("N")
                .Substring(0, 4)
                .ToUpper();

            return $"QRY{time}{random}";
        }
        #endregion
    }
}
