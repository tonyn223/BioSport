
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioSport.Models
{
    [Table("Pagos")]
    public class Pago
    {
        [Key]
        [Column("id_pago")]
        public int IdPago { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("id_membresia")]
        public int? IdMembresia { get; set; }

        [Column("monto")]
        public decimal Monto { get; set; }

        [Column("fecha_pago")]
        public DateTime FechaPago { get; set; }

        [Column("metodo")]
        public string? Metodo { get; set; }

        [Column("estado")]
        public string? Estado { get; set; }

        public Usuario? Usuario { get; set; }
        public Membresia? Membresia { get; set; }
    }
}
