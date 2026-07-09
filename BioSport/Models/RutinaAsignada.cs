
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioSport.Models
{
    [Table("RutinasAsignadas")]
    public class RutinaAsignada
    {
        [Key]
        [Column("id_asignacion")]
        public int IdAsignacion { get; set; }

        [Column("id_rutina")]
        public int IdRutina { get; set; }

        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Column("fecha_asignacion")]
        public DateTime? FechaAsignacion { get; set; }

        public Rutina? Rutina { get; set; }
        public Usuario? Cliente { get; set; }
    }
}
