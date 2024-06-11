using Microsoft.Extensions.Logging;
using Repositories.Helper;
using System;
using System.Net;
using System.Web;
using BusinessObjects;



namespace Services
{
    public class VnpayService
    {
        private readonly ILogger<VnpayService> _logger;
        public string url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"; // HTTPS
        public string returnUrl = $"https://localhost:7104/vnpayAPI/PaymentConfirm";
        public string tmnCode = "B4VPCIZO";
        public string hashSecret = "TKD4EZR2LKIZPJ809OL1I8WJYDDFGAXE";

        public VnpayService(ILogger<VnpayService> logger)
        {
            _logger = logger;
        }

        public string CreatePaymentUrl( decimal amount, string infor, string? orderinfor)
        {
            try
            {
                string hostName = Dns.GetHostName();
                string clientIPAddress = Dns.GetHostAddresses(hostName).GetValue(0).ToString();
                PayLib pay = new PayLib();
                var vnp_Amount = amount * 100000;
                pay.AddRequestData("vnp_Version", PayLib.VERSION);
                pay.AddRequestData("vnp_Command", "pay");
                pay.AddRequestData("vnp_TmnCode", tmnCode);
                pay.AddRequestData("vnp_Amount", vnp_Amount.ToString("0"));
                pay.AddRequestData("vnp_BankCode", "");
                pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                pay.AddRequestData("vnp_CurrCode", "VND");
                pay.AddRequestData("vnp_IpAddr", clientIPAddress);
                pay.AddRequestData("vnp_Locale", "vn");
                pay.AddRequestData("vnp_OrderInfo", infor);
                pay.AddRequestData("vnp_OrderType", "other");
                pay.AddRequestData("vnp_ReturnUrl", returnUrl);
                pay.AddRequestData("vnp_TxnRef", orderinfor);

                string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
                return paymentUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreatePaymentUrl method");
                throw;
            }
        }

        public bool ValidatePaymentResponse(string queryString, out string redirectUrl)
        {
            redirectUrl = "LINK";
            try
            {
                var json = HttpUtility.ParseQueryString(queryString);

                string orderId = (json["vnp_TxnRef"]).ToString();
                string orderInfor = json["vnp_OrderInfo"].ToString();
                long vnpayTranId = Convert.ToInt64(json["vnp_TransactionNo"]);
                string vnp_ResponseCode = json["vnp_ResponseCode"].ToString();
                string vnp_SecureHash = json["vnp_SecureHash"].ToString();
                var pos = queryString.IndexOf("&vnp_SecureHash");

                bool checkSignature = ValidateSignature(queryString.Substring(1, pos - 1), vnp_SecureHash, hashSecret);
                if (checkSignature && tmnCode == json["vnp_TmnCode"].ToString())
                {
                    if (vnp_ResponseCode == "00")
                    {
                        redirectUrl = "hEHE"; // Để link lúc mã vnpay thành công
                        return true;
                    }
                    else
                    {
                        redirectUrl = "LINK_FAIL"; // link đến trang lỗi giao dịch
                        return false;
                    }
                }
                else
                {
                    _logger.LogWarning("Signature validation failed or tmnCode mismatch");
                    redirectUrl = "LINK_INVALID"; // link đến trang lỗi giao dịch (không khớp)
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ValidatePaymentResponse method");
                redirectUrl = "LINK_ERROR";
                return false;
            }
        }

        private bool ValidateSignature(string rspraw, string inputHash, string secretKey)
        {
            string myChecksum = Utils.HmacSHA512(secretKey, rspraw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
