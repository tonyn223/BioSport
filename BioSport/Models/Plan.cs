 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioSport.Models
{
    [Table("Planes")]
    public class Plan
    {
        [Key]
        [Column("id_plan")]
        public int IdPlan { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("precio")]
        public decimal Precio { get; set; }

        [Column("duracion_dias")]
        public int DuracionDias { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }
    }
}
