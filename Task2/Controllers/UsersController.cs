using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Task2.Data;
using Task2.Models;
using Task2.Models.ViewModel;
using Task2.Services;

namespace Task2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly Task2Context _context;

        public UsersController(Task2Context context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var task2Context = _context.User.Include(u => u.Role);
            return View(await task2Context.ToListAsync());
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(_context.Role, "RoleId", "RoleName");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Password,ConfirmedPassword,Email")] RegisterViewModel model, byte RoleId)
        {
            if (ModelState.IsValid)
            {
                var user = _context.User
                    .FirstOrDefault(u => u.UserName.Equals(model.Username) || u.UserEmail.Equals(model.Email));

                if (user != null)
                {
                    ModelState.AddModelError("", "Email or Username was exist!");
                    ViewBag.Roles = new SelectList(_context.Role, "RoleId", "RoleName");
                    return View(model);
                }

                user = new User
                {
                    UserEmail = model.Email,
                    UserName = model.Username,
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    RoleId = RoleId,
                    UserHashPassword = PasswordHasher.HashPassword(model.Password)
                };

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Roles = new SelectList(_context.Role, "RoleId", "RoleName");
            return View(model);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.Roles = new SelectList(_context.Role, "RoleId", "RoleName");
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("UserId,UserName,RoleId,UserEmail")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var mainUser = await _context.User.FindAsync(id);
                    if (IsEmailExists(user.UserEmail, id))
                    {
                        ModelState.AddModelError("", "Email was exist!");
                        ViewBag.Roles = new SelectList(_context.Role, "RoleId", "RoleName");
                        return View(user);
                    }
                    else
                    {
                        mainUser.UserName = user.UserName;
                    }

                    if (IsUsernameExists(user.UserName, id))
                    {
                        ModelState.AddModelError("", "Username was exist!");
                        ViewBag.Roles = new SelectList(_context.Role, "RoleId", "RoleName");
                        return View(user);
                    }
                    else
                    {
                        mainUser.UserEmail = user.UserEmail;
                    }

                    mainUser.RoleId = user.RoleId;
                    mainUser.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            ViewBag.Roles = new SelectList(_context.Role, "RoleId", "RoleName");
            return View(user);
        }



        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                _context.User.Remove(user);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(long id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
        private bool IsUsernameExists(string username, long originalUserId)
        {
            return _context.User.Any(u => u.UserName.Equals(username) && u.UserId != originalUserId);
        }

        private bool IsEmailExists(string email, long originalUserId)
        {
            return _context.User.Any(u => u.UserEmail.Equals(email) && u.UserId != originalUserId);
        }
    }
}
