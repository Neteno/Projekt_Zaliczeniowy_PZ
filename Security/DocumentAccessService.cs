using Microsoft.EntityFrameworkCore;
using Projekt_Zaliczeniowy_PZ.Data;
using Projekt_Zaliczeniowy_PZ.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Projekt_Zaliczeniowy_PZ.Models;

namespace Projekt_Zaliczeniowy_PZ.Security
{
    public interface IDocumentAccessService
    {
        Task<bool> CanViewAsync(int documentId, string userId);
        Task<bool> CanEditAsync(int documentId, string userId);
        Task<bool> IsOwnerAsync(int documentId, string userId);
    }

    public class DocumentAccessService : IDocumentAccessService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public DocumentAccessService(ApplicationDbContext context, UserManager<AppUser> userManager) { _context = context; _userManager = userManager; }
        private async Task<bool> IsAdminAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            return await _userManager.IsInRoleAsync(user, AppRoles.Admin);
        }
        public async Task<bool> CanViewAsync(int documentId, string userId)
        {
            if (await IsAdminAsync(userId)) return true;
            var doc = await _context.Documents.AsNoTracking().Where(d => d.Id == documentId).Select(d => new { d.CreatedById }).FirstOrDefaultAsync();

            if (doc == null) return false;
            if (doc.CreatedById == userId) return true;

            return await _context.DocumentPermissions.AsNoTracking().AnyAsync(p => p.DocumentId == documentId && p.UserId == userId);
        }

        public async Task<bool> CanEditAsync(int documentId, string userId)
        {
            if (await IsAdminAsync(userId)) return true;
            var doc = await _context.Documents.AsNoTracking().Where(d => d.Id == documentId).Select(d => new { d.CreatedById }).FirstOrDefaultAsync();

            if (doc == null) return false;
            if (doc.CreatedById == userId) return true;

            return await _context.DocumentPermissions.AsNoTracking().AnyAsync(p => p.DocumentId == documentId && p.UserId == userId &&(p.Role == DocumentRole.Author || p.Role == DocumentRole.Editor));
        }

        public async Task<bool> IsOwnerAsync(int documentId, string userId)
        {
            if (await IsAdminAsync(userId)) return true;
            return await _context.Documents.AsNoTracking().AnyAsync(d => d.Id == documentId && d.CreatedById == userId);
        }
    }
}
