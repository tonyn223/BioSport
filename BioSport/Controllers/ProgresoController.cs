using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BioSport.Data;
using BioSport.Models;

namespace BioSport.Controllers
{
    [Authorize(Roles = "Entrenador,Administrador")]
    public class ProgresoController : Controller
    {
        private readonly AppDbContext _context;

        public ProgresoController(AppDbContext context)
        {
            _context = context;
        }

        private int ObtenerIdUsuarioActual()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(idClaim!);
        }

        // GET: /Progreso
        public async Task<IActionResult> Index()
        {
            var idEntrenador = ObtenerIdUsuarioActual();

            var registros = await _context.Progresos
                .Include(p => p.Cliente)
                .Where(p => p.IdEntrenador == idEntrenador)
                .OrderByDescending(p => p.FechaRegistro)
                .ToListAsync();

            return View(registros);
        }

        // GET: /Progreso/Create
        public async Task<IActionResult> Create()
        {
            var idEntrenador = ObtenerIdUsuarioActual();

            var clientes = await _context.RutinasAsignadas
                .Include(ra => ra.Cliente)
                .Where(ra => ra.Rutina!.IdEntrenador == idEntrenador)
                .Select(ra => ra.Cliente)
                .Distinct()
                .ToListAsync();

            ViewBag.Clientes = clientes;
            return View();
        }

        // POST: /Progreso/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Progreso progreso)
        {
            progreso.IdEntrenador = ObtenerIdUsuarioActual();
            progreso.FechaRegistro = DateTime.Now;

            ModelState.Remove("IdEntrenador");
            ModelState.Remove("Entrenador");
            ModelState.Remove("Cliente");

            if (ModelState.IsValid)
            {
                _context.Progresos.Add(progreso);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Progreso registrado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            var idEntrenador = ObtenerIdUsuarioActual();
            ViewBag.Clientes = await _context.RutinasAsignadas
                .Include(ra => ra.Cliente)
                .Where(ra => ra.Rutina!.IdEntrenador == idEntrenador)
                .Select(ra => ra.Cliente)
                .Distinct()
                .ToListAsync();

            return View(progreso);
        }

        // GET: /Progreso/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var idEntrenador = ObtenerIdUsuarioActual();
            var progreso = await _context.Progresos
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.IdProgreso == id && p.IdEntrenador == idEntrenador);

            if (progreso == null)
            {
                return NotFound();
            }

            return View(progreso);
        }

        // POST: /Progreso/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Progreso progreso)
        {
            var idEntrenador = ObtenerIdUsuarioActual();

            if (id != progreso.IdProgreso)
            {
                return NotFound();
            }

            var registroExistente = await _context.Progresos
                .FirstOrDefaultAsync(p => p.IdProgreso == id && p.IdEntrenador == idEntrenador);

            if (registroExistente == null)
            {
                return NotFound();
            }

            registroExistente.Peso = progreso.Peso;
            registroExistente.Observaciones = progreso.Observaciones;

            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Progreso actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Progreso/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var idEntrenador = ObtenerIdUsuarioActual();
            var progreso = await _context.Progresos
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.IdProgreso == id && p.IdEntrenador == idEntrenador);

            if (progreso == null)
            {
                return NotFound();
            }

            return View(progreso);
        }

        // POST: /Progreso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var idEntrenador = ObtenerIdUsuarioActual();
            var progreso = await _context.Progresos
                .FirstOrDefaultAsync(p => p.IdProgreso == id && p.IdEntrenador == idEntrenador);

            if (progreso != null)
            {
                _context.Progresos.Remove(progreso);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Registro de progreso eliminado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
