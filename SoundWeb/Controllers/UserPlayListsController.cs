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
    public class UserPlayListsController : Controller
    {
        private readonly SoundContext _context;

        public UserPlayListsController(SoundContext context)
        {
            _context = context;
        }

        // GET: UserPlayLists
        public async Task<IActionResult> Index()
        {
            var activeUserId = HttpContext.Session.GetInt32("ActiveUserId");
            if (activeUserId == null)
            {
                return RedirectToAction("Login", "Account"); 
            }
            var soundContext = _context.UserPlayLists.Include(u => u.User).Where(p => p.UserId == activeUserId);

            return View(await soundContext.ToListAsync());
        }


        // GET: UserPlayLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPlayList = await _context.UserPlayLists
                .Include(u => u.User)
                .Include(u => u.UserPlayListsMusics)
                    .ThenInclude(upm => upm.Music)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (userPlayList == null)
            {
                return NotFound();
            }

            HttpContext.Session.SetInt32("PlaylistId", (int)id);

            return View(userPlayList);
        }


        // GET: UserPlayLists/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: UserPlayLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,UserId")] UserPlayList userPlayList)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userPlayList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", userPlayList.UserId);
            return View(userPlayList);
        }

        public IActionResult Createown()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: UserPlayLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Createown([Bind("Id,Name")] UserPlayList userPlayList)
        {
                var activeUserId = HttpContext.Session.GetInt32("ActiveUserId");

                if (activeUserId.HasValue)
                {
                    userPlayList.UserId = activeUserId.Value;
                    _context.Add(userPlayList);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "ActiveUserId is not set in session");
                }
            return View(userPlayList);
        }
 
        // GET: UserPlayLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPlayList = await _context.UserPlayLists.FindAsync(id);
            if (userPlayList == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", userPlayList.UserId);
            return View(userPlayList);
        }

        // POST: UserPlayLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,UserId")] UserPlayList userPlayList)
        {
            if (id != userPlayList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userPlayList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserPlayListExists(userPlayList.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", userPlayList.UserId);
            return View(userPlayList);
        }

        // GET: UserPlayLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPlayList = await _context.UserPlayLists
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userPlayList == null)
            {
                return NotFound();
            }

            return View(userPlayList);
        }

        // POST: UserPlayLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userPlayList = await _context.UserPlayLists.FindAsync(id);
            if (userPlayList != null)
            {
                _context.UserPlayLists.Remove(userPlayList);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserPlayListExists(int id)
        {
            return _context.UserPlayLists.Any(e => e.Id == id);
        }
    }
}
