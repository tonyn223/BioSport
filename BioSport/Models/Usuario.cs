 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioSport.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("contraseña")]
        public string Contrasena { get; set; } = string.Empty;

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("codigo_acceso")]
        public string? CodigoAcceso { get; set; }

        [Column("id_rol")]
        public int IdRol { get; set; }

        [Column("fecha_creacion")]
        public DateTime? FechaCreacion { get; set; }

        // Relación
        public Rol? Rol { get; set; }
    }
}
