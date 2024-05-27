using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ThanhBuoiAPI.Models
{
    public class Tang
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Ten { get; set; }

        public int STT { get; set; }


        [ForeignKey("ID_SoDoGhe")]
        public SoDoGhe SoDoGhe { get; set; }

        public ICollection<Hang> Hangs { get; set; }

    }
}
