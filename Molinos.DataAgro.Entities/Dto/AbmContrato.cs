using Molinos.DataAgro.Entities.Entities;
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


}

