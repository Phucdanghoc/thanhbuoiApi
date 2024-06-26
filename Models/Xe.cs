using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ThanhBuoiAPI.Models
{

    public enum TrangThaiXe
    {
        Acitve,
        NoActive,
        Maintenance
    }
    public class Xe
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Ten { get; set; }

        [Required]
        [StringLength(255)]
        public required string BienSo { get; set; }

        public string MaXe { get; set; }

        [ForeignKey("ID_LoaiXe")]
        public LoaiXe LoaiXe { get; set; }

        public TrangThaiXe Trangthai { get; set; }
        public string HangXe { get; set; }
        [JsonIgnore]
        public ICollection<SoDoGhe> soDoGhes { get; set; }

        [JsonIgnore]
        public ICollection<Chuyen> Chuyens { get; set; }
    }
}
