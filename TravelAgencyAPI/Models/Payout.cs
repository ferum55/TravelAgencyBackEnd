using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelAgencyAPI.Models
{
    public class Payout
    {
        [Key]
        [Column("payout_id")]
        public int PayoutId { get; set; }

        [Required]
        [Column("resolution_id")]
        public int ResolutionId { get; set; }

        [ForeignKey("ResolutionId")]
        public IncidentResolution IncidentResolution { get; set; } = null!;

        [Required]
        [Column("payout_date")]
        public DateTime PayoutDate { get; set; }

        [MaxLength(50)]
        [Column("status")]
        public string? Status { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }
    }
}
