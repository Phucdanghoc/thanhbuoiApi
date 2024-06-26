namespace ThanhBuoiAPI.Models.DTO
{
    public class CheckoutDto
    {
        public int Id { get; set; }
        public string Mota { get; set; }
        public string PaymentMethod { get; set; }
        public string Email { get; set; }
    }
    public class PaymentDTO
    {
        public string cost { get; set; }
        public string url { get; set; }
    }

    public class MomoPaymentResponseDTO
    {
        public string PartnerCode { get; set; }
        public string OrderId { get; set; }
        public string RequestId { get; set; }
        public double Amount { get; set; }
        public long ResponseTime { get; set; }
        public string Message { get; set; }
        public int ResultCode { get; set; }
        public string PayUrl { get; set; }
        public string ShortLink { get; set; }
    }
}
