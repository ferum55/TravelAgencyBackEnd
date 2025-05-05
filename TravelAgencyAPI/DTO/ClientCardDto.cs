namespace TravelAgencyAPI.DTO
{
    public class ClientCardDto
    {
        public int ClientId { get; set; }

        // Персональні дані
        public string LastName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;

        // Список покупок
        public List<PurchaseDto> Purchases { get; set; } = new();
    }

    public class PurchaseDto
    {
        public int TourPurchaseId { get; set; }
        public string PurchaseNumber { get; set; } = null!;
        public DateTime PurchaseDate { get; set; }
        public int StatusId { get; set; }           // ← ДОДАНО
        public string Status { get; set; } = null!;
        public decimal Price { get; set; }
        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string InsuranceType { get; set; } = null!;
        public decimal PaymentAmount { get; set; }
        public decimal CoverageAmount { get; set; }
        public string InsuranceCompanyName { get; set; } = null!;
        public List<string> CoveredRisks { get; set; } = new();
    }


}
