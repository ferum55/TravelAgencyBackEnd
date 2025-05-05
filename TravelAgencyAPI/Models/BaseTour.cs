using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TravelAgencyAPI.Models
{
    public class BaseTour
    {
        [Key]
        [Required]
        [Column("base_tour_id")]
        public int BaseTourId { get; set; }
        [ForeignKey("CountryId")]
        [Required]
        [Column("country_id")]
        public int CountryId { get; set; }
        
        public Country Country { get; set; }
        [ForeignKey("CityId")]
        [Required]
        [Column("city_id")]
        public int CityId { get; set; }
        
        public City City { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Required]
        [Column("duration")]
        public int Duration { get; set; }

        [Required]
        [Column("price")]
        public decimal Price { get; set; }
    }
}
