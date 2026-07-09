using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BioSport.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            if (User.IsInRole("Administrador"))
                return RedirectToAction("AdminDashboard");
            else if (User.IsInRole("Recepcionista"))
                return RedirectToAction("RecepcionistaDashboard");
            else if (User.IsInRole("Entrenador"))
                return RedirectToAction("EntrenadorDashboard");
            else
                return RedirectToAction("ClienteDashboard");
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult AdminDashboard()
        {
            ViewBag.Title = "Panel Administrativo";
            return View();
        }

        [Authorize(Roles = "Recepcionista")]
        public IActionResult RecepcionistaDashboard()
        {
            ViewBag.Title = "Panel Recepcionista";
            return View();
        }

        [Authorize(Roles = "Entrenador")]
        public IActionResult EntrenadorDashboard()
        {
            ViewBag.Title = "Panel Entrenador";
            return View();
        }

        [Authorize(Roles = "Cliente")]
        public IActionResult ClienteDashboard()
        {
            ViewBag.Title = "Mi Panel";
            return View();
        }
    }
}
