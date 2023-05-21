using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcSendEmailAzure.Models
{
    [Table("CITAS")]
    public class Cita
    {
        [Key]
        [Column("IDCITA")]
        public int IdCita { get; set; }

        [Column("IDUSUARIO")]
        public int IdUsuario { get; set; }

        [Column("IDMASCOTA")]
        public int IdMascota { get; set; }

        [Column("TIPO_CITA")]
        public string TipoCita { get; set; }

        [Column("DIA_CITA")]
        public DateTime DiaCita { get; set; }

    }
}
