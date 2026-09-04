using Molinos.DataAgro.Entities.Dto.Distribucion;
using System;
using System.IO;

namespace Molinos.DataAgro.Interfaces
{
    public interface ISapArchivoParserManager
    {
        ImportacionSapResultadoDto ParsearArchivo(Stream stream, string extension, DateTime fecha);
    }
}
