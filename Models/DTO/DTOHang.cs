using System.ComponentModel.DataAnnotations.Schema;

namespace ThanhBuoiAPI.Models.DTO
{
    public class DTOHang : HangGui
    {
        [NotMapped] 
        public int IdChuyen { get; set; }
        [NotMapped]
        public double Tien { get; set; }
    }
}
