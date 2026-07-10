using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioSport.Data;
using BioSport.Models;

namespace BioSport.Controllers
{
    [Authorize(Roles = "Recepcionista,Administrador")]
    public class AsistenciaController : Controller
    {
        private readonly AppDbContext _context;

        public AsistenciaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Asistencia
        public async Task<IActionResult> Index()
        {
            var asistencias = await _context.Asistencias
                .Include(a => a.Usuario)
                .OrderByDescending(a => a.FechaEntrada)
                .ThenByDescending(a => a.HoraEntrada)
                .Take(50)
                .ToListAsync();

            return View(asistencias);
        }

        // GET: /Asistencia/Registrar
        public IActionResult Registrar()
        {
            return View();
        }

        // POST: /Asistencia/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(string codigoAcceso)
        {
            if (string.IsNullOrWhiteSpace(codigoAcceso))
            {
                ViewBag.Error = "Debes ingresar un código de acceso.";
                return View();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CodigoAcceso == codigoAcceso);

            if (usuario == null)
            {
                ViewBag.Error = "No se encontró ningún cliente con ese código de acceso.";
                return View();
            }

            var ahora = DateTime.Now;

            var asistencia = new Asistencia
            {
                IdUsuario = usuario.IdUsuario,
                FechaEntrada = ahora.Date,
                HoraEntrada = ahora.TimeOfDay
            };

            _context.Asistencias.Add(asistencia);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"Asistencia registrada para {usuario.Nombre} a las {ahora:HH:mm}.";
            return RedirectToAction(nameof(Registrar));
        }
    }
}
