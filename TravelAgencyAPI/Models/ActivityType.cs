using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TravelAgencyAPI.Models
{
    public class ActivityType
    {
        [Key]
        [Required]
        [Column("activity_type_id")]
        public int ActivityTypeId { get; set; }
        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; }
        [Required]
        [Column("price")]
        public decimal Price { get; set; }
    }
}
