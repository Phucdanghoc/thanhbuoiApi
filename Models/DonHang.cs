using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ThanhBuoiAPI.Models
{
    public enum TrangThaiDonHang
    {
        Waiting,
        Cod,
        Payment,
    }
    public class DonHang
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ID_TaiKhoan")]
        public TaiKhoan? TaiKhoan { get; set; }
        public string MaDon { get; set; }

        [StringLength(255)]
        public string? RequestId { get; set; }

        [StringLength(255)]
        public string? Id_momoRes { get; set; }

        public double Tien { get; set; }

        public double TienPhaiTra { get; set; }

        [StringLength(255)]
        public string? PhuongThucThanhToan { get; set; }

        [StringLength(255)]
        public string? Mota { get; set; }

        [StringLength(255)]
        public TrangThaiDonHang? Trangthai { get; set; }

        public DateTime NgayTao { get; set; }
        public string? Email { get; set; }
        public ICollection<DonHangChiTiet> DonHangChiTiets { get; set; }
    }
}
