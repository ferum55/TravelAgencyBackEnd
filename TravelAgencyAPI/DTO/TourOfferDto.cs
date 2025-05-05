// TravelAgencyAPI/DTO/TourOfferDto.cs
namespace TravelAgencyAPI.DTO
{
    public class TourOfferDto
    {
        public int BaseTourId { get; set; }

        public string Country { get; set; } = null!;
        public int CountryId { get; set; }

        public string City { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int Duration { get; set; }

        public decimal Price { get; set; }

    }
}
