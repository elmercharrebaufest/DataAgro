using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class DatosIniAbmOperador
    {
        public List<OperadorCombo> Operador { get; set; }
    }

    public class DataAbmOperador : Resultado
    {
        public OperadorDto Operador { get; set; }

        public DataAbmOperador()
        {
            Operador = new OperadorDto();
        }
    }
    
    public class ResultIniOperador
    {
        public List<OperadorIni> Operador { get; set; }
    }
    
    public class OperadorIni
    {
        public int Id { get; set; }                  
        public string Descripcion { get; set; }
    }
}


