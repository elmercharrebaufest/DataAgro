using Mastersoft.Framework.Standard;
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


    public class ParamCubProveedores : IEntityValid
    {
        public Nullable<int> proveedorId { get; set; }
        public Nullable<int> provinciaId { get; set; }
        public Nullable<int> LocalidadId { get; set; }
        public Nullable<int> EstadoId { get; set; }
        public Nullable<int> SegmentacionId { get; set; }
        public Nullable<int> material { get; set; }
        public Nullable<int> ComercialId { get; set; }

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

        //--------------------------------------------------------------------------------
        //   Implementacion de IEntityValid
        //--------------------------------------------------------------------------------

        public bool Validate(List<ErrorMessage> oErrorMessages)
        {
            return oErrorMessages.Count == 0;
        }

    }


    public class ResultCubProveedores : EntityErrors
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
        public Nullable<double> ToneladasCompradas { get; set; }
        public Nullable<double> ToneladasObjetivo { get; set; }
        public string CampañaCampo { get; set; }
        public string MaterialCampo { get; set; }
        public Nullable<double> HectCampo { get; set; }
        public Nullable<double> TonCampo { get; set; }
        public string LocalidadCampo { get; set; }
        public string ProvinciaCampo { get; set; }
        public string CampañaAcopio { get; set; }
        public Nullable<double> PorcentajeAcopio { get; set; }
        public Nullable<double> TonAcopio { get; set; }
        public string LocalidadAcopio { get; set; }
        public string ProvinciaAcopio { get; set; }
    }




}


