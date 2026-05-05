using System.Security.Claims;
using MediLink.Interfaces;
using MediLink.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace MediLink.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult ChooseRegister()
        {
            return View();
        }

        public IActionResult ChooseLogin()
        {
            return View();
        }

        public IActionResult RegisterPatient()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPatient(RegisterPatientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _authService.RegisterPatientAsync(model);
                return RedirectToAction(nameof(LoginPatient));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        
        public IActionResult LoginPatient()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginPatient(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _authService.ValidateUserAsync(model.Email, model.Password, "Patient");

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid patient login details.");
                return View(model);
            }

            await SignInUserAsync(user);
            return RedirectToAction("PatientDashboard", "Dashboard");
        }

        public IActionResult LoginDoctor()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginDoctor(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _authService.ValidateUserAsync(model.Email, model.Password, "Doctor");

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid doctor login details.");
                return View(model);
            }

            await SignInUserAsync(user);
            return RedirectToAction("DoctorDashboard", "Dashboard");
        }

        public IActionResult LoginAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAdmin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _authService.ValidateUserAsync(model.Email, model.Password, "Admin");

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid admin login details.");
                return View(model);
            }

            await SignInUserAsync(user);
            return RedirectToAction("AdminDashboard", "Dashboard");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(ChooseLogin));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private async Task SignInUserAsync(Models.User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString())
            };

            if (user.PatientId.HasValue)
            {
                claims.Add(new Claim("PatientId", user.PatientId.Value.ToString()));
            }

            if (user.DoctorId.HasValue)
            {
                claims.Add(new Claim("DoctorId", user.DoctorId.Value.ToString()));
            }

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);
        }
    }
}