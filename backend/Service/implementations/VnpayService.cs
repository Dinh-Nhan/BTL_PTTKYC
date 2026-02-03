using backend.Dtos.Request;
using backend.Dtos.Response;
using backend.Models;
using backend.Service.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace backend.Service.implementations
{
    public class VnpayService : IVnPayService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<VnpayService> _log;
        private readonly IHttpClientFactory _httpClientFactory;

        public VnpayService(
            IConfiguration config,
            ILogger<VnpayService> log,
            IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _log = log;
            _httpClientFactory = httpClientFactory;
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



        public async Task<VnpayQueryResponse> QueryTransaction(
            VnpayTransaction vnpayTransaction,
            string orderInfo,
            string ipAddress)
        {
            if (vnpayTransaction == null)
                throw new ArgumentNullException(nameof(vnpayTransaction));

            if (ipAddress == "::1") ipAddress = "127.0.0.1";

            string requestId = GenerateRequestId();
            string createDate = DateTime.Now.ToString("yyyyMMddHHmmss");

            string transactionDate = ParseVnpPayDate(vnpayTransaction.VnpPayDate);

            string tmnCode = _config["Vnpay:TmnCode"]!.Trim();
            string hashSecret = _config["Vnpay:HashSecret"]!.Trim();

            string rawData = string.Join("|",
                requestId,
                "2.1.0",
                "querydr",
                tmnCode,
                vnpayTransaction.VnpTxnRef,
                transactionDate,
                createDate,
                ipAddress,
                orderInfo
            );

            _log.LogInformation("QueryDR Raw data: {rawData}", rawData);

            string secureHash = HmacSHA512(hashSecret, rawData);

            var queryRequest = new VnpayQueryRequest
            {
                vnp_RequestId = requestId,
                vnp_Version = "2.1.0",
                vnp_Command = "querydr",
                vnp_TmnCode = tmnCode,
                vnp_TxnRef = vnpayTransaction.VnpTxnRef,
                vnp_OrderInfo = orderInfo,
                vnp_TransactionNo = vnpayTransaction.VnpTransactionNo,
                vnp_TransactionDate = transactionDate,
                vnp_CreateDate = createDate,
                vnp_IpAddr = ipAddress,
                vnp_SecureHash = secureHash
            };


            string apiUrl = _config["Vnpay:querydrUrl"]!.Trim();

            var httpClient = _httpClientFactory.CreateClient();

            var jsonContent = JsonSerializer.Serialize(queryRequest);
            _log.LogInformation("===== QUERYDR REQUEST =====");
            _log.LogInformation("URL: {url}", apiUrl);
            _log.LogInformation("Request Body: {json}", jsonContent);

            using var content = new StringContent(
                jsonContent,
                Encoding.UTF8,
                "application/json"
            );

            var response = await httpClient.PostAsync(apiUrl, content);

            var responseContent = await response.Content.ReadAsStringAsync();

            _log.LogInformation("===== QUERYDR RESPONSE =====");
            _log.LogInformation("StatusCode: {status}", response.StatusCode);
            _log.LogInformation("Response Body: {body}", responseContent);

            if (responseContent.TrimStart().StartsWith("<"))
            {
                _log.LogError("VNPay returned HTML instead of JSON. Possible authentication or URL error.");
                throw new InvalidOperationException($"VNPay API error. Response: {responseContent.Substring(0, Math.Min(500, responseContent.Length))}");
            }

            var vnpayResponse = JsonSerializer.Deserialize<VnpayQueryResponse>(responseContent);

            if (!VerifyQueryResponseHash(vnpayResponse, hashSecret))
            {
                throw new InvalidOperationException("Invalid response hash from VNPay");
            }

            return vnpayResponse;
        }

        public async Task<VnpayRefundResponse> RefundTransaction(
            VnpayTransaction vnpayTransaction,
            string orderInfo,
            string createdBy,
            string ipAddress)
        {
            if (vnpayTransaction == null)
                throw new ArgumentNullException(nameof(vnpayTransaction));

            if (ipAddress == "::1") ipAddress = "127.0.0.1";

            string requestId = GenerateRequestId();
            string createDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            string transactionDate = ParseVnpPayDate(vnpayTransaction.VnpPayDate);

            string tmnCode = _config["Vnpay:TmnCode"]!.Trim();
            string hashSecret = _config["Vnpay:HashSecret"]!.Trim();
            string amount = (vnpayTransaction.VnpAmount * 100).ToString();

            string sanitizedOrderInfo = SanitizeOrderInfo(orderInfo);


            string rawData = $"{requestId}|2.1.0|refund|{tmnCode}|02|{vnpayTransaction.VnpTxnRef}|{amount}|{vnpayTransaction.VnpTransactionNo}|{transactionDate}|{createdBy}|{createDate}|{ipAddress}|{sanitizedOrderInfo}";

            _log.LogInformation("Raw Data for Hash    :");
            _log.LogInformation("{rawData}", rawData);
            _log.LogInformation("───────────────────────────────────────────────────────────────");

            string secureHash = HmacSHA512(hashSecret, rawData);

            _log.LogInformation("SecureHash (HMAC512) : {hash}", secureHash);
            _log.LogInformation("Hash Secret Length   : {length} chars", hashSecret.Length);

            var refundRequest = new VnpayRefundRequest
            {
                vnp_RequestId = requestId,
                vnp_Version = "2.1.0",
                vnp_Command = "refund",
                vnp_TmnCode = tmnCode,
                vnp_TransactionType = "02",
                vnp_TxnRef = vnpayTransaction.VnpTxnRef,
                vnp_Amount = amount,
                vnp_OrderInfo = sanitizedOrderInfo,
                vnp_TransactionNo = vnpayTransaction.VnpTransactionNo,
                vnp_TransactionDate = transactionDate,
                vnp_CreateBy = createdBy,
                vnp_CreateDate = createDate,
                vnp_IpAddr = ipAddress,
                vnp_SecureHash = secureHash
            };

            string apiUrl = _config["Vnpay:querydrUrl"]!.Trim();

            _log.LogInformation("╔══════════════════════════════════════════════════════════════╗");
            _log.LogInformation("║                 SENDING REQUEST TO VNPAY                     ║");
            _log.LogInformation("╚══════════════════════════════════════════════════════════════╝");
            _log.LogInformation("API URL: {url}", apiUrl);

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            var jsonContent = JsonSerializer.Serialize(refundRequest, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            _log.LogInformation("Request JSON:");
            _log.LogInformation("{json}", jsonContent);

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync(apiUrl, content); 

                var responseContent = await response.Content.ReadAsStringAsync();

                // Kiểm tra nếu là HTML
                if (responseContent.TrimStart().StartsWith("<"))
                {
                    throw new InvalidOperationException("VNPay returned HTML instead of JSON");
                }

                var vnpayResponse = JsonSerializer.Deserialize<VnpayRefundResponse>(responseContent);

                _log.LogInformation("✓ Parsed VNPay Response:");
                _log.LogInformation("  ResponseCode       : {code}", vnpayResponse?.vnp_ResponseCode);
                _log.LogInformation("  Message            : {msg}", vnpayResponse?.vnp_Message);
                _log.LogInformation("  TransactionStatus  : {status}", vnpayResponse?.vnp_TransactionStatus);
                _log.LogInformation("  TransactionNo      : {txnNo}", vnpayResponse?.vnp_TransactionNo);

                // Verify response hash
                if (!VerifyRefundResponseHash(vnpayResponse, hashSecret))
                {
                    _log.LogError("❌ ERROR: Invalid response hash from VNPay!");
                    throw new InvalidOperationException("Invalid response hash from VNPay");
                }

                _log.LogInformation("✓ Response hash verified successfully");

                return vnpayResponse;
            }
            catch (HttpRequestException ex)
            {
                _log.LogError(ex, "HTTP Request Exception");
                throw new InvalidOperationException($"Cannot connect to VNPay: {ex.Message}", ex);
            }
            catch (TaskCanceledException ex)
            {
                _log.LogError(ex, "Request Timeout");
                throw new InvalidOperationException("VNPay request timeout after 30 seconds", ex);
            }
            catch (JsonException ex)
            {
                _log.LogError(ex, "JSON Parsing Error");
                throw new InvalidOperationException($"Cannot parse VNPay response: {ex.Message}", ex);
            }
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
            string time = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);

            string random = Guid.NewGuid()
                .ToString("N")
                .Substring(0, 4)
                .ToUpper();

            return $"QRY{time}{random}";
        }

        private string ParseVnpPayDate(string vnpPayDate)
        {
            if (string.IsNullOrEmpty(vnpPayDate) || vnpPayDate.Length != 14)
            {
                throw new ArgumentException("Invalid VnpPayDate format");
            }
            return vnpPayDate;
        }

        private bool VerifyQueryResponseHash(VnpayQueryResponse response, string hashSecret)
        {
            if (response == null || string.IsNullOrEmpty(response.vnp_SecureHash))
                return false;

            string rawData = $"{response.vnp_ResponseId}|{response.vnp_Command}|{response.vnp_ResponseCode}|{response.vnp_Message}|{response.vnp_TmnCode}|{response.vnp_TxnRef}|{response.vnp_Amount}|{response.vnp_BankCode}|{response.vnp_PayDate}|{response.vnp_TransactionNo}|{response.vnp_TransactionType}|{response.vnp_TransactionStatus}|{response.vnp_OrderInfo}|{response.vnp_PromotionCode ?? ""}|{response.vnp_PromotionAmount ?? ""}";

            string calculatedHash = HmacSHA512(hashSecret, rawData);

            return calculatedHash.Equals(response.vnp_SecureHash, StringComparison.OrdinalIgnoreCase);
        }

        private bool VerifyRefundResponseHash(VnpayRefundResponse response, string hashSecret)
        {
            if (response == null)
                return false;

            string receivedHash = response.vnp_SecureHash;

            string rawData = $"{response.vnp_ResponseId}|{response.vnp_Command}|{response.vnp_ResponseCode}|{response.vnp_Message}|{response.vnp_TmnCode}|{response.vnp_TxnRef}|{response.vnp_Amount}|{response.vnp_BankCode}|{response.vnp_OrderInfo}";

            string calculatedHash = HmacSHA512(hashSecret, rawData);

            _log.LogInformation("Hash Verification:");
            _log.LogInformation("  Response RawData: {rawData}", rawData);
            _log.LogInformation("  Received Hash   : {received}", receivedHash);
            _log.LogInformation("  Calculated Hash : {calculated}", calculatedHash);
            _log.LogInformation("  Match           : {match}", receivedHash == calculatedHash);

            return receivedHash == calculatedHash;
        }

       
        private string SanitizeOrderInfo(string orderInfo)
        {
            if (string.IsNullOrEmpty(orderInfo))
                return "Refund request";

            try
            {
                // Bỏ dấu tiếng Việt
                string normalized = orderInfo.Normalize(NormalizationForm.FormD);
                var stringBuilder = new StringBuilder();

                foreach (char c in normalized)
                {
                    var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                    if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    {
                        stringBuilder.Append(c);
                    }
                }

                string result = stringBuilder
                    .ToString()
                    .Normalize(NormalizationForm.FormC)
                    .Replace("đ", "d")
                    .Replace("Đ", "D");

                result = new string(result.Where(c => c >= 32 && c <= 126).ToArray());

                if (result.Length > 255)
                    result = result.Substring(0, 255);

                _log.LogInformation("OrderInfo sanitized:");
                _log.LogInformation("  Original : {original}", orderInfo);
                _log.LogInformation("  Sanitized: {sanitized}", result);

                return result;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error sanitizing OrderInfo");
                return "Refund request";
            }
        }
        #endregion
    }
}
