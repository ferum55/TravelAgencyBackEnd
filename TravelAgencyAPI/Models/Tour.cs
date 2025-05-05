using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelAgencyAPI.Models
{
    public class Tour
    {
        [Key]
        [Column("tour_id")]
        public int TourId { get; set; }

        [Required]
        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Required]
        [Column("total_cost")]
        public decimal TotalCost { get; set; }

        [Required]
        [Column("employee_id")]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; } = null!;

        [Required]
        [Column("base_tour_id")]
        public int BaseTourId { get; set; }

        [ForeignKey("BaseTourId")]
        public BaseTour BaseTour { get; set; } = null!;

        [Required]
        [Column("activity_type_id")]
        public int ActivityTypeId { get; set; }

        [ForeignKey("ActivityTypeId")]
        public ActivityType ActivityType { get; set; } = null!;

        public ICollection<HotelBooking> HotelBookings { get; set; } = new List<HotelBooking>();
        public ICollection<TransportBooking> TransportBookings { get; set; } = new List<TransportBooking>();
    }
}
