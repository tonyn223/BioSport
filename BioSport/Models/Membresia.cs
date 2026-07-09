using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioSport.Models
{
    [Table("Membresias")]
    public class Membresia
    {
        [Key]
        [Column("id_membresia")]
        public int IdMembresia { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("id_plan")]
        public int IdPlan { get; set; }

        [Column("fecha_inicio")]
        public DateTime FechaInicio { get; set; }

        [Column("fecha_vencimiento")]
        public DateTime FechaVencimiento { get; set; }

        [Column("estado")]
        public string? Estado { get; set; }

        public Usuario? Usuario { get; set; }
        public Plan? Plan { get; set; }
    }
}

