using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BioSport.Data;
using BioSport.Models;
using BioSport.ViewModels;
using BCrypt.Net;

namespace BioSport.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(model.Password, usuario.Contrasena))
            {
                ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
                return View(model);
            }

            await IniciarSesion(usuario, model.RememberMe);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        // GET: /Account/OlvideContrasena
        public IActionResult OlvideContrasena()
        {
            return View();
        }

        // POST: /Account/OlvideContrasena
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OlvideContrasena(OlvidoContrasenaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.CodigoAcceso == model.CodigoAcceso);

            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "El correo y el código de acceso no coinciden con ningún usuario.");
                return View(model);
            }

            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(model.NuevaContrasena);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Tu contraseña se actualizó correctamente. Ya puedes iniciar sesión.";
            return RedirectToAction(nameof(Login));
        }

        // GET: /Account/CambiarContrasena
        [Authorize]
        public IActionResult CambiarContrasena()
        {
            return View();
        }

        // POST: /Account/CambiarContrasena
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarContrasena(CambiarContrasenaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var idUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var usuario = await _context.Usuarios.FindAsync(idUsuario);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(model.ContrasenaActual, usuario.Contrasena))
            {
                ModelState.AddModelError(nameof(model.ContrasenaActual), "La contraseña actual es incorrecta.");
                return View(model);
            }

            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(model.NuevaContrasena);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Tu contraseña se actualizó correctamente.";
            return RedirectToAction(nameof(CambiarContrasena));
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Método auxiliar para iniciar sesión
        private async Task IniciarSesion(Usuario usuario, bool rememberMe = false)
        {
            var nombreRol = usuario.Rol?.NombreRol ?? "Cliente";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, nombreRol)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}
