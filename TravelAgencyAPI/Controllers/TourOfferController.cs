// src/Controllers/TourOffersController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAgencyAPI;
using TravelAgencyAPI.DTO;
using TravelAgencyAPI.Models;

namespace TravelAgencyAPI.Controllers
{
    [ApiController]
    [Route("api/tour-manager")]
    public class TourOffersController : ControllerBase
    {
        private readonly TravelAgencyContext _context;

        public TourOffersController(TravelAgencyContext context)
        {
            _context = context;
        }

        [HttpGet("offers")]
        public async Task<IActionResult> GetOffers(
    [FromQuery] string? searchTerm,
    [FromQuery(Name = "countryName")] string? countryName,
    [FromQuery(Name = "cityName")] string? cityName,
    [FromQuery] int? durationFrom,
    [FromQuery] int? durationTo,
    [FromQuery] decimal? priceFrom,
    [FromQuery] decimal? priceTo
)
        {
            var q = _context.BaseTours
                .Include(bt => bt.City).ThenInclude(c => c.Country)
                .AsQueryable();

            // загальний пошук
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                q = q.Where(bt =>
                    bt.Country.Name.ToLower().Contains(term) ||
                    bt.City.Name.ToLower().Contains(term) ||
                    bt.Description.ToLower().Contains(term)
                );
            }

            // фільтр за країною
            if (!string.IsNullOrWhiteSpace(countryName))
            {
                q = q.Where(bt => bt.City.Country.Name == countryName);
            }

            // фільтр за містом
            if (!string.IsNullOrWhiteSpace(cityName))
            {
                q = q.Where(bt => bt.City.Name == cityName);
            }

            // тривалість від
            if (durationFrom.HasValue)
            {
                q = q.Where(bt => bt.Duration >= durationFrom.Value);
            }

            // тривалість до
            if (durationTo.HasValue)
            {
                q = q.Where(bt => bt.Duration <= durationTo.Value);
            }

            // вартість від
            if (priceFrom.HasValue)
            {
                q = q.Where(bt => bt.Price >= priceFrom.Value);
            }

            // вартість до
            if (priceTo.HasValue)
            {
                q = q.Where(bt => bt.Price <= priceTo.Value);
            }

            var offers = await q
                .OrderBy(bt => bt.City.Country.Name)
                .Select(bt => new TourOfferDto
                {
                    BaseTourId = bt.BaseTourId,
                    Country = bt.City.Country.Name,
                    City = bt.City.Name,
                    Description = bt.Description,
                    Duration = bt.Duration,
                    Price = bt.Price
                })
                .ToListAsync();

            return Ok(offers);
        }


        [HttpPut("offers/{id}")]
        public async Task<IActionResult> UpdateOffer(int id, [FromBody] TourOfferDto dto)
        {
            // Знаходимо існуючий базовий тур
            var baseTour = await _context.BaseTours
                .Include(bt => bt.City)
                    .ThenInclude(c => c.Country)
                .FirstOrDefaultAsync(bt => bt.BaseTourId == id);

            if (baseTour == null)
                return NotFound();

            // Оновлюємо лише дозволені поля
            baseTour.Description = dto.Description;
            baseTour.Duration = dto.Duration;
            baseTour.Price = dto.Price;  // ← додаємо оновлення ціни

            // Якщо дозволяємо змінювати місто/країну — шукаємо нове місто
            var city = await _context.Cities
                .Include(c => c.Country)
                .FirstOrDefaultAsync(c => c.Name == dto.City && c.Country.Name == dto.Country);
            if (city != null) baseTour.CityId = city.CityId;

            await _context.SaveChangesAsync();

            // Повертаємо оновлену DTO
            return Ok(new TourOfferDto
            {
                BaseTourId = baseTour.BaseTourId,
                Country = baseTour.City.Country.Name,
                City = baseTour.City.Name,
                Description = baseTour.Description,
                Duration = baseTour.Duration,
                Price = baseTour.Price  // ← додаємо ціну у відповідь
            });
        }

        [HttpPost("offers")]
        public async Task<IActionResult> CreateOffer([FromBody] TourOfferDto dto)
        {
            // шукаємо місто разом з країною
            var city = await _context.Cities
                .Include(c => c.Country)  // ← додаємо Include!
                .FirstOrDefaultAsync(c => c.Name == dto.City && c.CountryId == dto.CountryId);

            if (city == null)
                return BadRequest("Невірна країна/місто.");

            // створюємо новий BaseTour
            var baseTour = new BaseTour
            {
                CountryId = city.CountryId,
                CityId = city.CityId,
                Description = dto.Description,
                Duration = dto.Duration,
                Price = dto.Price
            };

            _context.BaseTours.Add(baseTour);
            await _context.SaveChangesAsync();

            // формуємо результат DTO
            var result = new TourOfferDto
            {
                BaseTourId = baseTour.BaseTourId,
                CountryId = city.CountryId,
                Country = city.Country.Name,
                City = city.Name,
                Description = baseTour.Description,
                Duration = baseTour.Duration,
                Price = baseTour.Price
            };

            return CreatedAtAction(nameof(UpdateOffer),
                new { id = result.BaseTourId }, result);
        }




    }
}
