using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TravelAgencyAPI.Models
{
    public class Country
    {
        [Key]
        [Required]
        [Column("country_id")]
        public int CountryId { get; set; }
        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; }
    }
}
