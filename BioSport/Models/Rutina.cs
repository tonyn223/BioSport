
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioSport.Models
{
    [Table("Rutinas")]
    public class Rutina
    {
        [Key]
        [Column("id_rutina")]
        public int IdRutina { get; set; }

        [Column("id_entrenador")]
        public int IdEntrenador { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("fecha_creacion")]
        public DateTime? FechaCreacion { get; set; }

        public Usuario? Entrenador { get; set; }
    }
}
