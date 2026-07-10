using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioSport.Data;
using BioSport.ViewModels;

namespace BioSport.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ReportesController : Controller
    {
        private readonly AppDbContext _context;

        public ReportesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var ahora = DateTime.Now;

            var model = new ReporteViewModel
            {
                TotalClientes = await _context.Usuarios.CountAsync(u => u.IdRol == 4),
                TotalEntrenadores = await _context.Usuarios.CountAsync(u => u.IdRol == 3),
                TotalRecepcionistas = await _context.Usuarios.CountAsync(u => u.IdRol == 2),
                TotalAdministradores = await _context.Usuarios.CountAsync(u => u.IdRol == 1),

                MembresiasActivas = await _context.Membresias.CountAsync(m => m.FechaVencimiento >= ahora),
                MembresiasVencidas = await _context.Membresias.CountAsync(m => m.FechaVencimiento < ahora),

                IngresosTotales = await _context.Pagos.SumAsync(p => (decimal?)p.Monto) ?? 0,

                AsistenciasHoy = await _context.Asistencias.CountAsync(a => a.FechaEntrada.Date == ahora.Date),

                PlanMasVendido = await _context.Membresias
                    .Include(m => m.Plan)
                    .GroupBy(m => m.Plan!.Nombre)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefaultAsync() ?? "N/A"
            };

            return View(model);
        }
    }
}
