using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcSendEmailAzure.Models
{

    [Table("MASCOTAS")]
    public class Mascota
    {
        [Key]
        [Column("IDMASCOTA")]
        public int IdMascota { get; set; }

        [Column("NMASCOTA")]
        public string Nombre { get; set; }

        [Column("FECHA_NACIMIENTO")]
        public DateTime Fecha_Nacimiento { get; set; }

        [Column("PESO")]
        public int Peso { get; set; }

        [Column("RAZA")]
        public string Raza { get; set; }

        [Column("TIPO")]
        public string Tipo { get; set; }

        [Column("IMAGEN")]
        public string Imagen { get; set; }

        [Column("IDUSUARIO")]
        public int IdUsuario { get; set; }
    }
}
