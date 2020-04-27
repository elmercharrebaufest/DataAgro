using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{

    public class DatosFiltro
    {
        public int? ProveedorId { get; set; }
        public int ContratoSap { get; set; }
        public int ComercialId { get; set; }
        public int MaterialId { get; set; }
        public decimal Precio { get; set; }
        public int CampaniaId { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public int EstadoId { get; set; }
        public string Usuario { get; set; }

        public DatosFiltro()
        {
            ProveedorId = 0;
            ContratoSap = 0;
            ComercialId = 0;
            MaterialId = 0;
            Precio = 0;
            CampaniaId = 0;
            FechaDesde = "";
            FechaHasta = "";
            EstadoId = 0;
            Usuario = "";
        }
    }

    public class DatosIniCompraNet
    {
        public List<ProveedorCombo> Proveedor { get; set; }
        public List<ComercialCombo> Comercial { get; set; }
        public List<MaterialCombo> Material { get; set; }
        //public List<MonedaCombo> Moneda { get; set; }
        public List<Campaña> Campaña { get; set; }
        public List<Provincia> Provincia { get; set; }
        public List<LocalidadCombo> Localidad { get; set; }
        //public List<MonedaCombo> MonedaSustentable { get; set; }
    }

    public class ContratoExcel
    {
        public DateTime? Fecha { get; set; }
        public string Comercial { get; set; }
        public string Grano { get; set; }
        public string Tipo { get; set; }
        public string Contrato { get; set; }
        public string Destino { get; set; }
        public string Zona { get; set; }
        public string Nombre { get; set; }
        public string CUIT { get; set; }
        public string NombreCorredor { get; set; }
        public string CUITCorredor { get; set; }
        public string Figura { get; set; }
        public double Cantidad { get; set; }
        public decimal Precio { get; set; }
        public string Moneda { get; set; }
        public int? Camiones { get; set; }
        public decimal? Comision { get; set; }
        public string PorcentajeBonificacion { get; set; }
        public string ImporteBonificacion { get; set; }
        public string MonedaBonificacion { get; set; }
        public string MesPosicion { get; set; }
        public string Procedencia { get; set; }        
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public string Cosecha { get; set; }
        public decimal? FleteProcedencia { get; set; }
        public string Observaciones { get; set; }
        
    }
}

