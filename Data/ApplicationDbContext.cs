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
    }
}
