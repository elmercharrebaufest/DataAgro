using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class DatosIniAbmOperadorModel : Resultado
    {
        public DatosIniAbmOperador Datos { get; set; }

        public DatosIniAbmOperadorModel()
        {
            Datos = new DatosIniAbmOperador();
        }
    }

    public class ResultIniOperadorModel : Resultado
    {
        public List<OperadorIni> Datos { get; set; }

        public ResultIniOperadorModel()
        {
            Datos = new List<OperadorIni>();
        }
    }

    public class AbmOperadorParam
    {
        public int Id { get; set; }
    }

    public class AbmOperadorResult : Resultado
    {
        public OperadorDto Operador { get; set; }

        public AbmOperadorResult()
        {
            Operador = new OperadorDto();
        }
    }

    public class ResultIniPostOperadorModel : Resultado
    {
        public string Texto { get; set; }
    }
}