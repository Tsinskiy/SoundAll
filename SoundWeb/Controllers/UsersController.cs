using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using SoundWeb.CustomClasses;
using Microsoft.AspNetCore.Authorization;

namespace SoundWeb.Controllers
{
    [LayoutByRole]
 
    public class UsersController : Controller
    {
        private readonly SoundContext _context;

        public UsersController(SoundContext context)
        {
            _context = context;
        }

        // GET: Users
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }



        [Authorize(Roles = "Admin,RegisteredUser,PaidUser")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUserRole = HttpContext.Session.GetString("UserRole");
            var currentUserId = HttpContext.Session.GetInt32("ActiveUserId");

            if (currentUserRole != "Admin" && currentUserId != id)
            {
                return Forbid(); // Возвращаем ошибку 403 Forbidden, если пользователь не администратор и пытается получить доступ к чужой информации
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [Authorize(Roles = "Admin")]
        // GET: Users/Create
        public IActionResult Create()
        {
            ViewBag.UserTypes = Enum.GetValues(typeof(UsersTypes)).Cast<UsersTypes>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString()
                }).ToList();
            return View();
        }

        // POST: Users/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,Email,Password,UserType,AccountFinishDate")] User user)
        {
            if (ModelState.IsValid)
            {
                // Set AccountFinishDate to null
                user.AccountFinishDate = null;

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // If model state is not valid, pass the UserTypes again for the view
            ViewBag.UserTypes = Enum.GetValues(typeof(UsersTypes)).Cast<UsersTypes>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString()
                }).ToList();
            return View(user);
        }


        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Name,Surname,Email,Password")] User user)
        {
            user.UserType = 2;
            if (ModelState.IsValid)
            {
                user.AccountFinishDate = null;
                
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [AllowAnonymous]
        public IActionResult Login()
		{
			return View();
		}


        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User model)
        {
            string email = model.Email;
            string password = model.Password;
            User? user = await _context.Users.FirstOrDefaultAsync(p => p.Email == email && p.Password == password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, ((UsersTypes)user.UserType).ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                HttpContext.Session.SetInt32("ActiveUserId", user.Id);
                HttpContext.Session.SetString("UserRole", ((UsersTypes)user.UserType).ToString());

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        // GET: Users/Edit/5
        [Authorize(Roles = "Admin,RegisteredUser,PaidUser")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var currentUserRole = HttpContext.Session.GetString("UserRole");
            var currentUserId = HttpContext.Session.GetInt32("ActiveUserId");

            if (currentUserRole != "Admin" && currentUserId != id)
            {
                return Forbid(); // Возвращаем ошибку 403 Forbidden, если пользователь не администратор и пытается редактировать чужую информацию
            }

            return View(user);
        }

        // POST: Users/Edit/5
        [Authorize(Roles = "Admin,RegisteredUser,PaidUser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,Email,Password,AccountFinishDate")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            var currentUserRole = HttpContext.Session.GetString("UserRole");
            var currentUserId = HttpContext.Session.GetInt32("ActiveUserId");

            if (currentUserRole != "Admin" && currentUserId != id)
            {
                return Forbid(); 
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }


        // GET: Users/Delete/5
        [Authorize(Roles = "Admin,RegisteredUser,PaidUser")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var currentUserRole = HttpContext.Session.GetString("UserRole");
            var currentUserId = HttpContext.Session.GetInt32("ActiveUserId");

            if (currentUserRole != "Admin" && currentUserId != id)
            {
                return Forbid(); 
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [Authorize(Roles = "Admin,RegisteredUser,PaidUser")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUserRole = HttpContext.Session.GetString("UserRole");
            var currentUserId = HttpContext.Session.GetInt32("ActiveUserId");

            if (currentUserRole != "Admin" && currentUserId != id)
            {
                return Forbid();
            }

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            if (currentUserRole != "Admin")

                 return RedirectToAction(nameof(Logout));

            else
                return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
