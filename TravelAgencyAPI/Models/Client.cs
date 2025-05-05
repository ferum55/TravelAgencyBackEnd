// у файлі TravelAgencyAPI/Models/Client.cs (або як він у вас називається)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TravelAgencyAPI.Models
{
    [Table("Clients")]
    public class Client
    {
        [Key]
        [Column("client_id")]
        public int ClientId { get; set; }

        // ваші інші поля:
        [Required, MaxLength(100)]
        [Column("email")]
        public string Email { get; set; } = null!;

        [Required, MaxLength(20)]
        [Column("phone_number")]
        public string PhoneNumber { get; set; } = null!;

        [Required, MaxLength(50)]
        [Column("last_name")]
        public string LastName { get; set; } = null!;

        [Required, MaxLength(50)]
        [Column("first_name")]
        public string FirstName { get; set; } = null!;

        [MaxLength(50)]
        [Column("middle_name")]
        public string? MiddleName { get; set; }

        // Ось ця властивість — колекція покупок цього клієнта
        public ICollection<TourPurchase> TourPurchases { get; set; } = new List<TourPurchase>();
    }
}
