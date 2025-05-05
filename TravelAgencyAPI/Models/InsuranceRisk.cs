using System.ComponentModel.DataAnnotations.Schema;

namespace TravelAgencyAPI.Models
{
    public class InsuranceRisk
    {
        [Column("insurance_id")]
        public int InsuranceId { get; set; }

        [ForeignKey("InsuranceId")]
        public Insurance Insurance { get; set; } = null!;

        [Column("risk_id")]
        public int RiskId { get; set; }

        [ForeignKey("RiskId")]
        public Risk Risk { get; set; } = null!;
    }
}
