using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    [Table("ContactoComercialAuditoria")]
    public class ContactoComercialAuditoria
    {
        [Key]
        public int ContactoComercialAuditoriaId { get; set; }
        
        public int ContactoComercialId { get; set; }
        
        public int? ProveedorId { get; set; }
        
        public string Apellido { get; set; }
        
        public string Nombres { get; set; }
        
        public string CuitApoderado { get; set; }
        
        public DateTime? FechaDesde { get; set; }
        
        public DateTime? FechaHasta { get; set; }
        
        public bool? EsApoderado { get; set; }
        
        public int? PuestoApoderadoId { get; set; }
        
        public string Puesto { get; set; }
        
        public string Telefono1 { get; set; }
        
        public int? TipoTelefono1Id { get; set; }
        
        public string Telefono2 { get; set; }
        
        public int? TipoTelefono2Id { get; set; }
        
        public string Telefono3 { get; set; }
        
        public int? TipoTelefono3Id { get; set; }
        
        public string Email1 { get; set; }
        
        public string Email2 { get; set; }
        
        public string Email3 { get; set; }
        
        public DateTime? FechaNacimiento { get; set; }
        
        public string OtrosIntereses { get; set; }
        
        public bool? EsPrincipal { get; set; }
        
        public string Cargo { get; set; }
        
        public bool? CompraNet { get; set; }
        
        public bool? Cupo { get; set; }
        
        public bool? Boleto { get; set; }
        
        /// <summary>
        /// Tipo de operación: 'INSERT', 'UPDATE', 'DELETE'
        /// </summary>
        public string TipoOperacion { get; set; }
        
        /// <summary>
        /// Usuario del sistema que realizó la operación
        /// </summary>
        public string UsuarioModificacion { get; set; }
        
        /// <summary>
        /// Fecha y hora (UTC) de la modificación
        /// </summary>
        public DateTime FechaModificacion { get; set; }
        
        /// <summary>
        /// JSON con los valores anteriores (en UPDATE y DELETE)
        /// </summary>
        public string ValoresAnteriores { get; set; }
        
        /// <summary>
        /// JSON con los valores nuevos (en INSERT y UPDATE)
        /// </summary>
        public string ValoresNuevos { get; set; }
    }
}
