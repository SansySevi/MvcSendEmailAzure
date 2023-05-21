using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcSendEmailAzure.Models
{
    [Table("USUARIOS")]
    public class Usuario
    {
        [Key]
        [Column("IDUSUARIO")]
        public int IdUsuario { get; set; }
        [Column("APODO")]
        public string Apodo { get; set; }
        [Column("NOMBRE")]
        public string Nombre { get; set; }
        [Column("EMAIL")]
        public string Email { get; set; }
        [Column("TELEFONO")]
		public string Telefono { get; set; }
		[Column("PASS")]
		public string Pass { get; set; }
        [Column("SALT")]
        public string Salt { get; set; }
        [Column("IMAGEN")]
        public string Imagen { get; set; }

        //LOS TIPOS VARBINARY, CLOB, BLOB
        //SON CONVERTIDOS AUTOMATICAMENTE A BYTE[]
        //POR EF
        [Column("PASS_CIFRADA")]
        public byte[] Password { get; set; }
        
    }
}
