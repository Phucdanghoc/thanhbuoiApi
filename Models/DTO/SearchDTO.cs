namespace ThanhBuoiAPI.Models.DTO
{
    public class SearchDTO
    {
        public DateTime? SelectedDate { get; set; }
        public TimeSpan? SelectedTime { get; set; }
        public string SeatType { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
    }
}
