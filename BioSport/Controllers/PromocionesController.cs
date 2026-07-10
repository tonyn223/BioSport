using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioSport.Data;
using BioSport.Models;

namespace BioSport.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class PromocionesController : Controller
    {
        private readonly AppDbContext _context;

        public PromocionesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var promociones = await _context.Promociones.ToListAsync();
            return View(promociones);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Promocion promocion)
        {
            if (ModelState.IsValid)
            {
                _context.Promociones.Add(promocion);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Promoción creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(promocion);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var promocion = await _context.Promociones.FindAsync(id);
            if (promocion == null)
            {
                return NotFound();
            }
            return View(promocion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Promocion promocion)
        {
            if (id != promocion.IdPromocion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(promocion);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Promoción actualizada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(promocion);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var promocion = await _context.Promociones.FirstOrDefaultAsync(p => p.IdPromocion == id);
            if (promocion == null)
            {
                return NotFound();
            }
            return View(promocion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var promocion = await _context.Promociones.FindAsync(id);
            if (promocion != null)
            {
                _context.Promociones.Remove(promocion);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Promoción eliminada exitosamente.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
