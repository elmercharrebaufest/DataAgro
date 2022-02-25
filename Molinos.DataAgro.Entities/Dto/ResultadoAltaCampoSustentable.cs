using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ResultadoAltaCampoSustentable
    {
        public ResultadoAltaCampoSustentable()
        {
            Errores = new List<ErrorMessage>();
        }
        public void Error(string clave, string descripcion)
        {
            Errores.Add(new ErrorMessage(descripcion, clave));
        }

        public List<ErrorMessage> Errores { get; set; }

        public bool HayError { get; set; }

    }
}
