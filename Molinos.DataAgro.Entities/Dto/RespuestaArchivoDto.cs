using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class RespuestaArchivoDto
    {
        public bool EsExitoso { get; set; }
        public string Mensaje { get; set; }
        public List<string> Errores { get; set; }
        public string NombreArchivo { get; set; }
        public byte[] Contenido { get; set; }
    }
}