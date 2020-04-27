using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class SugerenciaCupoDto : ICloneable
    {

        public int ComercialId;

        public int Id { get; set; }
        public int MaterialId { get; set; }

        public string MaterialDesc { get; set; }

        public decimal? Precio { get; set; }

        public string MonedaId { get; set; }

        //public int? AgenteCompraId { get; set; }
        //public int? FijacionDePrecioContratoId { get; set; }
        //public int? FasonId { get; set; }
        //public int? ContratoId { get; set; }

        public int? ConfiguracionEspacioDinamicoId { get; set; }
        
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public decimal PrecioPizarra { get; set; }

        public Formula formula { get; set; }

        public Dictionary<string, decimal> Puntuaciones { get; set; } = new Dictionary<string, decimal>();
        public string PuntuacionesString{ get; set; }
        public decimal PuntuacionTotal { get; set; }
        public int DestinoId { get; set; }
        public int CantidadDeCupos { get; set; }
        public decimal CantidadDeCuposMaximo { get { return this.CantidadDeCupos; } }
        public int? CantidadFleteProcedencia { get; set; }
        public string ZonaDescrip { get; set; }
        public bool Priorizado { get; set; } 
        public DateTime FechaSugerida { get; set; }
        public int? ProveedorId { get; set; }
        public int CentroId { get; set; }
        public string MonedaDesc { get; set; }
        public string ProveedorCUIT { get; set; }
        public string ProveedorDesc { get; set; }
        public string TipoNegocioDesc { get; set; }
        public bool? Aceptado { get; set; }
        public int? ZonaCupoId { get; set; }
        public string Destinatario { get; set; }
        public string StandardDeCalidad { get; set; }
        public int TipoNegocioId { get; set; }

        public string ContratoSAP { get; set; }
        public int? NegocioId { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}

