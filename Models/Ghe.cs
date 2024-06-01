using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ThanhBuoiAPI.Models
{
    public class Ghe
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        public string Ten { get; set; }

        public int STT { get; set; }

        public bool KhoangTrong { get; set; }

        [ForeignKey("ID_Hang")]
        public Hang Hang { get; set; }

        [JsonIgnore]
        public  ICollection<Ve> Ves { get; set; }

    }
}
