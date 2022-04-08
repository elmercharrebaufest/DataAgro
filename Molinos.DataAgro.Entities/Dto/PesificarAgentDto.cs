using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Molinos.DataAgro.Entities.Dto
{
    public class PesificarAgentDto : IEquatable<PesificarAgentDto>
    {
        public string Material { get; set; }
        public string Contrato { get; set; }
        public int? CantidadPendiente { get; set; }
        public string Comercial { get; set; }
        public string Fijacion { get; set; }
        public DateTime? FechaFijacion { get; set; }
        public DateTime? FechaHastaDolarizado { get; set; }
        public DateTime? FechaUltimaAplicacion { get; set; }
        public bool DolarizadoNoProductor { get; set; }
        public string CuitCorredor { get; set; }
        public string CuitVendedor { get; set; }
        public bool DolarizadoExpress { get; set; }
        public string Moneda { get; set; }
        public decimal KgNoPesificable { get; set; }
        public decimal KgVencimientoPesificable { get; set; }
        public decimal Precio { get; set; }
        public string NombreCorredor { get; set; }
        public string NombreVendedor { get; set; }
        public string Unidad { get; set; }
        public bool Dolarizado { get; set; }
        public string Clasificacion { get; set; }
        public string Anticipo { get; set; }
        public int USDPesificable { get; set; }
        public int USDNoPesificable { get; set; }
        public decimal KgTotales { get; set; }
        public bool Pase { get; set; }
        public decimal? Plus { get; set; }
        public string Posicion { get; set; }
        public double? KgTotalesPase { get; set; }
        public string Status { get; set; }
        public bool Cesion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CantidadLiquidada { get; set; }
        public decimal CantidadRecibida { get; set; }
        public string ConPrecio { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as PesificarAgentDto);
        }

        public bool Equals(PesificarAgentDto other)
        {
            if ((other == null) || !this.GetType().Equals(other.GetType()))
            {
                return false;
            }
            else
            {
                var oType = other.GetType();

                foreach (var oProperty in oType.GetProperties())
                {

                    var oOldValue = oProperty.GetValue(other, null);
                    var oNewValue = oProperty.GetValue(this, null);

                    if (Equals(oOldValue, oNewValue)) { continue; }
                    else
                    {
                        return false;
                    };

                }

                return true;
            }
        }

        public override int GetHashCode()
        {
            int hashCode = this.GetHashCodeOnProperties();
            return hashCode;            
        }        
    }

}


