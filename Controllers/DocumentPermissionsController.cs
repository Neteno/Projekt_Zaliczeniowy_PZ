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
        public async Task<IActionResult> Index(int? documentId)
        {
            var userId = GetUserId();

            var myDocs = await _context.Documents.AsNoTracking().Where(d => d.CreatedById == userId).Select(d => new { d.Id, d.Title }).OrderBy(d => d.Title).ToListAsync();

            ViewBag.MyDocuments = new SelectList(myDocs, "Id", "Title", documentId);

            if (documentId == null)
            {
                ViewBag.DocumentId = null;
                return View(new List<DocumentPermission>());
            }

            if (!await _access.IsOwnerAsync(documentId.Value, userId))
                return Forbid();

            ViewBag.DocumentId = documentId.Value;

            var perms = await _context.DocumentPermissions.AsNoTracking().Include(p => p.User).Where(p => p.DocumentId == documentId.Value).OrderBy(p => p.User.Email).ToListAsync();

            return View(perms);
        }

        // GET: DocumentPermissions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documentPermission = await _context.DocumentPermissions.Include(d => d.User).FirstOrDefaultAsync(m => m.Id == id);
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
            ViewData["RoleList"] = new SelectList(new[]
            {
                Projekt_Zaliczeniowy_PZ.Models.Enums.DocumentRole.Editor,
                Projekt_Zaliczeniowy_PZ.Models.Enums.DocumentRole.Viewer
            });

            return View(new DocumentPermission { DocumentId = documentId });
        }

        // POST: DocumentPermissions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DocumentId,UserId,Role")] DocumentPermission documentPermission)
        {
            if (!await _access.IsOwnerAsync(documentPermission.DocumentId, GetUserId()))
                return Forbid();

            if (documentPermission.DocumentId <= 0)
                ModelState.AddModelError(nameof(documentPermission.DocumentId), "Brak DocumentId.");

            if (documentPermission.Role == Projekt_Zaliczeniowy_PZ.Models.Enums.DocumentRole.Author)
                ModelState.AddModelError(nameof(documentPermission.Role), "Nie można nadać roli Author. Właściciel (Author) to twórca dokumentu.");

            var existsUser = await _context.Users.AnyAsync(u => u.Id == documentPermission.UserId);
            if (!existsUser)
                ModelState.AddModelError("UserId", "Wybrany użytkownik nie istnieje.");

            if (!ModelState.IsValid)
            {
                ViewData["DocumentId"] = documentPermission.DocumentId;
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", documentPermission.UserId);
                ViewData["RoleList"] = new SelectList(new[]
                {
                    Projekt_Zaliczeniowy_PZ.Models.Enums.DocumentRole.Editor,
                    Projekt_Zaliczeniowy_PZ.Models.Enums.DocumentRole.Viewer
                }, documentPermission.Role);

                return View(documentPermission);
            }

            var existing = await _context.DocumentPermissions
                .FirstOrDefaultAsync(p => p.DocumentId == documentPermission.DocumentId &&
                                          p.UserId == documentPermission.UserId);

            if (existing != null)
            {
                existing.Role = documentPermission.Role;
            }
            else
            {
                _context.Add(documentPermission);
            }

            // Log
            _context.VersionLogs.Add(new Versionlog
            {
                DocumentId = documentPermission.DocumentId,
                CreatedAt = DateTime.Now,
                UserId = GetUserId(),
                Description = $"Nadał uprawnienia {documentPermission.Role} użytkownikowi {documentPermission.UserId} w Dokumencie {documentPermission.DocumentId}"
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = "Uprawnienie zostało zapisane.";

            return RedirectToAction(nameof(Index), new { documentId = documentPermission.DocumentId });
        }


        // GET: DocumentPermissions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var documentPermission = await _context.DocumentPermissions.Include(p => p.User).AsNoTracking().FirstOrDefaultAsync(p => p.Id == id.Value);

            if (documentPermission == null)
                return NotFound();

            if (!await _access.IsOwnerAsync(documentPermission.DocumentId, GetUserId()))
                return Forbid();

            ViewData["DocumentId"] = documentPermission.DocumentId;

            ViewData["RoleList"] = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = Projekt_Zaliczeniowy_PZ.Models.Enums.DocumentRole.Editor.ToString(), Text = "Editor" },
                new SelectListItem { Value = Projekt_Zaliczeniowy_PZ.Models.Enums.DocumentRole.Viewer.ToString(), Text = "Viewer" }
            }, "Value", "Text", documentPermission.Role.ToString());


            return View(documentPermission);
        }

        // POST: DocumentPermissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Role")] DocumentPermission documentPermission)
        {
            Console.WriteLine($"[POST EDIT] ENTER id={id}, model.Id={documentPermission.Id}, model.Role={documentPermission.Role}");

            if (id != documentPermission.Id)
            {
                return NotFound();
            }

            var existing = await _context.DocumentPermissions.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);

            if (existing == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            var isOwner = await _access.IsOwnerAsync(existing.DocumentId, userId);

            if (!isOwner)
            {
                return Forbid();
            }

            if (documentPermission.Role == Projekt_Zaliczeniowy_PZ.Models.Enums.DocumentRole.Author)
            {
                ModelState.AddModelError(nameof(documentPermission.Role),
                    "Nie można ustawić roli Author. Właściciel dokumentu jest ustalany przez CreatedById.");
            }

            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    foreach (var err in kv.Value.Errors) ;
                }

                ViewData["DocumentId"] = existing.DocumentId;
                ViewData["RoleList"] = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Value = "Editor", Text = "Editor" },
                    new SelectListItem { Value = "Viewer", Text = "Viewer" }
                }, "Value", "Text", documentPermission.Role.ToString());

                existing.Role = documentPermission.Role;
                return View(existing);
            }

            var before = existing.Role;
            existing.Role = documentPermission.Role;

            // Log
            _context.VersionLogs.Add(new Versionlog
            {
                DocumentId = existing.DocumentId,
                CreatedAt = DateTime.Now,
                UserId = GetUserId(),
                Description = $"Zmienił uprawnienia użytkownikowi {documentPermission.UserId} na {documentPermission.Role} w Dokumencie {existing.DocumentId}"
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = "Uprawnienie zostało zaktualizowane.";
            return RedirectToAction(nameof(Index), new { documentId = existing.DocumentId });
        }


        // GET: DocumentPermissions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documentPermission = await _context.DocumentPermissions.Include(d => d.User).FirstOrDefaultAsync(m => m.Id == id);
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

            // Log
            _context.VersionLogs.Add(new Versionlog
            {
                DocumentId = documentPermission.DocumentId,
                CreatedAt = DateTime.Now,
                UserId = GetUserId(),
                Description = $"Użytkownik {documentPermission.UserId} traci dostęp do Dokumentu {documentPermission.DocumentId} "
            });
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { documentId });

        }

        private bool DocumentPermissionExists(int id)
        {
            return _context.DocumentPermissions.Any(e => e.Id == id);
        }
    }
}
