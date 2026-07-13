using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioSport.Data;
using BioSport.Models;

namespace BioSport.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class HorariosController : Controller
    {
        private readonly AppDbContext _context;

        public HorariosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var horarios = await _context.Horarios.ToListAsync();
            return View(horarios);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Horario horario)
        {
            if (ModelState.IsValid)
            {
                _context.Horarios.Add(horario);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Horario creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(horario);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var horario = await _context.Horarios.FindAsync(id);
            if (horario == null)
            {
                return NotFound();
            }
            return View(horario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Horario horario)
        {
            if (id != horario.IdHorario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(horario);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Horario actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(horario);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var horario = await _context.Horarios.FirstOrDefaultAsync(h => h.IdHorario == id);
            if (horario == null)
            {
                return NotFound();
            }
            return View(horario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var horario = await _context.Horarios.FindAsync(id);
            if (horario != null)
            {
                try
                {
                    _context.Horarios.Remove(horario);
                    await _context.SaveChangesAsync();
                    TempData["Mensaje"] = "Horario eliminado exitosamente.";
                }
                catch (DbUpdateException)
                {
                    TempData["Error"] = "No se pudo eliminar este horario porque tiene registros asociados.";
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
