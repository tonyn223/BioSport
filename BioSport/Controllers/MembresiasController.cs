using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioSport.Data;
using BioSport.Models;

namespace BioSport.Controllers
{
    [Authorize(Roles = "Recepcionista,Administrador")]
    public class MembresiasController : Controller
    {
        private readonly AppDbContext _context;

        public MembresiasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Membresias
        public async Task<IActionResult> Index()
        {
            var membresias = await _context.Membresias
                .Include(m => m.Usuario)
                .Include(m => m.Plan)
                .OrderByDescending(m => m.FechaInicio)
                .ToListAsync();

            return View(membresias);
        }

        // GET: /Membresias/Crear
        public IActionResult Crear()
        {
            ViewBag.Planes = _context.Planes.ToList();
            return View();
        }

        // POST: /Membresias/Buscar (busca cliente por código de acceso o email)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(string busqueda, int idPlan, string metodoPago)
        {
            ViewBag.Planes = _context.Planes.ToList();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CodigoAcceso == busqueda || u.Email == busqueda);

            if (usuario == null)
            {
                ViewBag.Error = "No se encontró ningún cliente con ese código o correo.";
                return View();
            }

            var plan = await _context.Planes.FindAsync(idPlan);
            if (plan == null)
            {
                ViewBag.Error = "Plan no válido.";
                return View();
            }

            var fechaInicio = DateTime.Now;
            var fechaVencimiento = fechaInicio.AddDays(plan.DuracionDias);

            var membresia = new Membresia
            {
                IdUsuario = usuario.IdUsuario,
                IdPlan = plan.IdPlan,
                FechaInicio = fechaInicio,
                FechaVencimiento = fechaVencimiento,
                Estado = "Activo"
            };

            _context.Membresias.Add(membresia);
            await _context.SaveChangesAsync();

            var pago = new Pago
            {
                IdUsuario = usuario.IdUsuario,
                IdMembresia = membresia.IdMembresia,
                Monto = plan.Precio,
                FechaPago = fechaInicio,
                Metodo = metodoPago,
                Estado = "Completado"
            };

            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"Membresía '{plan.Nombre}' asignada a {usuario.Nombre}. Pago de ₡{plan.Precio:N0} registrado.";
            return RedirectToAction(nameof(Index));
        }
    }
}
