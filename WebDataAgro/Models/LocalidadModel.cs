using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class DatosIniAbmLocalidadModel : Resultado
    {
        public DatosIniAbmLocalidad Datos { get; set; }
        public Localidad Localidad { get; set; }

        public DatosIniAbmLocalidadModel()
        {
            this.Datos = new DatosIniAbmLocalidad();
        }
    }


    public class ResultIniLocalidadModel : Resultado
    {
        public List<LocalidadIni> Datos { get; set; }

        public ResultIniLocalidadModel()
        {
            this.Datos = new List<LocalidadIni>();
        }
    }


    public class AbmLocalidadParam
    {
        public int LocalidadId { get; set; }

    }


    public class AbmLocalidadResult : Resultado
    {
        public Localidad Localidad { get; set; }

        public AbmLocalidadResult()
        {
            this.Localidad = new Localidad();
        }
    }

    public class AbmLocalidadCrearResult : Resultado
    {
        public LocalidadDto Localidad { get; set; }

        public AbmLocalidadCrearResult()
        {
            this.Localidad = new LocalidadDto();
        }
    }

    public class ResultPartidosModel : Resultado
    {
        public ResultIniPartido Datos { get; set; }

        public ResultPartidosModel()
        {
            Datos = new ResultIniPartido();
        }
    }
}