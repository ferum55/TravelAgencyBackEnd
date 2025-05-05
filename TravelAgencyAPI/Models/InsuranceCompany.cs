using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TravelAgencyAPI.Models
{
    public class InsuranceCompany
    {
        [Key]
        [Required]
        [Column("insurance_company_id")]
        public int InsuranceCompanyId { get; set; }
        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; }
        [Required]
        [MaxLength(20)]
        [Column("phone_number")]
        public string PhoneNumber { get; set; }
        [Required]
        [MaxLength(100)]
        [Column("email")]
        public string Email { get; set; }
        [MaxLength(20)]
        [Column("rating")]
        public string Rating { get; set; }
    }
}
