using Microsoft.EntityFrameworkCore;
using Projekt_Zaliczeniowy_PZ.Data;
using Projekt_Zaliczeniowy_PZ.Models.Enums;

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
        public DocumentAccessService(ApplicationDbContext context) => _context = context;

        public async Task<bool> CanViewAsync(int documentId, string userId)
        {
            var doc = await _context.Documents.AsNoTracking()
                .Where(d => d.Id == documentId)
                .Select(d => new { d.CreatedById })
                .FirstOrDefaultAsync();

            if (doc == null) return false;
            if (doc.CreatedById == userId) return true;

            return await _context.DocumentPermissions.AsNoTracking()
                .AnyAsync(p => p.DocumentId == documentId && p.UserId == userId);
        }

        public async Task<bool> CanEditAsync(int documentId, string userId)
        {
            var doc = await _context.Documents.AsNoTracking()
                .Where(d => d.Id == documentId)
                .Select(d => new { d.CreatedById })
                .FirstOrDefaultAsync();

            if (doc == null) return false;
            if (doc.CreatedById == userId) return true;

            return await _context.DocumentPermissions.AsNoTracking()
                .AnyAsync(p => p.DocumentId == documentId && p.UserId == userId &&
                             (p.Role == DocumentRole.Author || p.Role == DocumentRole.Editor));
        }

        public async Task<bool> IsOwnerAsync(int documentId, string userId)
        {
            return await _context.Documents.AsNoTracking()
                .AnyAsync(d => d.Id == documentId && d.CreatedById == userId);
        }
    }
}
