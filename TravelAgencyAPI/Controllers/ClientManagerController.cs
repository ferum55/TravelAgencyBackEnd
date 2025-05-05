using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAgencyAPI;
using TravelAgencyAPI.DTO;
using TravelAgencyAPI.Models;

namespace TravelAgencyAPI.Controllers
{
    [ApiController]
    [Route("api/clients")]
    public class ClientsController : ControllerBase
    {
        private readonly TravelAgencyContext _context;

        public ClientsController(TravelAgencyContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Повертає список клієнтів з їхніми покупками.
        /// Підтримує пошук за term (firstName, lastName, email, phone).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetClients(
    [FromQuery] string? searchTerm,
    [FromQuery] string? firstName,
    [FromQuery] string? lastName,
    [FromQuery] string? email,
    [FromQuery] string? phoneNumber,
    [FromQuery] int? purchasesCountFrom,
    [FromQuery] int? purchasesCountTo,
    [FromQuery] decimal? totalSpentFrom,
    [FromQuery] decimal? totalSpentTo
)
        {
            var q = _context.Clients
                .Include(c => c.TourPurchases)
                    .ThenInclude(tp => tp.Tour)
                        .ThenInclude(t => t.BaseTour)
                            .ThenInclude(bt => bt.City)
                                .ThenInclude(ci => ci.Country)
                .Include(c => c.TourPurchases)
                    .ThenInclude(tp => tp.Status)
                .Include(c => c.TourPurchases)
                    .ThenInclude(tp => tp.Insurance)
                        .ThenInclude(i => i.InsuranceCompany)
                .Include(c => c.TourPurchases)
                    .ThenInclude(tp => tp.Insurance)
                        .ThenInclude(i => i.InsuranceRisks)
                            .ThenInclude(ir => ir.Risk)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                q = q.Where(c =>
                    c.FirstName.ToLower().Contains(term) ||
                    c.LastName.ToLower().Contains(term) ||
                    c.Email.ToLower().Contains(term) ||
                    c.PhoneNumber.Contains(term)
                );
            }

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                q = q.Where(c => c.FirstName.ToLower().Contains(firstName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                q = q.Where(c => c.LastName.ToLower().Contains(lastName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                q = q.Where(c => c.Email.ToLower().Contains(email.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                q = q.Where(c => c.PhoneNumber.Contains(phoneNumber));
            }

            if (purchasesCountFrom.HasValue)
                q = q.Where(c => c.TourPurchases.Count >= purchasesCountFrom.Value);
            if (purchasesCountTo.HasValue)
                q = q.Where(c => c.TourPurchases.Count <= purchasesCountTo.Value);

            if (totalSpentFrom.HasValue)
                q = q.Where(c => c.TourPurchases.Sum(tp => tp.Price) >= totalSpentFrom.Value);
            if (totalSpentTo.HasValue)
                q = q.Where(c => c.TourPurchases.Sum(tp => tp.Price) <= totalSpentTo.Value);

            var clients = await q
                .Select(c => new ClientCardDto
                {
                    ClientId = c.ClientId,
                    LastName = c.LastName,
                    FirstName = c.FirstName,
                    MiddleName = c.MiddleName,
                    PhoneNumber = c.PhoneNumber,
                    Email = c.Email,
                    Purchases = c.TourPurchases.Select(tp => new PurchaseDto
                    {
                        TourPurchaseId = tp.TourPurchaseId,
                        PurchaseNumber = tp.PurchaseNumber,
                        PurchaseDate = tp.PurchaseDate,
                        Status = tp.Status.StatusName,
                        Price = tp.Price,
                        Country = tp.Tour.BaseTour.City.Country.Name,
                        City = tp.Tour.BaseTour.City.Name,
                        StartDate = tp.Tour.StartDate,
                        EndDate = tp.Tour.EndDate,
                        InsuranceType = tp.Insurance.InsuranceType!,
                        PaymentAmount = tp.Insurance.PaymentAmount,
                        CoverageAmount = tp.Insurance.CoverageAmount,
                        InsuranceCompanyName = tp.Insurance.InsuranceCompany.Name,
                        CoveredRisks = tp.Insurance.InsuranceRisks
                                                  .Select(ir => ir.Risk.Type)
                                                  .ToList()
                    }).ToList()
                })
                .ToListAsync();

            return Ok(clients);
        }


        [HttpGet("purchase-statuses")]
        public async Task<IActionResult> GetPurchaseStatuses()
        {
            var statuses = await _context.PurchaseStatuses
                .OrderBy(s => s.StatusName)
                .Select(s => new
                {
                    statusId = s.StatusId,
                    statusName = s.StatusName
                })
                .ToListAsync();

            return Ok(statuses);
        }


        /// <summary>
        /// Створення нового клієнта.
        /// (якщо треба)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] ClientCardDto dto)
        {
            var client = new Client
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                MiddleName = dto.MiddleName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            dto.ClientId = client.ClientId;
            return CreatedAtAction(nameof(GetClients), new { id = client.ClientId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, [FromBody] ClientCardDto dto)
        {
            // 1. Завантажуємо клієнта разом із покупками
            var client = await _context.Clients
                .Include(c => c.TourPurchases)
                .FirstOrDefaultAsync(c => c.ClientId == id);

            if (client == null)
                return NotFound();

            // 2. Оновлюємо поля клієнта
            client.FirstName = dto.FirstName;
            client.LastName = dto.LastName;
            client.MiddleName = dto.MiddleName;
            client.Email = dto.Email;
            client.PhoneNumber = dto.PhoneNumber;

            // 3. Визначаємо, які покупки видалено на фронті
            var incomingIds = dto.Purchases.Select(p => p.TourPurchaseId).ToHashSet();
            var toRemove = client.TourPurchases
                .Where(tp => !incomingIds.Contains(tp.TourPurchaseId))
                .ToList();
            _context.TourPurchases.RemoveRange(toRemove);

            // 4. Оновлюємо кожну існуючу покупку
            foreach (var pDto in dto.Purchases)
            {
                var tp = client.TourPurchases
                    .FirstOrDefault(x => x.TourPurchaseId == pDto.TourPurchaseId);

                if (tp == null)
                {
                    // якщо з якоїсь причини фронт прислав новий TourPurchaseId
                    continue;
                }

                tp.PurchaseDate = pDto.PurchaseDate;
                tp.Price = pDto.Price;
                // Шукаємо statusId за назвою статусу
                tp.StatusId = await _context.PurchaseStatuses
                                       .Where(s => s.StatusName == pDto.Status)
                                       .Select(s => s.StatusId)
                                       .FirstOrDefaultAsync();
            }

            // 5. Зберігаємо всі зміни
            await _context.SaveChangesAsync();

            return Ok(dto);
        }




    }
}
