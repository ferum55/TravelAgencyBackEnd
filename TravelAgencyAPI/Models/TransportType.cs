using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TravelAgencyAPI.Models
{
    public class TransportType
    {
        [Key]
        [Required]
        [Column("transport_type_id")]
        public int TransportTypeId { get; set; }
        [Required]
        [MaxLength(50)]
        [Column("name")]
        public string Name { get; set; }
    }
}
