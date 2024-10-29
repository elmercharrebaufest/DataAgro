using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public class ConfirmaAltaEstadoDocumento
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int CodigoConfirmaAltaEstadoDocumento { get; set; }
    }
}