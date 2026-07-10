using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioSport.Data;
using BioSport.Models;

namespace BioSport.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var horarios = await _context.Horarios.ToListAsync();
            var planes = await _context.Planes.ToListAsync();

            ViewBag.Horarios = horarios;
            ViewBag.Planes = planes;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
