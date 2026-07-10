using System.ComponentModel.DataAnnotations;

namespace BioSport.ViewModels
{
    public class UsuarioViewModel
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "El rol es requerido")]
        [Display(Name = "Rol")]
        public int IdRol { get; set; }

        // Solo se usa al crear un nuevo usuario
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña (dejar en blanco para no cambiar)")]
        public string? Password { get; set; }
    }
}
