using Microsoft.EntityFrameworkCore;

namespace Conduit.Data
{
    public class ConduitContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Editorial> Editorials { get; set; }
        public DbSet<Commentary> Commentaries { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Tag> Tags { get; set; }


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

            modelBuilder.Entity<Editorial>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Editorials)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Articles_People");
            });

            modelBuilder.Entity<Commentary>(entity =>
            {
                entity.Property(e => e.CreateAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Editorial)
                    .WithMany(p => p.Commentaries)
                    .HasForeignKey(d => d.EditorialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Commentary_Editorials");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Commentaries)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Commentaries_People");
            });

            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.HasKey(e => new { e.PersonId, e.EditorialId });

                entity.HasOne(d => d.Editorial)
                    .WithMany(p => p.Favorites)
                    .HasForeignKey(d => d.EditorialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Favorites_Editorials");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Favorites)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Favorites_People");
            });

            base.OnModelCreating(modelBuilder);
        }

        public ConduitContext(DbContextOptions<ConduitContext> options) : base(options)
        {

        }
    }
}
