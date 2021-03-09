using Microsoft.EntityFrameworkCore;

namespace Conduit.Data
{
    public class ConduitContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Follow> Follows { get; set; }

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

            modelBuilder.Entity<Follow>(entity =>
            {
                entity.HasKey(e => new { e.Follower, e.Following });

                entity.HasOne(d => d.FollowerNavigation)
                    .WithMany(p => p.FollowerNavigations)
                    .HasForeignKey(d => d.Follower)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Follower_Profiles");

                entity.HasOne(d => d.FollowingNavigation)
                    .WithMany(p => p.FollowingNavigations)
                    .HasForeignKey(d => d.Following)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Following_Profiles");
            });
            base.OnModelCreating(modelBuilder);
        }

        public ConduitContext(DbContextOptions<ConduitContext> options) : base(options)
        {

        }
    }
}
