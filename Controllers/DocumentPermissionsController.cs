using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projekt_Zaliczeniowy_PZ.Data;
using Projekt_Zaliczeniowy_PZ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Projekt_Zaliczeniowy_PZ.Security;
using System.Security.Claims;


namespace Projekt_Zaliczeniowy_PZ.Controllers
{
    [Authorize]
    public class DocumentPermissionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDocumentAccessService _access;
        private readonly UserManager<AppUser> _userManager;

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }


        public DocumentPermissionsController(ApplicationDbContext context, IDocumentAccessService access, UserManager<AppUser> userManager)
        {
            _context = context;
            _access = access;
            _userManager = userManager;
        }


        // GET: DocumentPermissions
        public async Task<IActionResult> Index(int documentId)
        {
            if (!await _access.IsOwnerAsync(documentId, GetUserId()))
                return Forbid();

            var applicationDbContext = _context.DocumentPermissions
                .Where(p => p.DocumentId == documentId)
                .Include(d => d.User);

            ViewBag.DocumentId = documentId;
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DocumentPermissions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documentPermission = await _context.DocumentPermissions
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (documentPermission == null)
            {
                return NotFound();
            }

            if (!await _access.IsOwnerAsync(documentPermission.DocumentId, GetUserId()))
            {
                return Forbid();
            }

            return View(documentPermission);
        }

        // GET: DocumentPermissions/Create
        public async Task<IActionResult> Create(int documentId)
        {
            if (!await _access.IsOwnerAsync(documentId, GetUserId()))
                return Forbid();

            ViewData["DocumentId"] = documentId;
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: DocumentPermissions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DocumentId,UserId,Role")] DocumentPermission documentPermission)
        {
            if (!await _access.IsOwnerAsync(documentPermission.DocumentId, GetUserId()))
                return Forbid();

            if (documentPermission.Role == Projekt_Zaliczeniowy_PZ.Models.Enums.DocumentRole.Author)
            {
                ModelState.AddModelError(nameof(documentPermission.Role), "Nie można nadać roli Author. Właściciel (Author) to twórca dokumentu.");
            }
            if (ModelState.IsValid)
            {
                _context.Add(documentPermission);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { documentId = documentPermission.DocumentId });

            }
            ViewData["DocumentId"] = documentPermission.DocumentId;
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", documentPermission.UserId);
            return View(documentPermission);

        }

        // GET: DocumentPermissions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documentPermission = await _context.DocumentPermissions.FindAsync(id);
            if (documentPermission == null)
            {
                return NotFound();
            }
            if (!await _access.IsOwnerAsync(documentPermission.DocumentId, GetUserId()))
            {
                return Forbid();
            }

            ViewData["DocumentId"] = documentPermission.DocumentId;
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", documentPermission.UserId);
            return View(documentPermission);
        }

        // POST: DocumentPermissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DocumentId,UserId,Role")] DocumentPermission documentPermission)
        {
            if (id != documentPermission.Id)
            {
                return NotFound();
            }
            if (!await _access.IsOwnerAsync(documentPermission.DocumentId, GetUserId()))
                return Forbid();

            if (documentPermission.Role == Projekt_Zaliczeniowy_PZ.Models.Enums.DocumentRole.Author)
            {
                ModelState.AddModelError(nameof(documentPermission.Role), "Nie można ustawić roli Author. Właściciel dokumentu jest ustalany przez CreatedById.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(documentPermission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentPermissionExists(documentPermission.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { documentId = documentPermission.DocumentId });

            }
            ViewData["DocumentId"] = documentPermission.DocumentId;
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", documentPermission.UserId);
            return View(documentPermission);

        }

        // GET: DocumentPermissions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documentPermission = await _context.DocumentPermissions
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (documentPermission == null)
            {
                return NotFound();
            }
            if (!await _access.IsOwnerAsync(documentPermission.DocumentId, GetUserId()))
            {
                return Forbid();
            }

            if (documentPermission.Role == Projekt_Zaliczeniowy_PZ.Models.Enums.DocumentRole.Author)
            {
                return Forbid();
            }

            return View(documentPermission);
        }

        // POST: DocumentPermissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var documentPermission = await _context.DocumentPermissions.FindAsync(id);
            if (documentPermission == null)
            {
                return NotFound();
            }

            if (!await _access.IsOwnerAsync(documentPermission.DocumentId, GetUserId()))
            {
                return Forbid();
            }

            if (documentPermission.Role == Projekt_Zaliczeniowy_PZ.Models.Enums.DocumentRole.Author)
            {
                return Forbid();
            }

            var documentId = documentPermission.DocumentId;

            _context.DocumentPermissions.Remove(documentPermission);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { documentId });

        }

        private bool DocumentPermissionExists(int id)
        {
            return _context.DocumentPermissions.Any(e => e.Id == id);
        }
    }
}
