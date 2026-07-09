
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioSport.Models
{
    [Table("Progreso")]
    public class Progreso
    {
        [Key]
        [Column("id_progreso")]
        public int IdProgreso { get; set; }

        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Column("id_entrenador")]
        public int IdEntrenador { get; set; }

        [Column("observaciones")]
        public string? Observaciones { get; set; }

        [Column("peso")]
        public decimal? Peso { get; set; }

        [Column("fecha_registro")]
        public DateTime? FechaRegistro { get; set; }

        public Usuario? Cliente { get; set; }
        public Usuario? Entrenador { get; set; }
    }
}
