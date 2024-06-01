using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ThanhBuoiAPI.Models
{
    public class Hang
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Ten { get; set; }

        public int STT { get; set; }


        [ForeignKey("ID_Tang")]
        public Tang Tang { get; set; }
            
        [JsonIgnore] 
        public ICollection<Ghe> Ghes{ get; set; }

    }

}
