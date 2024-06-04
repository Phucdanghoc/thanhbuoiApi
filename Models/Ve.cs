using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ThanhBuoiAPI.Models
{
    public enum TrangThaiVe
    {
        Cancel,
        Empty,
        Booked,
        Waiting
    }
    public class Ve
    {
        [Key]
        public int Id { get; set; }


        [ForeignKey("ID_TaiKhoan")]
        public TaiKhoan? TaiKhoan { get; set; }

        [ForeignKey("ID_Chuyen")]
        public Chuyen Chuyen { get; set; }
        public string? email { get; set; }

        public string? MaVe { get; set; }    

        [ForeignKey("ID_Ghe")]
        public Ghe Ghe { get; set; }

        public double Tien { get; set; }

        [StringLength(255)]
        public string? Ten { get; set; }

        [StringLength(255)]
        public string? Sdt { get; set; }

        [StringLength(255)]
        public string? CMND { get; set; }

        [Required]
        [StringLength(255)]
        public string? DiemDon { get; set; }

        [Required]
        public TrangThaiVe  TrangThai { get; set; }

        public int Hanhli { get; set; }
        public string? phuongthucthanhtoan { get; set; }

        public bool isCancel {  get; set; }
        public DateTime NgayTao { get; set; }

    }
}
