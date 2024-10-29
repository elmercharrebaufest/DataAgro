using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public class ConfirmaAltaEstadoLote
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int CodigoConfirmaAltaEstadoLote { get; set; }
    }
}