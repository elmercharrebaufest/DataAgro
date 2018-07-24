
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class Proveedor : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProveedorId { get; set; }
        public string CUIT { get; set; }
        public int? EstadoId { get; set; }
        public string RazonSocial { get; set; }
        public int SegmentacionId { get; set; }
        public string NombreReferente { get; set; }
        public Nullable<int> Calificacion { get; set; }
        public string Intermediario { get; set; }
        public string Observaciones { get; set; }
        public string Direccion { get; set; }
        public Nullable<int> LocalidadId { get; set; }
        public Nullable<int> ProvinciaId { get; set; }
        public string CodigoPostal { get; set; }
        public Nullable<int> AreaInfluenciaId { get; set; }
        public Nullable<double> AlmacVolAnualTotal { get; set; }
        public Nullable<bool> AlmacHabilitadoSojaSust { get; set; }
        public Nullable<double> AlmacTonsMaxSojaSust { get; set; }
        public Nullable<double> AlmacHectSojaSust { get; set; }
        public Nullable<bool> ClienteMOA { get; set; }
        public string GrupoCompras { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Email3 { get; set; }
        public string Email4 { get; set; }
        public string Telefono1 { get; set; }
        public Nullable<int> TipoTelefono1Id { get; set; }
        public string Telefono2 { get; set; }
        public Nullable<int> TipoTelefono2Id { get; set; }
        public string Telefono3 { get; set; }
        public Nullable<int> TipoTelefono3Id { get; set; }
        public string Telefono4 { get; set; }
        public Nullable<int> TipoTelefono4Id { get; set; }
        public Nullable<System.DateTime> FechaUltimoContacto { get; set; }
        public string RiesgoComercialSap { get; set; }
        public Nullable<DateTime> FechaAlta { get; set; }
        public Nullable<int> LocalidadCompraNetId { get; set; }
        public Nullable<int> ProvinciaCompraNetId { get; set; }
        public Nullable<int> ClasificacionCompraNetId { get; set; }
        public Nullable<int> BoletoCompraNetId { get; set; }
        public Nullable<int> BolsaCompraNetId { get; set; }
                    
        public Proveedor()
        {
            
        }
    }


}
   


