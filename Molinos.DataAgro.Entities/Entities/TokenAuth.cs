using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class TokenAuth
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Cuit { get; set; }
        public string Token { get; set; }
        public DateTime Vencimiento { get; set; }
        public string NombreUsuario { get; set; }
    }


}
   


