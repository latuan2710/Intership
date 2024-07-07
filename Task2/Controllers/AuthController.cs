using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;
using Task2.Data;
using Task2.Models;
using Task2.Models.ViewModel;
using Task2.Services;

namespace Task2.Controllers
{
    public class AuthController : Controller
    {
        private readonly Task2Context _context;

        public AuthController(Task2Context context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (IsEmailExists(model.Email))
                {
                    ModelState.AddModelError("", "Email was exist!");
                    return View(model);
                }

                if (IsUsernameExists(model.Username))
                {
                    ModelState.AddModelError("", "Username was exist!");
                    return View(model);
                }

                var role = _context.Role.FirstOrDefault(r => r.RoleName.Equals("User"));

                var user = new User
                {
                    UserEmail = model.Email,
                    UserName = model.Username,
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    RoleId = role.RoleId,
                    UserHashPassword = PasswordHasher.HashPassword(model.Password)
                };

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Login));
            }
            return View(model);
        }

        private bool IsUsernameExists(string username)
        {
            return _context.User.Any(u => u.UserName.Equals(username));
        }

        private bool IsEmailExists(string email)
        {
            return _context.User.Any(u => u.UserEmail.Equals(email));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = _context.User
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.UserName.Equals(model.Username) || u.UserEmail.Equals(model.Username));

                if (user == null)
                {
                    ModelState.AddModelError("", "Email or Username was not exist!");
                    return View(model);
                }

                if (PasswordHasher.VerifyPassword(model.Password, user.UserHashPassword))
                {
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, user.UserEmail),
                        new(ClaimTypes.Role, user.Role.RoleName),
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = DateTime.Now.AddHours(1)
                        });

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Password was incorrect!");
                    return View();
                }

            }
            ModelState.AddModelError("", "Email or Password was incorrect!");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string returnUrl = "")
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect(returnUrl);
        }
    }
}
