using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ThanhBuoiAPI.Models
{
    public class Tuyen
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Ten { get; set; }

        [ForeignKey("DiemDi")]
        public int ID_DiemDi { get; set; }
        public Diadiem DiemDi { get; set; }

        [ForeignKey("DiemDen")]
        public int ID_DiemDen { get; set; }
        public Diadiem DiemDen { get; set; }

        [JsonIgnore]
        public ICollection<Chuyen> Chuyens { get; set; }
    }
}
