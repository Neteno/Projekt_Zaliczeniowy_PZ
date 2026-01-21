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
    public class VersionlogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VersionlogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Versionlogs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.VersionLogs.Include(v => v.Document).Include(v => v.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Versionlogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var versionlog = await _context.VersionLogs
                .Include(v => v.Document)
                .Include(v => v.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (versionlog == null)
            {
                return NotFound();
            }

            return View(versionlog);
        }

        // GET: Versionlogs/Create
        public IActionResult Create()
        {
            ViewData["DocumentId"] = new SelectList(_context.Documents, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Versionlogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DocumentId,UserId,CreatedAt,Description")] Versionlog versionlog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(versionlog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DocumentId"] = new SelectList(_context.Documents, "Id", "Id", versionlog.DocumentId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", versionlog.UserId);
            return View(versionlog);
        }

        // GET: Versionlogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var versionlog = await _context.VersionLogs.FindAsync(id);
            if (versionlog == null)
            {
                return NotFound();
            }
            ViewData["DocumentId"] = new SelectList(_context.Documents, "Id", "Id", versionlog.DocumentId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", versionlog.UserId);
            return View(versionlog);
        }

        // POST: Versionlogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DocumentId,UserId,CreatedAt,Description")] Versionlog versionlog)
        {
            if (id != versionlog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(versionlog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VersionlogExists(versionlog.Id))
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
            ViewData["DocumentId"] = new SelectList(_context.Documents, "Id", "Id", versionlog.DocumentId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", versionlog.UserId);
            return View(versionlog);
        }

        // GET: Versionlogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var versionlog = await _context.VersionLogs
                .Include(v => v.Document)
                .Include(v => v.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (versionlog == null)
            {
                return NotFound();
            }

            return View(versionlog);
        }

        // POST: Versionlogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var versionlog = await _context.VersionLogs.FindAsync(id);
            if (versionlog != null)
            {
                _context.VersionLogs.Remove(versionlog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VersionlogExists(int id)
        {
            return _context.VersionLogs.Any(e => e.Id == id);
        }
    }
}
