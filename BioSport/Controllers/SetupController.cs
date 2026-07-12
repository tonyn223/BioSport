using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BioSport.Data;
using BioSport.Models;
using BioSport.ViewModels;

namespace BioSport.Controllers
{
    // Permite crear el primer usuario Administrador cuando la base de datos no tiene ninguno.
    // La acción se desactiva sola en cuanto exista un Administrador.
    public class SetupController : Controller
    {
        private readonly AppDbContext _context;

        public SetupController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Setup
        public async Task<IActionResult> Index()
        {
            if (await _context.Usuarios.AnyAsync(u => u.IdRol == 1))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // POST: /Setup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SetupAdminViewModel model)
        {
            if (await _context.Usuarios.AnyAsync(u => u.IdRol == 1))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existeUsuario = await _context.Usuarios.AnyAsync(u => u.Email == model.Email);
            if (existeUsuario)
            {
                ModelState.AddModelError("Email", "Este correo ya está registrado.");
                return View(model);
            }

            var random = new Random();
            var usuario = new Usuario
            {
                Nombre = model.Nombre,
                Email = model.Email,
                Contrasena = BCrypt.Net.BCrypt.HashPassword(model.Password),
                CodigoAcceso = random.Next(1000, 9999).ToString(),
                IdRol = 1, // Administrador
                FechaCreacion = DateTime.Now
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, "Administrador")
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) });

            TempData["Mensaje"] = $"Cuenta de administrador creada. ¡Bienvenido, {usuario.Nombre}!";
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
