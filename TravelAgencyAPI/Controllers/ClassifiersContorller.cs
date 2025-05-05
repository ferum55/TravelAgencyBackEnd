using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAgencyAPI;
using TravelAgencyAPI.Models;

namespace TravelAgencyAPI.Controllers
{
    [ApiController]
    [Route("api/tour-classifiers")]
    public class TourClassifiersController : ControllerBase
    {
        private readonly TravelAgencyContext _context;

        public TourClassifiersController(TravelAgencyContext context)
        {
            _context = context;
        }

        [HttpGet("countries")]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _context.Countries
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    countryId = c.CountryId,
                    name = c.Name
                })
                .ToListAsync();

            return Ok(countries);
        }


        [HttpGet("cities")]
        public async Task<IActionResult> GetCities([FromQuery] int countryId)
        {
            var cities = await _context.Cities
                .Where(c => c.CountryId == countryId)
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    cityId = c.CityId,
                    name = c.Name
                })
                .ToListAsync();

            return Ok(cities);
        }


        [HttpGet("hotels")]
        public async Task<IActionResult> GetHotels([FromQuery] int cityId)
        {
            var hotels = await _context.Hotels
                .Where(h => h.CityId == cityId)
                .OrderBy(h => h.HotelName)
                .Select(h => new
                {
                    hotelId = h.HotelId,
                    name = h.HotelName,
                    city = h.City.Name,
                    address = h.Address
                })
                .ToListAsync();

            return Ok(hotels);
        }


        [HttpGet("activities")]
        public async Task<IActionResult> GetActivities()
        {
            var activities = await _context.ActivityTypes
                .OrderBy(a => a.Name)
                .Select(a => a.Name)
                .ToListAsync();

            return Ok(activities);
        }

        [HttpGet("transport-types")]
        public async Task<IActionResult> GetTransportTypes()
        {
            var transportTypes = await _context.TransportTypes
                .OrderBy(t => t.Name)
                .Select(t => t.Name)
                .ToListAsync();

            return Ok(transportTypes);
        }

        [HttpGet("transport-points")]
        public async Task<IActionResult> GetTransportPoints([FromQuery] int cityId)
        {
            var transportPoints = await _context.TransportPoints
                .Where(tp => tp.CityId == cityId)
                .OrderBy(tp => tp.Name)
                .Select(tp => new
                {
                    transportPointId = tp.TransportPointId,
                    name = tp.Name,
                    transportType = tp.TransportType.Name
                })
                .ToListAsync();

            return Ok(transportPoints);
        }


        [HttpGet("all-transport-points")]
        public async Task<IActionResult> GetAllTransportPoints()
        {
            var transportPoints = await _context.TransportPoints
                .OrderBy(tp => tp.Name)
                .Select(tp => new
                {
                    transportPointId = tp.TransportPointId,
                    name = tp.Name,
                    transportType = tp.TransportType.Name,
                    city = tp.City.Name,
                    country = tp.City.Country.Name
                })
                .ToListAsync();

            return Ok(transportPoints);
        }

        [HttpGet("base-tours")]
        public async Task<IActionResult> GetBaseTours([FromQuery] int cityId)
        {
            var baseTours = await _context.BaseTours
                .Where(bt => bt.CityId == cityId)
                .OrderBy(bt => bt.Duration)
                .Select(bt => new
                {
                    baseTourId = bt.BaseTourId,
                    duration = bt.Duration,
                    description = bt.Description,
                    countryId=bt.CountryId,
                    cityId=bt.CityId,
                    country = bt.Country.Name,
                    city = bt.City.Name,
                    price = bt.Price   // ← додано!
                })
                .ToListAsync();

            return Ok(baseTours);
        }

        [HttpGet("base-tours/{id}")]
        public async Task<IActionResult> GetBaseTourById(int id)
        {
            var baseTour = await _context.BaseTours
                .Include(bt => bt.City)
                .ThenInclude(c => c.Country)
                .Where(bt => bt.BaseTourId == id)
                .Select(bt => new
                {
                    baseTourId = bt.BaseTourId,
                    duration = bt.Duration,
                    description = bt.Description,
                    countryId = bt.CountryId,
                    cityId = bt.CityId,
                    country = bt.Country.Name,
                    city = bt.City.Name,
                    price = bt.Price
                })
                .FirstOrDefaultAsync();

            if (baseTour == null)
                return NotFound();

            return Ok(baseTour);
        }



        [HttpGet("activity-cost")]
        public async Task<IActionResult> GetActivityCost([FromQuery] string activityName)
        {
            var activity = await _context.ActivityTypes
                .FirstOrDefaultAsync(a => a.Name == activityName);

            if (activity == null)
                return NotFound($"Activity '{activityName}' not found.");

            return Ok(activity.Price);
        }

        [HttpGet("insurance-id")]
        public async Task<IActionResult> GetInsuranceId([FromQuery] string insuranceType, [FromQuery] int insuranceCompanyId)
        {
            var insurance = await _context.Insurances
                .FirstOrDefaultAsync(i =>
                    i.InsuranceType == insuranceType &&
                    i.InsuranceCompanyId == insuranceCompanyId);

            if (insurance == null)
                return NotFound("Insurance not found for given type and company.");

            return Ok(new { insuranceId = insurance.InsuranceId });
        }


        [HttpGet("tour-id")]
        public async Task<IActionResult> GetTourId(
    [FromQuery] int countryId,
    [FromQuery] int cityId,
    [FromQuery] int duration,
    [FromQuery] int activityTypeId,
    [FromQuery] DateTime startDate)
        {
            var tour = await _context.Tours
                .Include(t => t.BaseTour)
                .FirstOrDefaultAsync(t =>
                    t.ActivityTypeId == activityTypeId &&
                    t.StartDate == startDate &&
                    t.BaseTour.CountryId == countryId &&
                    t.BaseTour.CityId == cityId &&
                    t.BaseTour.Duration == duration);

            if (tour == null)
                return NotFound("Tour not found for given parameters.");

            return Ok(new { tourId = tour.TourId });
        }

        [HttpGet("durations")]
        public async Task<IActionResult> GetDurations()
        {
            var durations = await _context.BaseTours
                .Select(bt => bt.Duration)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            return Ok(durations);
        }

        [HttpGet("insurance-types")]
        public async Task<IActionResult> GetInsuranceTypes()
        {
            var insuranceTypes = await _context.Insurances
                .Select(i => i.InsuranceType)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            return Ok(insuranceTypes);
        }

        [HttpGet("insurance-companies")]
        public async Task<IActionResult> GetInsuranceCompanies()
        {
            var companies = await _context.InsuranceCompanies
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    insuranceCompanyId = c.InsuranceCompanyId,
                    name = c.Name
                })
                .ToListAsync();

            return Ok(companies);
        }




    }
}