using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioSport.Data;
using BioSport.Models;
using BioSport.ViewModels;

namespace BioSport.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol)
                .OrderBy(u => u.Nombre)
                .ToListAsync();
            return View(usuarios);
        }

        // GET: /Usuarios/Create
        public IActionResult Create()
        {
            ViewBag.Roles = _context.Roles.ToList();
            return View();
        }

        // POST: /Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioViewModel model)
        {
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("Password", "La contraseña es requerida al crear un usuario.");
            }

            var existeUsuario = await _context.Usuarios.AnyAsync(u => u.Email == model.Email);
            if (existeUsuario)
            {
                ModelState.AddModelError("Email", "Este correo ya está registrado.");
            }

            if (ModelState.IsValid)
            {
                var random = new Random();
                var usuario = new Usuario
                {
                    Nombre = model.Nombre,
                    Email = model.Email,
                    Telefono = model.Telefono,
                    IdRol = model.IdRol,
                    Contrasena = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    CodigoAcceso = random.Next(1000, 9999).ToString(),
                    FechaCreacion = DateTime.Now
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Usuario creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Roles = _context.Roles.ToList();
            return View(model);
        }

        // GET: /Usuarios/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var model = new UsuarioViewModel
            {
                IdUsuario = usuario.IdUsuario,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Telefono = usuario.Telefono,
                IdRol = usuario.IdRol
            };

            ViewBag.Roles = _context.Roles.ToList();
            return View(model);
        }

        // POST: /Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioViewModel model)
        {
            if (id != model.IdUsuario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound();
                }

                usuario.Nombre = model.Nombre;
                usuario.Email = model.Email;
                usuario.Telefono = model.Telefono;
                usuario.IdRol = model.IdRol;

                // Solo actualiza la contraseña si se escribió una nueva
                if (!string.IsNullOrEmpty(model.Password))
                {
                    usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(model.Password);
                }

                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Usuario actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Roles = _context.Roles.ToList();
            return View(model);
        }

        // GET: /Usuarios/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: /Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                using var transaccion = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Rutinas asignadas donde el usuario es el cliente
                    await _context.RutinasAsignadas
                        .Where(ra => ra.IdCliente == id)
                        .ExecuteDeleteAsync();

                    // Progreso donde el usuario es cliente o entrenador
                    await _context.Progresos
                        .Where(p => p.IdCliente == id || p.IdEntrenador == id)
                        .ExecuteDeleteAsync();

                    // Rutinas creadas por el usuario como entrenador: primero sus asignaciones a clientes
                    var idsRutinasPropias = await _context.Rutinas
                        .Where(r => r.IdEntrenador == id)
                        .Select(r => r.IdRutina)
                        .ToListAsync();

                    if (idsRutinasPropias.Count > 0)
                    {
                        await _context.RutinasAsignadas
                            .Where(ra => idsRutinasPropias.Contains(ra.IdRutina))
                            .ExecuteDeleteAsync();

                        await _context.Rutinas
                            .Where(r => idsRutinasPropias.Contains(r.IdRutina))
                            .ExecuteDeleteAsync();
                    }

                    // Pagos, membresías y asistencia del usuario
                    await _context.Pagos.Where(p => p.IdUsuario == id).ExecuteDeleteAsync();
                    await _context.Membresias.Where(m => m.IdUsuario == id).ExecuteDeleteAsync();
                    await _context.Asistencias.Where(a => a.IdUsuario == id).ExecuteDeleteAsync();

                    _context.Usuarios.Remove(usuario);
                    await _context.SaveChangesAsync();

                    await transaccion.CommitAsync();
                    TempData["Mensaje"] = "Usuario y todos sus registros asociados fueron eliminados exitosamente.";
                }
                catch (DbUpdateException)
                {
                    await transaccion.RollbackAsync();
                    TempData["Error"] = "No se pudo eliminar este usuario debido a un error inesperado.";
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
