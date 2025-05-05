using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TravelAgencyAPI.Models
{
    public class TransportPoint
    {
        [Key]
        [Column("transport_point_id")]
        public int TransportPointId { get; set; }

        [Required]
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column("address")]
        [StringLength(200)]
        public string? Address { get; set; }

        [Required]
        [Column("city_id")]
        public int CityId { get; set; }

        [ForeignKey("CityId")]
        public City City { get; set; } = null!;

        [Required]
        [Column("transport_type_id")]
        public int TransportTypeId { get; set; }

        [ForeignKey("TransportTypeId")]
        public TransportType TransportType { get; set; } = null!;
    }

}
