using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioSport.Data;
using BioSport.Models;

namespace BioSport.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class PlanesController : Controller
    {
        private readonly AppDbContext _context;

        public PlanesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Planes
        public async Task<IActionResult> Index()
        {
            var planes = await _context.Planes.ToListAsync();
            return View(planes);
        }

        // GET: /Planes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Planes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Plan plan)
        {
            if (ModelState.IsValid)
            {
                _context.Planes.Add(plan);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Plan creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        // GET: /Planes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var plan = await _context.Planes.FindAsync(id);
            if (plan == null)
            {
                return NotFound();
            }
            return View(plan);
        }

        // POST: /Planes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Plan plan)
        {
            if (id != plan.IdPlan)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(plan);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Plan actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        // GET: /Planes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var plan = await _context.Planes.FirstOrDefaultAsync(p => p.IdPlan == id);
            if (plan == null)
            {
                return NotFound();
            }
            return View(plan);
        }

        // POST: /Planes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plan = await _context.Planes.FindAsync(id);
            if (plan != null)
            {
                _context.Planes.Remove(plan);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Plan eliminado exitosamente.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
