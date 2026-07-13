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

            // Cada usuario debe tener una sola membresía: si ya tiene una, se renueva/actualiza
            // en vez de crear una fila nueva (los pagos sí se acumulan como historial).
            var membresia = await _context.Membresias
                .FirstOrDefaultAsync(m => m.IdUsuario == usuario.IdUsuario);

            // Si la membresía actual sigue vigente, la renovación extiende desde su vencimiento
            // en vez de la fecha de hoy, para no perder los días ya pagados.
            var fechaBase = (membresia != null && membresia.FechaVencimiento > fechaInicio)
                ? membresia.FechaVencimiento
                : fechaInicio;
            var fechaVencimiento = fechaBase.AddDays(plan.DuracionDias);

            if (membresia == null)
            {
                membresia = new Membresia
                {
                    IdUsuario = usuario.IdUsuario,
                    IdPlan = plan.IdPlan,
                    FechaInicio = fechaInicio,
                    FechaVencimiento = fechaVencimiento,
                    Estado = "Activo"
                };
                _context.Membresias.Add(membresia);
            }
            else
            {
                membresia.IdPlan = plan.IdPlan;
                membresia.FechaInicio = fechaInicio;
                membresia.FechaVencimiento = fechaVencimiento;
                membresia.Estado = "Activo";
            }

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

        // GET: /Membresias/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var membresia = await _context.Membresias
                .Include(m => m.Usuario)
                .FirstOrDefaultAsync(m => m.IdMembresia == id);

            if (membresia == null)
            {
                return NotFound();
            }

            ViewBag.Planes = _context.Planes.ToList();
            return View(membresia);
        }

        // POST: /Membresias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Membresia membresia)
        {
            if (id != membresia.IdMembresia)
            {
                return NotFound();
            }

            ModelState.Remove("Usuario");
            ModelState.Remove("Plan");

            if (ModelState.IsValid)
            {
                var existente = await _context.Membresias.FindAsync(id);
                if (existente == null)
                {
                    return NotFound();
                }

                existente.IdPlan = membresia.IdPlan;
                existente.FechaInicio = membresia.FechaInicio;
                existente.FechaVencimiento = membresia.FechaVencimiento;
                existente.Estado = membresia.Estado;

                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Membresía actualizada exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Planes = _context.Planes.ToList();
            return View(membresia);
        }
    }
}
