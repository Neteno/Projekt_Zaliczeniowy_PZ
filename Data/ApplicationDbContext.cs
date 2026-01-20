using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Projekt_Zaliczeniowy_PZ.Models;

namespace Projekt_Zaliczeniowy_PZ.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Models.Document> Documents { get; set; } = default!;
        public DbSet<Models.DocumentPermission> DocumentPermissions { get; set; } = default!;
        public DbSet<Models.Versionlog> VersionLogs { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Document>()
                .HasOne(d => d.CreatedBy)
                .WithMany()
                .HasForeignKey(d => d.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DocumentPermission>()
                .HasOne(p => p.Document)
                .WithMany()
                .HasForeignKey(p => p.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DocumentPermission>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DocumentPermission>()
                .HasIndex(p => new { p.DocumentId, p.UserId })
                .IsUnique();
        }
    }
}
