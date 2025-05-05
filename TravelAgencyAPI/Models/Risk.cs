using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TravelAgencyAPI.Models
{
    public class Risk
    {
        [Key]
        [Required]
        [Column("risk_id")]
        public int RiskId { get; set; }
        [Required]
        [MaxLength(100)]
        [Column("type")]
        public string Type { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [MaxLength(50)]
        [Column("criticality")]
        public string Criticality { get; set; }
    }
}
