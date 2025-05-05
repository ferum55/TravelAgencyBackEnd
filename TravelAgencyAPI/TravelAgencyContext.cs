using Microsoft.EntityFrameworkCore;
using TravelAgencyAPI.Models;

namespace TravelAgencyAPI
{
    public class TravelAgencyContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<InsuranceRisk>()
                .HasKey(ir => new { ir.InsuranceId, ir.RiskId });
        }

        public TravelAgencyContext(DbContextOptions<TravelAgencyContext> options) : base(options) { }

        // 👤 Авторизація
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;

        // 📘 Класифікатори
        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<TransportType> TransportTypes { get; set; } = null!;
        public DbSet<Risk> Risks { get; set; } = null!;
        public DbSet<Hotel> Hotels { get; set; } = null!; // ➡️ новий DbSet

        // 📘 Сутності
        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<InsuranceCompany> InsuranceCompanies { get; set; } = null!;
        public DbSet<BaseTour> BaseTours { get; set; } = null!;
        public DbSet<ActivityType> ActivityTypes { get; set; } = null!;
        public DbSet<Tour> Tours { get; set; } = null!;

        // 📘 Операції
        public DbSet<HotelBooking> HotelBookings { get; set; } = null!;
        public DbSet<TransportPoint> TransportPoints { get; set; }
        public DbSet<TransportBooking> TransportBookings { get; set; }
        public DbSet<TourPurchase> TourPurchases { get; set; } = null!;

        public DbSet<PurchaseStatus> PurchaseStatuses { get; set; } = null!;
        public DbSet<Insurance> Insurances { get; set; } = null!;
        public DbSet<InsuranceRisk> InsuranceRisks { get; set; } = null!;
        public DbSet<Incident> Incidents { get; set; } = null!;
        public DbSet<IncidentResolution> IncidentResolutions { get; set; } = null!;
        public DbSet<Payout> Payouts { get; set; } = null!;
    }
}
