using System.ComponentModel.DataAnnotations;

namespace BioSport.ViewModels
{
    public class CambiarContrasenaViewModel
    {
        [Required(ErrorMessage = "El código de acceso es requerido")]
        [Display(Name = "Código de Acceso")]
        public string CodigoAcceso { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [Display(Name = "Nueva Contraseña")]
        public string NuevaContrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debes confirmar la nueva contraseña")]
        [DataType(DataType.Password)]
        [Compare(nameof(NuevaContrasena), ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmar Nueva Contraseña")]
        public string ConfirmarContrasena { get; set; } = string.Empty;
    }
}
