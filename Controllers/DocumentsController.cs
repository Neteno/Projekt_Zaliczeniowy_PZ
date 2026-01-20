using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projekt_Zaliczeniowy_PZ.Data;
using Projekt_Zaliczeniowy_PZ.Models;
using Projekt_Zaliczeniowy_PZ.DTOs;
using Microsoft.AspNetCore.Authorization;
using Projekt_Zaliczeniowy_PZ.Security;
using Projekt_Zaliczeniowy_PZ.Models.Enums;
using Ganss.Xss;


namespace Projekt_Zaliczeniowy_PZ.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDocumentAccessService _access;
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
        public DocumentsController(ApplicationDbContext context, IDocumentAccessService access)
        {
            _context = context;
            _access = access;
        }


        // GET: Documents
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();

            var docs = await _context.Documents.AsNoTracking().Where(d => d.CreatedById == userId || _context.DocumentPermissions.Any(p => p.DocumentId == d.Id && p.UserId == userId)).ToListAsync();

            return View(docs);
        }


        // GET: Documents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            if (!await _access.CanViewAsync(id.Value, GetUserId()))
                return Forbid();

            var document = await _context.Documents.FirstOrDefaultAsync(m => m.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // GET: Documents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content")] DocumentDTO documentDTO)
        {
            if (!ModelState.IsValid)
                return View(documentDTO);

            var sanitizer = new HtmlSanitizer();
            var cleanHtml = sanitizer.Sanitize(documentDTO.Content ?? "");

            var userId = GetUserId();

            var document = new Document
            {
                Title = documentDTO.Title,
                Content = cleanHtml,
                CreatedById = userId
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            _context.DocumentPermissions.Add(new DocumentPermission
            {
                DocumentId = document.Id,
                UserId = userId,
                Role = DocumentRole.Author
            });
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Documents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            if (!await _access.CanEditAsync(id.Value, GetUserId()))
                return Forbid();

            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content")] DocumentDTO documentDTO)
        {
            if (id != documentDTO.Id)
            {
                return NotFound();
            }

            if (!await _access.CanEditAsync(id, GetUserId()))
                return Forbid();

            var document = await _context.Documents.FirstOrDefaultAsync(d => d.Id == id);

            if (document == null)
            {
                return NotFound();
            }

            document.Title = documentDTO.Title;

            document.Content = documentDTO.Content;

            if (!ModelState.IsValid)
            {
                return View(documentDTO);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: Documents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (!await _access.IsOwnerAsync(id.Value, GetUserId()))
                return Forbid();

            var document = await _context.Documents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (!await _access.IsOwnerAsync(id, GetUserId()))
                return Forbid();

            if (document != null)
            {
                _context.Documents.Remove(document);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentExists(int id, string userId)
        {
            return _context.Documents.Any(e => e.Id == id && e.CreatedById == userId);
        }
    }
}
