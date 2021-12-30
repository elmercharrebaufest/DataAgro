using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class DatosIniAbmCentro
    {
        public List<CentroCombo> Centro { get; set; }
    }

    public class DataAbmCentro : Resultado
    {
        public CentroDto Centro { get; set; }

        public DataAbmCentro()
        {
            Centro = new CentroDto();
        }
    }
    
    public class ResultIniCentro
    {
        public List<CentroIni> Centro { get; set; }
    }
    
    public class CentroIni
    {
        public int Id { get; set; }                  
        public string Descripcion { get; set; }                  
        public string CodigoSap { get; set; }
        public bool ValidaRedespacho { get; set; }
        public int? LocalidadId { get; set; }
        public string Localidad { get; set; }
        public bool Acopio { get; set; }
        public string CodigoPostal { get; set; }
        public string Direccion { get; set; }
        public bool Comision { get; set; }
        public bool CargaNegocios { get; set; }
        public bool CargaCupos { get; set; }
    }
}


