using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class DatosIniCubProveedores
    {
        public List<ProveedorCombo> Proveedor { get; set; }
        public List<Provincia> Provincia { get; set; }
        public List<LocalidadCombo> Localidad { get; set; }
        public List<Estado> Estado { get; set; }
        public List<Segmentacion> Segmentacion { get; set; }
        public List<MaterialCombo> Material { get; set; }
        public List<ComercialCombo> Comercial { get; set; }
    }


    public class ParamCubProveedores
    {
        public int? proveedorId { get; set; }
        public int? provinciaId { get; set; }
        public int? LocalidadId { get; set; }
        public int? EstadoId { get; set; }
        public int? SegmentacionId { get; set; }
        public int? material { get; set; }
        public int? ComercialId { get; set; }

        //--------------------------------------------------------------------------------
        //  Constructor
        //--------------------------------------------------------------------------------

        public ParamCubProveedores()
        {
            this.proveedorId = null;
            this.provinciaId = null;
            this.LocalidadId = null;
            this.EstadoId = null;
            this.SegmentacionId = null;
            this.material = null;
            this.ComercialId = null;
        }
    }


    public class ResultCubProveedores : Resultado
    {
        public List<ProveedoresCub> Proveedores { get; set; }
    }
    

    public class ProveedoresCub
    {
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public string Sergmentacion { get; set; }
        public string Estado { get; set; }
        public string ContactoApellido { get; set; }
        public string AreaDeInfluencia { get; set; }
        public string MaterialCampaña { get; set; }
        public string Campaña { get; set; }
        public double? ToneladasCompradas { get; set; }
        public double? ToneladasObjetivo { get; set; }
        public string CampañaCampo { get; set; }
        public string MaterialCampo { get; set; }
        public double? HectCampo { get; set; }
        public double? TonCampo { get; set; }
        public string LocalidadCampo { get; set; }
        public string ProvinciaCampo { get; set; }
        public string CampañaAcopio { get; set; }
        public double? PorcentajeAcopio { get; set; }
        public double? TonAcopio { get; set; }
        public string LocalidadAcopio { get; set; }
        public string ProvinciaAcopio { get; set; }
    }




}


