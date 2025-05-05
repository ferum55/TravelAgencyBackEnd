using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelAgencyAPI.Models
{
    public class TransportBooking
    {
        [Key]
        [Column("transport_booking_id")]
        public int TransportBookingId { get; set; }

        [Required]
        [Column("tour_id")]
        public int TourId { get; set; }

        [ForeignKey("TourId")]
        public Tour Tour { get; set; } = null!;

        [Required]
        [Column("arrival_point_id")]
        public int ArrivalPointId { get; set; }

        [ForeignKey("ArrivalPointId")]
        public TransportPoint ArrivalPoint { get; set; } = null!;

        [Required]
        [Column("departure_point_id")]
        public int DeparturePointId { get; set; }

        [ForeignKey("DeparturePointId")]
        public TransportPoint DeparturePoint { get; set; } = null!;

        [Column("arrival_date")]
        public DateTime ArrivalDate { get; set; }

        [Column("departure_date")]
        public DateTime DepartureDate { get; set; }

        [Column("price")]
        public decimal Price { get; set; }
    }
}