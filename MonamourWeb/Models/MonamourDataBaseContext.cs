using Microsoft.EntityFrameworkCore;

namespace MonamourWeb.Models
{
    public class MonamourDataBaseContext : DbContext
    {
        public MonamourDataBaseContext(DbContextOptions<MonamourDataBaseContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<Breed> Breeds { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<PetTag>  PetTags { get; set; }
        public DbSet<ClientTag> ClientTags { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<VisitStatus> VisitStatuses { get; set; }
        public DbSet<VisitProcedure> VisitProcedures { get; set; }
        public DbSet<Visit> Visits { get; set; }
    }
}