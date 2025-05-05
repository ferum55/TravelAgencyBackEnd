using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelAgencyAPI.Models
{
    public class TourPurchase
    {
        [Key]
        [Column("tour_purchase_id")]
        public int TourPurchaseId { get; set; }

        [Required]
        [Column("tour_id")]
        public int TourId { get; set; }

        [ForeignKey(nameof(TourId))]
        public Tour Tour { get; set; } = null!;

        [Required]
        [Column("client_id")]
        public int ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column("purchase_number")]
        public string PurchaseNumber { get; set; } = null!;

        [Required]
        [Column("purchase_date")]
        public DateTime PurchaseDate { get; set; }

        [Required]
        [Column("price")]
        public decimal Price { get; set; }

        [Required]
        [Column("status_id")]
        public int StatusId { get; set; }

        [ForeignKey(nameof(StatusId))]
        public PurchaseStatus Status { get; set; } = null!;

        [Required]
        [Column("insurance_id")]
        public int InsuranceId { get; set; }

        [ForeignKey(nameof(InsuranceId))]
        public Insurance Insurance { get; set; } = null!;

        [Required]
        [Column("employee_id")]
        public int EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; } = null!;
    }
}
