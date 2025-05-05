using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelAgencyAPI.Models
{
    public class IncidentResolution
    {
        [Key]
        [Column("resolution_id")]
        public int ResolutionId { get; set; }

        [Required]
        [Column("incident_id")]
        public int IncidentId { get; set; }

        [ForeignKey("IncidentId")]
        public Incident Incident { get; set; } = null!;

        [Column("description")]
        public string? Description { get; set; }

        [Column("resolution_date")]
        public DateTime? ResolutionDate { get; set; }

        [MaxLength(100)]
        [Column("resolution_type")]
        public string? ResolutionType { get; set; }
    }
}
