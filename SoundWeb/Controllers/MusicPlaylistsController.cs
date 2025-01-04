using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.Models;

namespace SoundWeb.Controllers
{
    public class MusicPlaylistsController : Controller
    {
        private readonly SoundContext _context;

        public MusicPlaylistsController(SoundContext context)
        {
            _context = context;
        }

        // GET: MusicPlaylists
        public async Task<IActionResult> Index()
        {
            var soundContext = _context.MusicPlaylists.Include(m => m.Music).Include(m => m.Playlist);
            return View(await soundContext.ToListAsync());
        }

        // GET: MusicPlaylists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var musicPlaylist = await _context.MusicPlaylists
                .Include(m => m.Music)
                .Include(m => m.Playlist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (musicPlaylist == null)
            {
                return NotFound();
            }

            return View(musicPlaylist);
        }

        // GET: MusicPlaylists/Create
        public IActionResult Create()
        {
            ViewData["MusicId"] = new SelectList(_context.Musics, "Id", "Name");
            ViewData["PlaylistId"] = new SelectList(_context.Playlists, "Id", "Name");
            return View();
        }

        // POST: MusicPlaylists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PlaylistId,MusicId")] MusicPlaylist musicPlaylist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(musicPlaylist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MusicId"] = new SelectList(_context.Musics, "Id", "Name", musicPlaylist.MusicId);
            ViewData["PlaylistId"] = new SelectList(_context.Playlists, "Id", "Name", musicPlaylist.PlaylistId);
            return View(musicPlaylist);
        }

        public IActionResult AddMusic(int? playlistId)
        {
            if (playlistId != null)
            {
                HttpContext.Session.SetInt32("PlaylistId", (int)playlistId);
            }

            var music = _context.Musics.ToList();
            ViewData["MusicId"] = new SelectList(music, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMusic(int MusicId)
        {
            var playlistId = HttpContext.Session.GetInt32("PlaylistId");

            if (playlistId.HasValue)
            {
                var MusicPlaylist = new MusicPlaylist
                {
                    PlaylistId = playlistId.Value,
                    MusicId = MusicId
                };

                _context.Add(MusicPlaylist);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Playlists", new { id = playlistId.Value });
            }

            ModelState.AddModelError("", "MusicId is not set in session");
            var activeUserId = HttpContext.Session.GetInt32("ActiveUserId");
            var music = _context.Musics.ToList();
            ViewData["MusicId"] = new SelectList(music, "Id", "Name");

            return RedirectToAction("Details","Playlists", new { id = playlistId.Value });

        }

        // GET: MusicPlaylists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var musicPlaylist = await _context.MusicPlaylists.FindAsync(id);
            if (musicPlaylist == null)
            {
                return NotFound();
            }
            ViewData["MusicId"] = new SelectList(_context.Musics, "Id", "Name", musicPlaylist.MusicId);
            ViewData["PlaylistId"] = new SelectList(_context.Playlists, "Id", "Name", musicPlaylist.PlaylistId);
            return View(musicPlaylist);
        }

        // POST: MusicPlaylists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PlaylistId,MusicId")] MusicPlaylist musicPlaylist)
        {
            if (id != musicPlaylist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(musicPlaylist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MusicPlaylistExists(musicPlaylist.Id))
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
            ViewData["MusicId"] = new SelectList(_context.Musics, "Id", "Name", musicPlaylist.MusicId);
            ViewData["PlaylistId"] = new SelectList(_context.Playlists, "Id", "Name", musicPlaylist.PlaylistId);
            return View(musicPlaylist);
        }

        // GET: MusicPlaylists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var musicPlaylist = await _context.MusicPlaylists
                .Include(m => m.Music)
                .Include(m => m.Playlist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (musicPlaylist == null)
            {
                return NotFound();
            }

            return View(musicPlaylist);
        }

        // POST: MusicPlaylists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var musicPlaylist = await _context.MusicPlaylists.FindAsync(id);
            if (musicPlaylist != null)
            {
                _context.MusicPlaylists.Remove(musicPlaylist);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MusicPlaylistExists(int id)
        {
            return _context.MusicPlaylists.Any(e => e.Id == id);
        }
    }
}
