using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelAgencyAPI.Models
{
    [Table("Insurances")]
    public class Insurance
    {
        [Key]
        [Column("insurance_id")]
        public int InsuranceId { get; set; }

        [Required]
        [Column("insurance_company_id")]
        public int InsuranceCompanyId { get; set; }

        [ForeignKey(nameof(InsuranceCompanyId))]
        public InsuranceCompany InsuranceCompany { get; set; } = null!;

        [MaxLength(50)]
        [Column("insurance_type")]
        public string? InsuranceType { get; set; }

        [Column("payment_amount")]
        public decimal PaymentAmount { get; set; }

        [Column("coverage_amount")]
        public decimal CoverageAmount { get; set; }

        public ICollection<InsuranceRisk> InsuranceRisks { get; set; } = new List<InsuranceRisk>();

    }
}
