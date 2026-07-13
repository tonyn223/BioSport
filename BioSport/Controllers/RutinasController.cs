using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BioSport.Data;
using BioSport.Models;

namespace BioSport.Controllers
{
    [Authorize(Roles = "Entrenador,Administrador")]
    public class RutinasController : Controller
    {
        private readonly AppDbContext _context;

        public RutinasController(AppDbContext context)
        {
            _context = context;
        }

        private int ObtenerIdUsuarioActual()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(idClaim!);
        }

        // GET: /Rutinas
        public async Task<IActionResult> Index()
        {
            var idEntrenador = ObtenerIdUsuarioActual();

            var rutinas = await _context.Rutinas
                .Where(r => r.IdEntrenador == idEntrenador)
                .OrderByDescending(r => r.FechaCreacion)
                .ToListAsync();

            return View(rutinas);
        }

        // GET: /Rutinas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Rutinas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Rutina rutina)
        {
            rutina.IdEntrenador = ObtenerIdUsuarioActual();
            rutina.FechaCreacion = DateTime.Now;

            ModelState.Remove("IdEntrenador");
            ModelState.Remove("Entrenador");

            if (ModelState.IsValid)
            {
                _context.Rutinas.Add(rutina);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Rutina creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(rutina);
        }

        // GET: /Rutinas/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var idEntrenador = ObtenerIdUsuarioActual();
            var rutina = await _context.Rutinas
                .FirstOrDefaultAsync(r => r.IdRutina == id && r.IdEntrenador == idEntrenador);

            if (rutina == null)
            {
                return NotFound();
            }

            return View(rutina);
        }

        // POST: /Rutinas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Rutina rutina)
        {
            var idEntrenador = ObtenerIdUsuarioActual();

            if (id != rutina.IdRutina)
            {
                return NotFound();
            }

            ModelState.Remove("IdEntrenador");
            ModelState.Remove("Entrenador");

            if (ModelState.IsValid)
            {
                var rutinaExistente = await _context.Rutinas
                    .FirstOrDefaultAsync(r => r.IdRutina == id && r.IdEntrenador == idEntrenador);

                if (rutinaExistente == null)
                {
                    return NotFound();
                }

                rutinaExistente.Nombre = rutina.Nombre;
                rutinaExistente.Descripcion = rutina.Descripcion;

                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Rutina actualizada exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(rutina);
        }

        // GET: /Rutinas/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var idEntrenador = ObtenerIdUsuarioActual();
            var rutina = await _context.Rutinas
                .FirstOrDefaultAsync(r => r.IdRutina == id && r.IdEntrenador == idEntrenador);

            if (rutina == null)
            {
                return NotFound();
            }

            return View(rutina);
        }

        // POST: /Rutinas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var idEntrenador = ObtenerIdUsuarioActual();
            var rutina = await _context.Rutinas
                .FirstOrDefaultAsync(r => r.IdRutina == id && r.IdEntrenador == idEntrenador);

            if (rutina != null)
            {
                try
                {
                    // Elimina primero las asignaciones relacionadas
                    var asignaciones = _context.RutinasAsignadas.Where(ra => ra.IdRutina == id);
                    _context.RutinasAsignadas.RemoveRange(asignaciones);

                    _context.Rutinas.Remove(rutina);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Rutina eliminada exitosamente.";
                }
                catch (DbUpdateException)
                {
                    TempData["Error"] = "No se pudo eliminar esta rutina porque tiene registros asociados.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Rutinas/Asignar/5
        public async Task<IActionResult> Asignar(int id)
        {
            var idEntrenador = ObtenerIdUsuarioActual();
            var rutina = await _context.Rutinas
                .FirstOrDefaultAsync(r => r.IdRutina == id && r.IdEntrenador == idEntrenador);

            if (rutina == null)
            {
                return NotFound();
            }

            ViewBag.Rutina = rutina;
            ViewBag.Clientes = await _context.Usuarios
                .Where(u => u.IdRol == 4)
                .OrderBy(u => u.Nombre)
                .ToListAsync();

            return View();
        }

        // POST: /Rutinas/Asignar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Asignar(int id, int idCliente)
        {
            var idEntrenador = ObtenerIdUsuarioActual();
            var rutina = await _context.Rutinas
                .FirstOrDefaultAsync(r => r.IdRutina == id && r.IdEntrenador == idEntrenador);

            if (rutina == null)
            {
                return NotFound();
            }

            var yaAsignada = await _context.RutinasAsignadas
                .AnyAsync(ra => ra.IdRutina == id && ra.IdCliente == idCliente);

            if (yaAsignada)
            {
                TempData["Mensaje"] = "Esta rutina ya estaba asignada a ese cliente.";
                return RedirectToAction(nameof(Index));
            }

            var asignacion = new RutinaAsignada
            {
                IdRutina = id,
                IdCliente = idCliente,
                FechaAsignacion = DateTime.Now
            };

            _context.RutinasAsignadas.Add(asignacion);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Rutina asignada exitosamente al cliente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Rutinas/MisClientes
        public async Task<IActionResult> MisClientes()
        {
            var idEntrenador = ObtenerIdUsuarioActual();

            var asignaciones = await _context.RutinasAsignadas
                .Include(ra => ra.Rutina)
                .Include(ra => ra.Cliente)
                .Where(ra => ra.Rutina!.IdEntrenador == idEntrenador)
                .OrderByDescending(ra => ra.FechaAsignacion)
                .ToListAsync();

            return View(asignaciones);
        }

        // POST: /Rutinas/Desasignar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desasignar(int id)
        {
            var idEntrenador = ObtenerIdUsuarioActual();

            var asignacion = await _context.RutinasAsignadas
                .Include(ra => ra.Rutina)
                .FirstOrDefaultAsync(ra => ra.IdAsignacion == id && ra.Rutina!.IdEntrenador == idEntrenador);

            if (asignacion != null)
            {
                _context.RutinasAsignadas.Remove(asignacion);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Rutina desasignada del cliente exitosamente.";
            }

            return RedirectToAction(nameof(MisClientes));
        }
    }
}
