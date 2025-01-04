using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using Microsoft.AspNetCore.Http;

namespace SoundWeb.Controllers
{
    public class UserPlayListsMusicsController : Controller
    {
        private readonly SoundContext _context;

        public UserPlayListsMusicsController(SoundContext context)
        {
            _context = context;
        }

        // GET: UserPlayListsMusics
        public async Task<IActionResult> Index()
        {
            var soundContext = _context.UserPlayListsMusics.Include(u => u.Music).Include(u => u.UserPlaylist);
            return View(await soundContext.ToListAsync());
        }

        // GET: UserPlayListsMusics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPlayListsMusic = await _context.UserPlayListsMusics
                .Include(u => u.Music)
                .Include(u => u.UserPlaylist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userPlayListsMusic == null)
            {
                return NotFound();
            }

            return View(userPlayListsMusic);
        }

        // GET: UserPlayListsMusics/Create
        public IActionResult Create ()
        {
            var activeUserId = HttpContext.Session.GetInt32("ActiveUserId");

            if (activeUserId.HasValue)
            {
                ViewData["UserPlaylistId"] = new SelectList(_context.UserPlayLists.Where(p => p.UserId == activeUserId), "Id", "Name");
            }
            else
            {
                ViewData["UserPlaylistId"] = new SelectList(Enumerable.Empty<UserPlayList>(), "Id", "Name");
            }

            ViewData["MusicId"] = new SelectList(_context.Musics, "Id", "Name");
            return View();
        }

        // POST: UserPlayListsMusics/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserPlaylistId,MusicId")] UserPlayListsMusic userPlayListsMusic)
        {
            if (userPlayListsMusic.MusicId == null || userPlayListsMusic.UserPlaylistId == null)
            {
                return RedirectToAction("Create");
            }

                _context.Add(userPlayListsMusic);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
        }


        //        var activeUserId = HttpContext.Session.GetInt32("ActiveUserId");
        //            if (activeUserId.HasValue)
        //            {
        //                ViewData["UserPlaylistId"] = new SelectList(_context.UserPlayLists.Where(p => p.UserId == activeUserId), "Id", "Name", userPlayListsMusic.UserPlaylistId);
        //            }
        //            else
        //            {
        //                ViewData["UserPlaylistId"] = new SelectList(Enumerable.Empty<UserPlayList>(), "Id", "Name");
        //}

        public IActionResult AddMusicToPlaylist(int? musicId)
        {
            if (musicId != null)
            {
                HttpContext.Session.SetInt32("MusicId", (int)musicId);
            }

            var activeUserId = HttpContext.Session.GetInt32("ActiveUserId");

            if (activeUserId.HasValue)
            {
                var userPlaylists = _context.UserPlayLists.Where(p => p.UserId == activeUserId).ToList();
                ViewData["UserPlaylistId"] = new SelectList(userPlaylists, "Id", "Name");
            }
            else
            {
                ViewData["UserPlaylistId"] = new SelectList(Enumerable.Empty<UserPlayList>(), "Id", "Name");
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMusicToPlaylist(int UserPlaylistId)
        {
            var musicId = HttpContext.Session.GetInt32("MusicId");

            if (musicId.HasValue)
            {
                var userPlayListsMusic = new UserPlayListsMusic
                {
                    UserPlaylistId = UserPlaylistId,
                    MusicId = musicId.Value
                };

                _context.Add(userPlayListsMusic);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "MusicId is not set in session");
            var activeUserId = HttpContext.Session.GetInt32("ActiveUserId");

            if (activeUserId.HasValue)
            {
                ViewData["UserPlaylistId"] = new SelectList(_context.UserPlayLists.Where(p => p.UserId == activeUserId), "Id", "Name");
            }
            else
            {
                ViewData["UserPlaylistId"] = new SelectList(Enumerable.Empty<UserPlayList>(), "Id", "Name");
            }

            return View();
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
                var userPlayListsMusic = new UserPlayListsMusic
                {
                    UserPlaylistId = playlistId.Value,
                    MusicId = MusicId
                };

                _context.Add(userPlayListsMusic);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "MusicId is not set in session");
            var activeUserId = HttpContext.Session.GetInt32("ActiveUserId");
            var music = _context.Musics.ToList();
            ViewData["MusicId"] = new SelectList(music, "Id", "Name");

            return View();
            
        }

        //ViewData["MusicId"] = new SelectList(_context.Musics, "Id", "Name", userPlayListsMusic.MusicId);
        //return View(userPlayListsMusic);
        // GET: UserPlayListsMusics/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPlayListsMusic = await _context.UserPlayListsMusics.FindAsync(id);
            if (userPlayListsMusic == null)
            {
                return NotFound();
            }
            ViewData["MusicId"] = new SelectList(_context.Musics, "Id", "Name", userPlayListsMusic.MusicId);
            ViewData["UserPlaylistId"] = new SelectList(_context.UserPlayLists, "Id", "Name", userPlayListsMusic.UserPlaylistId);
            return View(userPlayListsMusic);
        }

        // POST: UserPlayListsMusics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserPlaylistId,MusicId")] UserPlayListsMusic userPlayListsMusic)
        {
            if (id != userPlayListsMusic.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userPlayListsMusic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserPlayListsMusicExists(userPlayListsMusic.Id))
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
            ViewData["MusicId"] = new SelectList(_context.Musics, "Id", "Name", userPlayListsMusic.MusicId);
            ViewData["UserPlaylistId"] = new SelectList(_context.UserPlayLists, "Id", "Name", userPlayListsMusic.UserPlaylistId);
            return View(userPlayListsMusic);
        }

        // GET: UserPlayListsMusics/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPlayListsMusic = await _context.UserPlayListsMusics
                .Include(u => u.Music)
                .Include(u => u.UserPlaylist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userPlayListsMusic == null)
            {
                return NotFound();
            }

            return View(userPlayListsMusic);
        }

        // POST: UserPlayListsMusics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userPlayListsMusic = await _context.UserPlayListsMusics.FindAsync(id);
            if (userPlayListsMusic != null)
            {
                _context.UserPlayListsMusics.Remove(userPlayListsMusic);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserPlayListsMusicExists(int id)
        {
            return _context.UserPlayListsMusics.Any(e => e.Id == id);
        }
    }
}
