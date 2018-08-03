using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class Resultado
    {

        public List<ErrorMessage> Errores { get; set; } = new List<ErrorMessage>();

        public bool HayErrores
        {
            get { return Errores.Count != 0; }
        }

        public void Error(string clave, string descripcion)
        {
            Errores.Add(new ErrorMessage(descripcion, clave));
        }
    }
}
