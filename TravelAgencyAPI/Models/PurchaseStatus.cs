using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TravelAgencyAPI.Models
{
    [Table("PurchaseStatuses")]
    public class PurchaseStatus
    {
        [Key]
        [Column("status_id")]
        public int StatusId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("status_name")]
        public string StatusName { get; set; } = null!;

        public ICollection<TourPurchase> TourPurchases { get; set; } = new List<TourPurchase>();
    }
}
