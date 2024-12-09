using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class DatosIniProveedor
    {
        public List<SegmentacionQry> Segmentacion { get; set; }
        public List<TipoTelefonoQry> TipoTelefono { get; set; }
        public List<ProvinciaQry> prov { get; set; }
        public List<LocalidadQry> Localidad { get; set; }
        public List<CanalOperacionQry> CanalOperacion { get; set; }
        public List<MaterialQry> Material { get; set; }
        public List<DestinatarioQry> dest { get; set; }
        public List<CondicionQry> cond { get; set; }
        public List<InteresQry> inte { get; set; }
        public List<TipoActividadQry> TipoActividad { get; set; }
        public List<ContactoComercialQry> concom { get; set; }
        public List<ClasificacionCompraNetQry> ClasComNet { get; set; }
        public List<BoletoCompraNetQry> BoleComNet { get; set; }
        public List<BolsaCompraNetQry> BolsComNet { get; set; }
        public List<ComercialDto> comercial { get; set; }
        public List<PuestoApoderadoDto> TipoApoderado { get; set; }
        public DatosIniProveedor()
        {
            Segmentacion = new List<SegmentacionQry>();
            TipoTelefono = new List<TipoTelefonoQry>();
            prov = new List<ProvinciaQry>();
            Localidad = new List<LocalidadQry>();
            CanalOperacion = new List<CanalOperacionQry>();
            Material = new List<MaterialQry>();
            dest = new List<DestinatarioQry>();
            cond = new List<CondicionQry>();
            inte = new List<InteresQry>();
            TipoActividad = new List<TipoActividadQry>();
            concom = new List<ContactoComercialQry>();
            ClasComNet = new List<ClasificacionCompraNetQry>();
            BoleComNet = new List<BoletoCompraNetQry>();
            BolsComNet = new List<BolsaCompraNetQry>();
            comercial = new List<ComercialDto>();
            TipoApoderado = new List<PuestoApoderadoDto>();
        }
    }

    public class Datos
    {
        public string CUIT { get; set; }
        public string UsuarioDirectory { get; set; }
    }

    public class ComprasIniciales
    {
        public List<string> CUIT { get; set; }
        public string UsuarioDirectory { get; set; }

        public ComprasIniciales()
        {
            CUIT = new List<string>();
            UsuarioDirectory = String.Empty;
        }

    }
}
