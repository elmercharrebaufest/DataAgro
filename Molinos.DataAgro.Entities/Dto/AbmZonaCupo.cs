using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class DatosIniAbmZonaCupo
    {
        public List<ZonaCupoCombo> ZonaCupo { get; set; }
    }

    public class DataAbmZonaCupo : Resultado
    {
        public ZonaCupoDto ZonaCupo { get; set; }

        public DataAbmZonaCupo()
        {
            ZonaCupo = new ZonaCupoDto();
        }
    }

    public class ResultIniZonaCupo
    {
        public List<ZonaCupoIni> ZonaCupo { get; set; }
    }

    public class ZonaCupoIni
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string CodigoSap { get; set; }
    }
}
