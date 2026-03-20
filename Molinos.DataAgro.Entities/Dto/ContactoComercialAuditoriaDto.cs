using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ContactoComercialAuditoriaDto
    {
        public int ContactoComercialAuditoriaId { get; set; }
        
        public int ContactoComercialId { get; set; }
        
        public int? ProveedorId { get; set; }
        
        public string ProveedorNombre { get; set; }
        
        public string TipoOperacion { get; set; }
        
        public string UsuarioModificacion { get; set; }
        
        public DateTime FechaModificacion { get; set; }
        
        public string CamposModificados { get; set; }
        
        public string ValoresAnteriores { get; set; }
        
        public string ValoresNuevos { get; set; }
        
        /// <summary>
        /// Resumen legible del cambio realizado
        /// </summary>
        public string Resumen { get; set; }
    }
}
