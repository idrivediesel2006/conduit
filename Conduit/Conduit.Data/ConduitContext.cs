using Microsoft.EntityFrameworkCore;

namespace Conduit.Data
{
    public class ConduitContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Person> People { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity
                    .HasIndex(e => e.Email, "IX_Accounts_Email_Unique")
                    .IsUnique();
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity
                    .Property(e => e.Id)
                    .ValueGeneratedNever();

                entity
                    .HasOne(d => d.Account)
                    .WithOne(p => p.Person)
                    .HasForeignKey<Person>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity
                    .Property(e => e.Image)
                    .HasDefaultValue("'https://static.productionready.io/images/smiley-cyrus.jpg'");
            });
            base.OnModelCreating(modelBuilder);
        }

        public ConduitContext(DbContextOptions<ConduitContext> options) : base(options)
        {

        }
    }
}
