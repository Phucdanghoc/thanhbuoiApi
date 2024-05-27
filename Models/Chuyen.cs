using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ThanhBuoiAPI.Models
{
    public enum TrangThaiChuyen
    {
        WAITING,
        ACTIVE,
        COMPLETE
    }
    public class Chuyen
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Ten { get; set; }

        [JsonIgnore]
        [ForeignKey("ID_Xe")]
        public Xe? Xe { get; set; }
        [JsonIgnore]
        [ForeignKey("Id_Tuyen")] 
        public Tuyen? Tuyen { get; set; }
        [Required]
        [StringLength(255)]
        public string? DiemDon { get; set; }
        public DateTime ThoiGianDi { get; set; }
        [Required]
        [StringLength(255)]
        public TrangThaiChuyen? Trangthai { get; set; }
        public double Gia { get; set; }
        public String? Ngayle { get; set; }
        public double? GiaTang {  get; set; }

        [JsonIgnore]
        public ICollection<Ve> Ves;

       
    }
}
