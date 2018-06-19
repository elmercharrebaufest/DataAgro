using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities
{
    public class DatosIniProveedor
    {
        public List<SegmentacionQry> segm { get; set; }
        public List<TipoTelefonoQry> tiptel { get; set; }
        public List<ProvinciaQry> prov { get; set; }
        public List<LocalidadQry> loc { get; set; }
        public List<CanalOperacionQry> cope { get; set; }
        public List<MaterialQry> gran { get; set; }
        public List<DestinatarioQry> dest { get; set; }
        public List<CondicionQry> cond { get; set; }
        public List<InteresQry> inte { get; set; }
        public List<TipoActividadQry> tipoact { get; set; }
        public List<ContactoComercialQry> concom { get; set; }

        public DatosIniProveedor()
        {
            segm = new List<SegmentacionQry>();
            tiptel = new List<TipoTelefonoQry>();
            prov = new List<ProvinciaQry>();
            loc = new List<LocalidadQry>();
            cope = new List<CanalOperacionQry>();
            gran = new List<MaterialQry>();
            dest = new List<DestinatarioQry>();
            cond = new List<CondicionQry>();
            inte = new List<InteresQry>();
            tipoact = new List<TipoActividadQry>();
            concom = new List<ContactoComercialQry>();
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
