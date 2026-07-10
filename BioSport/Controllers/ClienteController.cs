using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BioSport.Data;

namespace BioSport.Controllers
{
    [Authorize(Roles = "Cliente,Administrador")]
    public class ClienteController : Controller
    {
        private readonly AppDbContext _context;

        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        private int ObtenerIdUsuarioActual()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(idClaim!);
        }

        // GET: /Cliente/MiMembresia
        public async Task<IActionResult> MiMembresia()
        {
            var idUsuario = ObtenerIdUsuarioActual();

            var membresia = await _context.Membresias
                .Include(m => m.Plan)
                .Where(m => m.IdUsuario == idUsuario)
                .OrderByDescending(m => m.FechaInicio)
                .FirstOrDefaultAsync();

            var usuario = await _context.Usuarios.FindAsync(idUsuario);
            ViewBag.CodigoAcceso = usuario?.CodigoAcceso;

            return View(membresia);
        }

        // GET: /Cliente/MisRutinas
        public async Task<IActionResult> MisRutinas()
        {
            var idUsuario = ObtenerIdUsuarioActual();

            var rutinas = await _context.RutinasAsignadas
                .Include(ra => ra.Rutina)
                .ThenInclude(r => r!.Entrenador)
                .Where(ra => ra.IdCliente == idUsuario)
                .OrderByDescending(ra => ra.FechaAsignacion)
                .ToListAsync();

            return View(rutinas);
        }

        // GET: /Cliente/MiProgreso
        public async Task<IActionResult> MiProgreso()
        {
            var idUsuario = ObtenerIdUsuarioActual();

            var progreso = await _context.Progresos
                .Include(p => p.Entrenador)
                .Where(p => p.IdCliente == idUsuario)
                .OrderByDescending(p => p.FechaRegistro)
                .ToListAsync();

            return View(progreso);
        }

        // GET: /Cliente/MisPagos
        public async Task<IActionResult> MisPagos()
        {
            var idUsuario = ObtenerIdUsuarioActual();

            var pagos = await _context.Pagos
                .Include(p => p.Membresia)
                .ThenInclude(m => m!.Plan)
                .Where(p => p.IdUsuario == idUsuario)
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();

            return View(pagos);
        }
    }
}
