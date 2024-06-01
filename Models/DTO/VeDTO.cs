namespace ThanhBuoiAPI.Models.DTO
{
    public class VeDTO
    {
        public int Id { get; set; }
        public string SDT { get; set; }
        public string CMND { get; set; }
        public string Ten { get; set; }
        public int HanhLi { get; set; }
    }
    public class BookingRequest
    {
        public List<VeDTO> Bookings { get; set; }
        public string Payment { get; set; }
        public string Email { get; set; }
    }

}
