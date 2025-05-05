using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelAgencyAPI.Models
{
    [Table("Incidents")]
    public class Incident
    {
        [Key]
        [Column("incident_id")]
        public int IncidentId { get; set; }

        [Required]
        [Column("tour_id")]
        public int TourId { get; set; }

        [ForeignKey(nameof(TourId))]
        public Tour Tour { get; set; } = null!;

        [Required]
        [Column("risk_id")]
        public int RiskId { get; set; }

        [ForeignKey(nameof(RiskId))]
        public Risk Risk { get; set; } = null!;

        [Required]
        [Column("incident_date")]
        public DateTime IncidentDate { get; set; }

        [Column("description", TypeName = "nvarchar(max)")]
        public string? Description { get; set; }
    }
}
