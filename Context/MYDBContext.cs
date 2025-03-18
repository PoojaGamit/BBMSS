using BBMSDATA1.Models;
using Microsoft.EntityFrameworkCore;

namespace BBMSDATA1.Context
{
    public class MYDBContext:DbContext
    {
        public MYDBContext(DbContextOptions<MYDBContext> options) : base(options)
        { }
        
        public DbSet<Donorregi> Donorregi { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<States> States { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<LoginRequest> LoginRequest { get; set; }
        public DbSet<Donations> Donations { get; set; }
        public DbSet<DonationTypes> DonationsTypes { get; set; }
        public DbSet<Districts> Districts { get; set; }
        public DbSet<ComponentTypes> ComponentTypes { get; set; }
        public DbSet<Cities> Cities { get; set; }
        public DbSet<BloodStock> BloodStock { get; set; }
        public DbSet<BloodRequest> BloodRequest { get; set; }
        public DbSet<BloodGroups> BloodGroups { get; set; }
        public DbSet<BloodBanks> BloodBanks { get; set; }
        public DbSet<BagTypes> BagTypes { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<ComponentStock> ComponentStock { get; set; }
        public DbSet<Appointments> Appointments { get; set; }
        public DbSet<BloodCamps> BloodCamps { get; set; }
        public DbSet<CampRegistration> CampRegistration { get; set; }
    }
}
