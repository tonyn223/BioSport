
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioSport.Models
{
    [Table("Asistencia")]
    public class Asistencia
    {
        [Key]
        [Column("id_asistencia")]
        public int IdAsistencia { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("fecha_entrada")]
        public DateTime FechaEntrada { get; set; }

        [Column("hora_entrada")]
        public TimeSpan HoraEntrada { get; set; }

        public Usuario? Usuario { get; set; }
    }
}
