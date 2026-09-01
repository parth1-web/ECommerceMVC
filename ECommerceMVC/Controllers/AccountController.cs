using System.Security.Claims;
using ECommerceMVC.Models;
using ECommerceMVC.Models.Api;
using ECommerceMVC.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthApiService _authApiService;

        public AccountController(
            IAuthApiService authApiService)
        {
            _authApiService = authApiService;
        }

        // ==================================================
        // LOGIN - GET
        // ==================================================

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            return View();
        }

        // ==================================================
        // LOGIN - POST
        // ==================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var request = new LoginRequestDto
            {
                Email = model.Email,
                Password = model.Password
            };

            var response =
                await _authApiService.LoginAsync(request);

            if (response == null ||
                string.IsNullOrWhiteSpace(response.AccessToken))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password.");

                return View(model);
            }

            // ==================================================
            // STORE JWT IN SESSION
            // ==================================================

            HttpContext.Session.SetString(
                "AccessToken",
                response.AccessToken);

            // ==================================================
            // CREATE CLAIMS FROM JWT
            // ==================================================

            var claims =
                CreateClaims(response.AccessToken);

            // Remove an existing AccessToken claim if the
            // backend ever happens to include one.
            claims.RemoveAll(
                c => c.Type == "AccessToken");

            // Store JWT inside authentication claims.
            claims.Add(
                new Claim(
                    "AccessToken",
                    response.AccessToken));

            // ==================================================
            // ENSURE USER EMAIL / NAME EXISTS
            // ==================================================

            if (!claims.Any(
                c => c.Type == ClaimTypes.Email))
            {
                claims.Add(
                    new Claim(
                        ClaimTypes.Email,
                        model.Email));
            }

            if (!claims.Any(
                c => c.Type == ClaimTypes.Name))
            {
                claims.Add(
                    new Claim(
                        ClaimTypes.Name,
                        model.Email));
            }

            // ==================================================
            // CREATE COOKIE IDENTITY
            // ==================================================

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults
                    .AuthenticationScheme);

            var principal =
                new ClaimsPrincipal(identity);

            // ==================================================
            // SIGN IN MVC USER
            // ==================================================

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme,
                principal);

            TempData["SuccessMessage"] =
                "Login successful.";

            return RedirectToAction(
                "Index",
                "Home");
        }

        // ==================================================
        // REGISTER - GET
        // ==================================================

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            return View();
        }

        // ==================================================
        // REGISTER - POST
        // ==================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var request = new RegisterRequestDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password
            };

            var success =
                await _authApiService.RegisterAsync(request);

            if (!success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Registration failed. " +
                    "The email may already be registered.");

                return View(model);
            }

            TempData["SuccessMessage"] =
                "Registration successful. Please login.";

            return RedirectToAction(
                "Login",
                "Account");
        }

        // ==================================================
        // LOGOUT
        // ==================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Remove JWT from session.
            HttpContext.Session.Remove(
                "AccessToken");

            // Remove MVC authentication cookie.
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme);

            TempData["SuccessMessage"] =
                "You have been logged out successfully.";

            return RedirectToAction(
                "Index",
                "Home");
        }

        // ==================================================
        // ACCESS DENIED
        // ==================================================

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // ==================================================
        // CREATE CLAIMS FROM JWT
        // ==================================================

        private static List<Claim> CreateClaims(
            string accessToken)
        {
            var handler =
                new System.IdentityModel.Tokens.Jwt
                    .JwtSecurityTokenHandler();

            var token =
                handler.ReadJwtToken(accessToken);

            return token.Claims.ToList();
        }
    }
}