using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelAgencyAPI.Models
{
    public class Hotel
    {
        [Key]
        [Column("hotel_id")]
        public int HotelId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("hotel_name")]
        public string HotelName { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        [Column("address")]
        public string Address { get; set; } = null!;

        [Required]
        [Column("city_id")]
        public int CityId { get; set; }

        [ForeignKey("CityId")]
        public City City { get; set; } = null!;

        public ICollection<HotelBooking> HotelBookings { get; set; } = new List<HotelBooking>();
    }
}

