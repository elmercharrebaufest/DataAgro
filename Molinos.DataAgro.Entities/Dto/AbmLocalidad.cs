using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class DatosIniAbmLocalidad
    {
        public List<Provincia> Provincia { get; set; }
    }
    
    public class ParamAbmLocalidad
    {
        public string Nombre { get; set; }
        public int? ProvinciaId { get; set; }
        public int? PartidoId { get; set; }
    }
    
    public class ResultIniLocalidad
    {
        public List<LocalidadIni> Localidad { get; set; }
    }
    
    public class LocalidadIni
    {
        public int? LocalidadId { get; set; }
        public string CodLocalidad { get; set; }
        public string Nombre { get; set; }
        public string ProNombre { get; set; }
        public string PartidoNombre { get; set; }
    }

    public class ResultIniPartido
    {
        public List<Partido> Partidos { get; set; }
    }
}