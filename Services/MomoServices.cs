using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ThanhBuoiAPI.Models.DTO;

namespace ThanhBuoiApi.Services
{
    public class MomoServices
    {
        private static readonly HttpClient client = new HttpClient();

        public MomoServices() { }

        public async Task<MomoPaymentResponseDTO> Pay(PaymentDTO paymentDTO)
        {
            var accessKey = "F8BBA842ECF85";
            var secretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";
            var orderInfo = "Thanh toán đơn hàng Thành bưởi";
            var partnerCode = "MOMO";
            var redirectUrl = paymentDTO.url;
            var ipnUrl = "https://webhook.site/b3088a6a-2d17-4f8d-a383-71389a6c600b";
            var requestType = "payWithMethod";
            var amount = paymentDTO.cost;
            var orderId = partnerCode + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var requestId = orderId;
            var extraData = "";
            var orderGroupId = "";
            var autoCapture = true;
            var lang = "vi";

            var rawSignature = $"accessKey={accessKey}&amount={amount}&extraData={extraData}&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType={requestType}";

            string signature;
            using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(rawSignature));
                signature = BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
            }

            var requestBody = new
            {
                partnerCode = partnerCode,
                partnerName = "Test",
                storeId = "MomoTestStore",
                requestId = requestId,
                amount = amount,
                orderId = orderId,
                orderInfo = orderInfo,
                redirectUrl = redirectUrl,
                ipnUrl = ipnUrl,
                lang = lang,
                requestType = requestType,
                autoCapture = autoCapture,
                extraData = extraData,
                orderGroupId = orderGroupId,
                signature = signature
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var url = "https://test-payment.momo.vn/v2/gateway/api/create";

            var response = await client.PostAsync(url, data);

            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine($"Headers: {response.Headers}");
            Console.WriteLine($"Body: {result}");
            var paymentResponse = JsonConvert.DeserializeObject<MomoPaymentResponseDTO>(result);
            return paymentResponse;
        }
    }
}
