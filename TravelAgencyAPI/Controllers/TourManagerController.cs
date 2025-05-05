using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using TravelAgencyAPI;
using TravelAgencyAPI.DTO;
using TravelAgencyAPI.Models;

namespace TravelAgencyAPI.Controllers
{
    [ApiController]
    [Route("api/tour-manager")]
    public class TourManagerController : ControllerBase
    {
        private readonly TravelAgencyContext _context;

        public TourManagerController(TravelAgencyContext context)
        {
            _context = context;
        }

        [HttpPut("update-tour")]
        public async Task<IActionResult> UpdateTour([FromBody] TourCardDto updatedTour)
        {
            var tour = await _context.Tours
                .Include(t => t.BaseTour)
                .Include(t => t.HotelBookings)
                .Include(t => t.ActivityType)
                .Include(t => t.TransportBookings)
                .FirstOrDefaultAsync(t => t.TourId == updatedTour.TourId);

            if (tour == null)
                return NotFound();

            // Оновлюємо основні поля
            tour.StartDate = DateTime.Parse(updatedTour.StartDate);
            tour.EndDate = DateTime.Parse(updatedTour.EndDate);
            tour.TotalCost = updatedTour.TotalCost;

            // Оновлюємо BaseTour
            var baseTour = await _context.BaseTours
                .FirstOrDefaultAsync(bt => bt.BaseTourId == updatedTour.BaseTourId);
            if (baseTour != null)
                tour.BaseTourId = baseTour.BaseTourId;

            // Оновлюємо ActivityType
            var newActivity = await _context.ActivityTypes
                .FirstOrDefaultAsync(a => a.Name == updatedTour.ActivityName);
            if (newActivity != null)
                tour.ActivityTypeId = newActivity.ActivityTypeId;

            // Оновлюємо HotelBooking, якщо є
            var hb = tour.HotelBookings.FirstOrDefault();
            if (hb != null && updatedTour.HotelBookingId > 0)
            {
                await _context.Database.ExecuteSqlRawAsync(@"
            UPDATE HotelBookings
            SET hotel_id = {0}, room_number = {1}, price = {2}
            WHERE hotel_booking_id = {3}",
                    updatedTour.Hotel.HotelId,
                    updatedTour.HotelRoomNumber,
                    updatedTour.HotelBookingPrice,
                    updatedTour.HotelBookingId
                );
            }

            // Видаляємо старі TransportBookings
            var existingTransportBookings = tour.TransportBookings.ToList();
            _context.TransportBookings.RemoveRange(existingTransportBookings);

            // Додаємо нові TransportBookings
            foreach (var tbDto in updatedTour.TransportBookings)
            {
                var depPoint = await _context.TransportPoints
                    .FirstOrDefaultAsync(p => p.TransportPointId == tbDto.DeparturePoint);
                var arrPoint = await _context.TransportPoints
                    .FirstOrDefaultAsync(p => p.TransportPointId == tbDto.ArrivalPoint);

                if (depPoint != null && arrPoint != null)
                {
                    _context.TransportBookings.Add(new TransportBooking
                    {
                        TourId = tour.TourId,
                        DeparturePointId = depPoint.TransportPointId,
                        ArrivalPointId = arrPoint.TransportPointId,
                        DepartureDate = DateTime.ParseExact(tbDto.DepartureDate, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture),
                        ArrivalDate = DateTime.ParseExact(tbDto.ArrivalDate, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture),
                        Price = tbDto.Price
                    });
                }
            }


            await _context.SaveChangesAsync();

            tour = await _context.Tours
    .Include(t => t.BaseTour).ThenInclude(bt => bt.City).ThenInclude(c => c.Country)
    .Include(t => t.ActivityType)
    .Include(t => t.HotelBookings).ThenInclude(hb => hb.Hotel).ThenInclude(h => h.City)
    .Include(t => t.TransportBookings)
        .ThenInclude(tb => tb.DeparturePoint).ThenInclude(dp => dp.City).ThenInclude(c => c.Country)
    .Include(t => t.TransportBookings)
        .ThenInclude(tb => tb.DeparturePoint).ThenInclude(dp => dp.TransportType)
    .Include(t => t.TransportBookings)
        .ThenInclude(tb => tb.ArrivalPoint).ThenInclude(ap => ap.City).ThenInclude(c => c.Country)
    .Include(t => t.TransportBookings)
        .ThenInclude(tb => tb.ArrivalPoint).ThenInclude(ap => ap.TransportType)
    .FirstOrDefaultAsync(t => t.TourId == updatedTour.TourId);


            // Безпечне формування DTO
            var firstHotelBooking = tour.HotelBookings.FirstOrDefault();

            var updatedDto = new TourCardDto
            {
                TourId = tour.TourId,
                StartDate = tour.StartDate.ToString("yyyy-MM-dd"),
                EndDate = tour.EndDate.ToString("yyyy-MM-dd"),
                Country = tour.BaseTour?.City?.Country?.Name ?? "",
                City = tour.BaseTour?.City?.Name ?? "",
                ActivityName = tour.ActivityType?.Name ?? "",
                TotalCost = tour.TotalCost,
                BaseTourId = tour.BaseTourId,
                BaseTourPrice = tour.BaseTour?.Price ?? 0,
                Duration = tour.BaseTour?.Duration ?? 0,
                Hotel = new HotelShortDto
                {
                    HotelId = firstHotelBooking?.Hotel?.HotelId ?? 0,
                    HotelName = firstHotelBooking?.Hotel?.HotelName ?? "",
                    HotelAddress = firstHotelBooking?.Hotel?.Address ?? "",
                    HotelCity = firstHotelBooking?.Hotel?.City?.Name ?? ""
                },
                HotelRoomNumber = firstHotelBooking?.RoomNumber ?? "",
                HotelBookingPrice = firstHotelBooking?.Price ?? 0,
                TransportBookings = tour.TransportBookings.Select(tb => new TransportBookingDto
                {
                    DepartureCountry = tb.DeparturePoint.City.Country.CountryId,
                    DepartureCity = tb.DeparturePoint.City.CityId,
                    DeparturePoint = tb.DeparturePoint.TransportPointId,
                    DeparturePointName = tb.DeparturePoint.Name,
                    ArrivalCountry = tb.ArrivalPoint.City.Country.CountryId,
                    ArrivalCity = tb.ArrivalPoint.City.CityId,
                    ArrivalPoint = tb.ArrivalPoint.TransportPointId,
                    ArrivalPointName = tb.ArrivalPoint.Name,
                    TransportType = tb.DeparturePoint.TransportType.Name,
                    DepartureDate = tb.DepartureDate.ToString("s").Substring(0, 16),
                    ArrivalDate = tb.ArrivalDate.ToString("s").Substring(0, 16),
                    Price = tb.Price
                }).ToList()
            };

            return Ok(updatedDto);
        }




        [HttpGet("tours")]
        public async Task<IActionResult> GetTours([FromQuery] string? searchTerm = null)
        {
            var query = _context.Tours
        // Базовий тур → місто → країна
        .Include(t => t.BaseTour)
            .ThenInclude(bt => bt.City)
                .ThenInclude(c => c.Country)

        // Тип активності
        .Include(t => t.ActivityType)

        // Готельні бронювання → готель → місто
        .Include(t => t.HotelBookings)
            .ThenInclude(hb => hb.Hotel)
                .ThenInclude(h => h.City)

        // TransportBookings → DeparturePoint → місто → країна
        .Include(t => t.TransportBookings)
            .ThenInclude(tb => tb.DeparturePoint)
                .ThenInclude(dp => dp.City)
                    .ThenInclude(c => c.Country)

        // TransportBookings → DeparturePoint → TransportType
        .Include(t => t.TransportBookings)
            .ThenInclude(tb => tb.DeparturePoint)
                .ThenInclude(dp => dp.TransportType)

        // TransportBookings → ArrivalPoint → місто → країна
        .Include(t => t.TransportBookings)
            .ThenInclude(tb => tb.ArrivalPoint)
                .ThenInclude(ap => ap.City)
                    .ThenInclude(c => c.Country)

        // TransportBookings → ArrivalPoint → TransportType
        .Include(t => t.TransportBookings)
            .ThenInclude(tb => tb.ArrivalPoint)
                .ThenInclude(ap => ap.TransportType)

        .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(t =>
                    t.BaseTour.City.Country.Name.ToLower().Contains(searchTerm) ||
                    t.BaseTour.City.Name.ToLower().Contains(searchTerm) ||
                    t.ActivityType.Name.ToLower().Contains(searchTerm) ||
                    t.TransportBookings.Any(tb =>
                        tb.ArrivalPoint.TransportType.Name.ToLower().Contains(searchTerm) ||
                        tb.DeparturePoint.TransportType.Name.ToLower().Contains(searchTerm) ||
                        tb.ArrivalPoint.Name.ToLower().Contains(searchTerm) ||
                        tb.DeparturePoint.Name.ToLower().Contains(searchTerm))
                );
            }

            var tours = await query
    .Select(t => new TourCardDto
    {
        TourId = t.TourId,
        StartDate = t.StartDate.ToString("yyyy-MM-dd"),
        EndDate = t.EndDate.ToString("yyyy-MM-dd"),
        Country = t.BaseTour.City.Country.Name,
        City = t.BaseTour.City.Name,
        ActivityName = t.ActivityType.Name,
        TotalCost = t.TotalCost,

        BaseTourId = t.BaseTour.BaseTourId,
        BaseTourPrice = t.BaseTour.Price,
        Duration = t.BaseTour.Duration,

        Hotel = t.HotelBookings.Select(hb => new HotelShortDto
        {
            HotelId = hb.Hotel.HotelId,
            HotelName = hb.Hotel.HotelName,
            HotelAddress = hb.Hotel.Address,
            HotelCity = hb.Hotel.City.Name
        }).FirstOrDefault() ?? new HotelShortDto(),

        HotelRoomNumber = t.HotelBookings.FirstOrDefault() != null ? t.HotelBookings.FirstOrDefault().RoomNumber : "N/A",
        HotelBookingPrice = t.HotelBookings.Select(hb => hb.Price).FirstOrDefault(),
        HotelBookingId = t.HotelBookings.Select(hb => hb.HotelBookingId).FirstOrDefault(),

        TransportBookings = t.TransportBookings.Select(tb => new TransportBookingDto
        {
            DepartureCountry = tb.DeparturePoint.City.Country.CountryId,
            DepartureCity = tb.DeparturePoint.City.CityId,
            DeparturePoint = tb.DeparturePoint.TransportPointId,
            DeparturePointName = tb.DeparturePoint.Name, 

            ArrivalCountry = tb.ArrivalPoint.City.Country.CountryId,
            ArrivalCity = tb.ArrivalPoint.City.CityId,
            ArrivalPoint = tb.ArrivalPoint.TransportPointId,
            ArrivalPointName = tb.ArrivalPoint.Name,  

            TransportType = tb.DeparturePoint.TransportType.Name,
            DepartureDate = tb.DepartureDate.ToString("s").Substring(0, 16),
            ArrivalDate = tb.ArrivalDate.ToString("s").Substring(0, 16),
            Price = tb.Price
        }).ToList()

    })
    .ToListAsync();


            return Ok(tours);
        }

        [HttpGet("tours-with-filters")]
        public async Task<IActionResult> GetToursWithFilters(
    [FromQuery] int? countryId,
    [FromQuery] int? cityId,
    [FromQuery] string? activityName,
    [FromQuery] DateTime? startDateFrom,
    [FromQuery] DateTime? startDateTo,
    [FromQuery] DateTime? endDateFrom,
    [FromQuery] DateTime? endDateTo,
    [FromQuery] decimal? priceFrom,
    [FromQuery] decimal? priceTo,
    [FromQuery] string? status // "ongoing", "completed", "upcoming"
)
        {
            var query = _context.Tours
                .Include(t => t.BaseTour)
                    .ThenInclude(bt => bt.City)
                        .ThenInclude(c => c.Country)
                .Include(t => t.ActivityType)
                .Include(t => t.HotelBookings)
                    .ThenInclude(hb => hb.Hotel)
                        .ThenInclude(h => h.City)
                .Include(t => t.TransportBookings)
                    .ThenInclude(tb => tb.ArrivalPoint)
                        .ThenInclude(tp => tp.TransportType)
                .Include(t => t.TransportBookings)
                    .ThenInclude(tb => tb.DeparturePoint)
                        .ThenInclude(tp => tp.TransportType)
                .AsQueryable();

            if (countryId.HasValue)
                query = query.Where(t => t.BaseTour.City.CountryId == countryId.Value);

            if (cityId.HasValue)
                query = query.Where(t => t.BaseTour.CityId == cityId.Value);

            if (!string.IsNullOrWhiteSpace(activityName))
                query = query.Where(t => t.ActivityType.Name.ToLower().Contains(activityName.ToLower()));

            if (startDateFrom.HasValue)
                query = query.Where(t => t.StartDate >= startDateFrom.Value);

            if (startDateTo.HasValue)
                query = query.Where(t => t.StartDate <= startDateTo.Value);

            if (endDateFrom.HasValue)
                query = query.Where(t => t.EndDate >= endDateFrom.Value);

            if (endDateTo.HasValue)
                query = query.Where(t => t.EndDate <= endDateTo.Value);

            if (priceFrom.HasValue)
                query = query.Where(t => t.TotalCost >= priceFrom.Value);

            if (priceTo.HasValue)
                query = query.Where(t => t.TotalCost <= priceTo.Value);

            if (!string.IsNullOrWhiteSpace(status))
            {
                var today = DateTime.Today;
                switch (status.ToLower())
                {
                    case "ongoing":
                        query = query.Where(t => t.StartDate <= today && t.EndDate >= today);
                        break;
                    case "completed":
                        query = query.Where(t => t.EndDate < today);
                        break;
                    case "upcoming":
                        query = query.Where(t => t.StartDate > today);
                        break;
                }
            }

            var tours = await query
                .Select(t => new TourCardDto
                {
                    TourId = t.TourId,
                    StartDate = t.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = t.EndDate.ToString("yyyy-MM-dd"),
                    Country = t.BaseTour.City.Country.Name,
                    City = t.BaseTour.City.Name,
                    ActivityName = t.ActivityType.Name,
                    TotalCost = t.TotalCost,
                    Hotel = t.HotelBookings.Select(hb => new HotelShortDto
                    {
                        HotelId = hb.Hotel.HotelId,
                        HotelName = hb.Hotel.HotelName,
                        HotelAddress = hb.Hotel.Address,
                        HotelCity = hb.Hotel.City.Name
                    }).FirstOrDefault() ?? new HotelShortDto(),
                    HotelRoomNumber = t.HotelBookings.FirstOrDefault() != null ? t.HotelBookings.First().RoomNumber : "",
                    HotelBookingPrice = t.HotelBookings.Select(hb => hb.Price).FirstOrDefault(),
                    TransportBookings = t.TransportBookings.Select(tb => new TransportBookingDto
                    {
                        DeparturePoint = tb.DeparturePoint.TransportPointId,
                        ArrivalPoint = tb.ArrivalPoint.TransportPointId,
                        TransportType = tb.DeparturePoint.TransportType.Name,
                        DepartureDate = tb.DepartureDate != default ? tb.DepartureDate.ToString("yyyy-MM-ddTHH:mm") : "",
                        ArrivalDate = tb.ArrivalDate != default ? tb.ArrivalDate.ToString("yyyy-MM-ddTHH:mm") : "",
                        Price = tb.Price
                    }).ToList()
                })
                .ToListAsync();

            return Ok(tours);
        }



        [HttpPost("create-tour")]
        public async Task<IActionResult> CreateTour([FromBody] TourCardDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var activityType = await _context.ActivityTypes
                    .FirstOrDefaultAsync(a => a.Name == dto.ActivityName);

                if (activityType == null)
                    return BadRequest("Invalid activity type.");

                var baseTour = await _context.BaseTours
                    .FirstOrDefaultAsync(bt => bt.BaseTourId == dto.BaseTourId);

                if (baseTour == null)
                    return BadRequest("Invalid base tour.");

                var tour = new Tour
                {
                    StartDate = DateTime.Parse(dto.StartDate),
                    EndDate = DateTime.Parse(dto.StartDate).AddDays(baseTour.Duration), // <-- розрахунок за duration
                    TotalCost = dto.TotalCost,
                    EmployeeId = dto.EmployeeId,
                    BaseTourId = baseTour.BaseTourId,
              
                    ActivityTypeId = activityType.ActivityTypeId
                };

                _context.Tours.Add(tour);
                await _context.SaveChangesAsync();

                var hotelBooking = new HotelBooking
                {
                    TourId = tour.TourId,
                    HotelId = dto.Hotel.HotelId,
                    RoomNumber = dto.HotelRoomNumber,
                    NightsCount = baseTour.Duration, // <-- беремо з baseTour
                    CheckInDate = DateTime.Parse(dto.StartDate),
                    Price = dto.HotelBookingPrice
                };

                _context.HotelBookings.Add(hotelBooking);

                foreach (var tb in dto.TransportBookings)
                {
                    var transportBooking = new TransportBooking
                    {
                        TourId = tour.TourId,
                        DeparturePointId = tb.DeparturePoint,
                        ArrivalPointId = tb.ArrivalPoint,
                        DepartureDate = DateTime.Parse(tb.DepartureDate),
                        ArrivalDate = DateTime.Parse(tb.ArrivalDate),
                        Price = tb.Price
                    };
                    _context.TransportBookings.Add(transportBooking);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                // після CommitAsync()
                var newTourWithIncludes = await _context.Tours
                    .Include(t => t.BaseTour).ThenInclude(bt => bt.City).ThenInclude(c => c.Country)
                    .Include(t => t.ActivityType)
                    .Include(t => t.HotelBookings).ThenInclude(hb => hb.Hotel).ThenInclude(h => h.City)
                    .Include(t => t.TransportBookings)
                        .ThenInclude(tb => tb.DeparturePoint).ThenInclude(dp => dp.City).ThenInclude(c => c.Country)
                    .Include(t => t.TransportBookings)
                        .ThenInclude(tb => tb.DeparturePoint).ThenInclude(dp => dp.TransportType)
                    .Include(t => t.TransportBookings)
                        .ThenInclude(tb => tb.ArrivalPoint).ThenInclude(ap => ap.City).ThenInclude(c => c.Country)
                    .Include(t => t.TransportBookings)
                        .ThenInclude(tb => tb.ArrivalPoint).ThenInclude(ap => ap.TransportType)
                    .FirstOrDefaultAsync(t => t.TourId == tour.TourId);

                var createdDto = new TourCardDto
                {
                    TourId = newTourWithIncludes.TourId,
                    StartDate = newTourWithIncludes.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = newTourWithIncludes.EndDate.ToString("yyyy-MM-dd"),
                    Country = newTourWithIncludes.BaseTour.City.Country.Name,
                    City = newTourWithIncludes.BaseTour.City.Name,
                    ActivityName = newTourWithIncludes.ActivityType.Name,
                    TotalCost = newTourWithIncludes.TotalCost,
                    BaseTourId = newTourWithIncludes.BaseTourId,
                    BaseTourPrice = newTourWithIncludes.BaseTour.Price,
                    Duration = newTourWithIncludes.BaseTour.Duration,
                    Hotel = new HotelShortDto
                    {
                        HotelId = newTourWithIncludes.HotelBookings.FirstOrDefault()?.Hotel.HotelId ?? 0,
                        HotelName = newTourWithIncludes.HotelBookings.FirstOrDefault()?.Hotel.HotelName ?? "",
                        HotelAddress = newTourWithIncludes.HotelBookings.FirstOrDefault()?.Hotel.Address ?? "",
                        HotelCity = newTourWithIncludes.HotelBookings.FirstOrDefault()?.Hotel.City.Name ?? ""
                    },
                    HotelRoomNumber = newTourWithIncludes.HotelBookings.FirstOrDefault()?.RoomNumber ?? "",
                    HotelBookingPrice = newTourWithIncludes.HotelBookings.FirstOrDefault()?.Price ?? 0,
                    TransportBookings = newTourWithIncludes.TransportBookings.Select(tb => new TransportBookingDto
                    {
                        DepartureCountry = tb.DeparturePoint.City.Country.CountryId,
                        DepartureCity = tb.DeparturePoint.City.CityId,
                        DeparturePoint = tb.DeparturePoint.TransportPointId,
                        DeparturePointName = tb.DeparturePoint.Name,
                        ArrivalCountry = tb.ArrivalPoint.City.Country.CountryId,
                        ArrivalCity = tb.ArrivalPoint.City.CityId,
                        ArrivalPoint = tb.ArrivalPoint.TransportPointId,
                        ArrivalPointName = tb.ArrivalPoint.Name,
                        TransportType = tb.DeparturePoint.TransportType.Name,
                        DepartureDate = tb.DepartureDate.ToString("s").Substring(0, 16),
                        ArrivalDate = tb.ArrivalDate.ToString("s").Substring(0, 16),
                        Price = tb.Price
                    }).ToList()
                };

                return Ok(createdDto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                    errorMessage += $" | Inner exception: {ex.InnerException.Message}";

                Console.WriteLine($"Error creating tour: {errorMessage}");
                return StatusCode(500, $"Error creating tour: {errorMessage}");
            }
        }



    }
}