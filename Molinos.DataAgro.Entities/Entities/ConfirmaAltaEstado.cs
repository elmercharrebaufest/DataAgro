using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public class ConfirmaAltaEstado
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int CodigoConfirmaAltaEstado { get; set; }
    }
}