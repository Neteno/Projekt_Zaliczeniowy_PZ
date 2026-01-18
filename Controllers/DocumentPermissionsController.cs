using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projekt_Zaliczeniowy_PZ.Data;
using Projekt_Zaliczeniowy_PZ.Models;

namespace Projekt_Zaliczeniowy_PZ.Controllers
{
    public class DocumentPermissionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DocumentPermissionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DocumentPermissions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DocumentPermissions.Include(d => d.User);
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

            return View(documentPermission);
        }

        // GET: DocumentPermissions/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: DocumentPermissions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Role")] DocumentPermission documentPermission)
        {
            if (ModelState.IsValid)
            {
                _context.Add(documentPermission);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", documentPermission.UserId);
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", documentPermission.UserId);
            return View(documentPermission);
        }

        // POST: DocumentPermissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Role")] DocumentPermission documentPermission)
        {
            if (id != documentPermission.Id)
            {
                return NotFound();
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", documentPermission.UserId);
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

            return View(documentPermission);
        }

        // POST: DocumentPermissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var documentPermission = await _context.DocumentPermissions.FindAsync(id);
            if (documentPermission != null)
            {
                _context.DocumentPermissions.Remove(documentPermission);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentPermissionExists(int id)
        {
            return _context.DocumentPermissions.Any(e => e.Id == id);
        }
    }
}
