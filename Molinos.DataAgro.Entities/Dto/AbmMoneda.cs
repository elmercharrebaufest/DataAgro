using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ParamAbmMoneda
    {
        public string MonedaId { get; set; }
        public string Descripcion { get; set; }
    }


    public class ResultIniMoneda
    {
        public List<MonedaIni> Moneda { get; set; }
    }


    public class MonedaIni
    {
        public string MonedaId { get; set; }

        public string Descripcion { get; set; }

    }
}
