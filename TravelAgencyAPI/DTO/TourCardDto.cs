namespace TravelAgencyAPI.DTO
{
    public class TourCardDto
    {
        public int TourId { get; set; }
        public int BaseTourId { get; set; }

        public int HotelBookingId { get; set; }

        public decimal BaseTourPrice { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Country { get; set; }  // ← якщо треба теж як ID, заміни на int
        public string City { get; set; }     // ← якщо треба теж як ID, заміни на int
        public string ActivityName { get; set; }
        public decimal TotalCost { get; set; }

        public HotelShortDto Hotel { get; set; } = new();
        public string HotelRoomNumber { get; set; }
        public decimal HotelBookingPrice { get; set; }

        public string CheckInDate { get; set; }
        public int Duration { get; set; }
        public int EmployeeId { get; set; }

        public List<TransportBookingDto> TransportBookings { get; set; } = new();
    }

    public class HotelShortDto
    {
        public int HotelId { get; set; }
        public string HotelName { get; set; }
        public string HotelAddress { get; set; }
        public string HotelCity { get; set; }  // ← якщо треба як ID, заміни на int
    }

    public class TransportBookingDto
    {
        public int DepartureCountry { get; set; }
        public int DepartureCity { get; set; }
        public int DeparturePoint { get; set; }

        public string DeparturePointName { get; set; }
        public int ArrivalCountry { get; set; }
        public int ArrivalCity { get; set; }
        public int ArrivalPoint { get; set; }

        public string ArrivalPointName { get; set; }
        public string TransportType { get; set; }
        public string DepartureDate { get; set; }
        public string ArrivalDate { get; set; }
        public decimal Price { get; set; }
    }
}
