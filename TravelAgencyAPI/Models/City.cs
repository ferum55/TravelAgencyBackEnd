using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelAgencyAPI.Models
{
    public class City
    {
        [Key]
        [Column("city_id")]
        public int CityId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = null!;

        [Required]
        [Column("country_id")]
        public int CountryId { get; set; }

        [ForeignKey("CountryId")]
        public Country Country { get; set; } = null!;
    }
}
