using System.ComponentModel.DataAnnotations;

namespace ThanhBuoiAPI.Models
{
    public class TinTuc
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string TieuDe { get; set; }

        [Required]
        [StringLength(255)]
        public string NoiDung { get; set; }

        public DateTime NgayDang { get; set; }

        public int LuotXem { get; set; }

    }
}
