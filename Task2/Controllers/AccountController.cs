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
    [Authorize]
    public class AccountController : Controller
    {
        private readonly Task2Context _context;

        public AccountController(Task2Context context)
        {
            _context = context;
        }


        public async Task<IActionResult> Profile()
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.UserEmail.Equals(User.Identity.Name));
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile([Bind("UserId,UserName")] User model)
        {
            if (ModelState.IsValid)
            {
                var mainUser = await _context.User.FirstOrDefaultAsync(u => u.UserEmail.Equals(User.Identity.Name));
                try
                {
                    if (IsUsernameExists(model.UserName, mainUser.UserId))
                    {
                        ModelState.AddModelError("", "Username was exist!");
                        return View(mainUser);
                    }
                    else
                    {
                        mainUser.UserName = model.UserName;
                    }

                    mainUser.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(model.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return View(mainUser);
            }

            return View(model);
        }

        public IActionResult ResetPassword()
        {
            var model = new ResetPasswordViewModel
            {
                OldPassword = string.Empty,
                NewPassword = string.Empty,
                ConfirmPassword = string.Empty,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var mainUser = await _context.User.FirstOrDefaultAsync(u => u.UserEmail.Equals(User.Identity.Name));

                if (PasswordHasher.VerifyPassword(model.OldPassword, mainUser.UserHashPassword))
                {
                    mainUser.UserHashPassword = PasswordHasher.HashPassword(model.NewPassword);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "The password was incorrect!");
                    return View(model);
                }

            }
            ModelState.AddModelError("", "Something was wrong!");
            return View(model);
        }

        private bool UserExists(long id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
        private bool IsUsernameExists(string username, long originalUserId)
        {
            return _context.User.Any(u => u.UserName.Equals(username) && u.UserId != originalUserId);
        }
    }
}
