using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThanhBuoiAPI.Models
{
    public class SoDoGhe
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Ten { get; set; }

        public int SoGhe { get; set; }

        [Required]
        [StringLength(255)]
        public string LoaiGhe { get; set; }

        [ForeignKey("ID_Xe")]   
        public Xe Xe { get; set; }

        public ICollection<Tang> Tangs { get; set; }


    }
}
