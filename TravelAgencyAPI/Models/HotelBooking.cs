using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelAgencyAPI.Models
{
    public class HotelBooking
    {
        [Key]
        [Column("hotel_booking_id")]
        public int HotelBookingId { get; set; }

        [Required]
        [Column("tour_id")]
        public int TourId { get; set; }

        [ForeignKey("TourId")]
        public Tour Tour { get; set; } = null!;

        [Required]
        [Column("hotel_id")]
        public int HotelId { get; set; }

        [ForeignKey("HotelId")]
        public Hotel Hotel { get; set; } = null!;

        [MaxLength(20)]
        [Column("room_number")]
        public string RoomNumber { get; set; } = null!;

        [Column("nights_count")]
        public int NightsCount { get; set; }

        [Column("check_in_date")]
        public DateTime CheckInDate { get; set; }

        [Column("price")]
        public decimal Price { get; set; }
    }
}
