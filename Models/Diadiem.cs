using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ThanhBuoiAPI.Models
{
    public class Diadiem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Ten { get; set; }

        [Required]
        [StringLength(255)]
        public string Diachi { get; set; }
        [NotMapped]
        public ICollection<Tuyen> TuyensDi { get; set; } // Danh sách các tuyến đi từ đây
        [NotMapped]
        public ICollection<Tuyen> TuyensDen { get; set; } // Danh sách các tuyến đến đây
    }
}
