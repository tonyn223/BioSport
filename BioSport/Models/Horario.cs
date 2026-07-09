
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioSport.Models
{
    [Table("Horarios")]
    public class Horario
    {
        [Key]
        [Column("id_horario")]
        public int IdHorario { get; set; }

        [Column("dia_semana")]
        public string DiaSemana { get; set; } = string.Empty;

        [Column("hora_apertura")]
        public TimeSpan HoraApertura { get; set; }

        [Column("hora_cierre")]
        public TimeSpan HoraCierre { get; set; }
    }
}
