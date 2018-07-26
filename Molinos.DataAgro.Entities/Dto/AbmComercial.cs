using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class DatosIniAbmComercial
    {
        public List<ComercialCombo> Comercial { get; set; }
        public List<Perfil> Perfil { get; set; }
        public List<GrupoDeCompras> GrupoDeCompras { get; set; }
    }

    public class DataAbmComercial
    {
        public List<MSErrorMessage> Errores { get; set; }
        public List<ComercialCombo> Comercial { get; set; }

        public DataAbmComercial()
        {
            Comercial = new List<ComercialCombo>();
            Errores = new List<MSErrorMessage>();
        }

    }

    public class ResultIniComercial
    {
        public List<ComercialIni> Comercial { get; set; }
    }

    public class ComercialIni
    {
        public int ComercialId { get; set; }                  
        public string Apellido { get; set; }                  
        public string Nombres { get; set; }                  
        public string PerDescripcion { get; set; }                  
    }

}


