using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using SoundWeb.CustomClasses;

namespace SoundWeb.Controllers
{
    [LayoutByRole]
    public class MediaController : Controller
    {
        private readonly SoundContext _context;

        public MediaController(SoundContext context)
        {
            _context = context;
        }

        // GET: Media
        public async Task<IActionResult> Index()
        {
            var soundContext = _context.Media.Include(m => m.Music);
            return View(await soundContext.ToListAsync());
        }

        // GET: Media/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medium = await _context.Media
                .Include(m => m.Music)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medium == null)
            {
                return NotFound();
            }

            return View(medium);
        }

        // GET: Media/Create
        public IActionResult Create()
        {
            ViewData["MusicId"] = new SelectList(_context.Musics, "Id", "Id");
            return View();
        }

        // POST: Media/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Data,FileType,MusicId,Picture")] Medium medium)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medium);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MusicId"] = new SelectList(_context.Musics, "Id", "Id", medium.MusicId);
            return View(medium);
        }

        // GET: Media/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medium = await _context.Media.FindAsync(id);
            if (medium == null)
            {
                return NotFound();
            }
            ViewData["MusicId"] = new SelectList(_context.Musics, "Id", "Id", medium.MusicId);
            return View(medium);
        }

        // POST: Media/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Data,FileType,MusicId,Picture")] Medium medium)
        {
            if (id != medium.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medium);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MediumExists(medium.Id))
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
            ViewData["MusicId"] = new SelectList(_context.Musics, "Id", "Id", medium.MusicId);
            return View(medium);
        }

        // GET: Media/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medium = await _context.Media
                .Include(m => m.Music)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medium == null)
            {
                return NotFound();
            }

            return View(medium);
        }

        // POST: Media/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medium = await _context.Media.FindAsync(id);
            if (medium != null)
            {
                _context.Media.Remove(medium);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MediumExists(int id)
        {
            return _context.Media.Any(e => e.Id == id);
        }
    }
}
