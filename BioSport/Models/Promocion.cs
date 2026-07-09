
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioSport.Models
{
    [Table("Promociones")]
    public class Promocion
    {
        [Key]
        [Column("id_promocion")]
        public int IdPromocion { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("descuento_porcentaje")]
        public decimal? DescuentoPorcentaje { get; set; }

        [Column("fecha_inicio")]
        public DateTime FechaInicio { get; set; }

        [Column("fecha_fin")]
        public DateTime FechaFin { get; set; }

        [Column("estado")]
        public string? Estado { get; set; }
    }
}
