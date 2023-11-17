using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ResearchDto
    {
        public string Title { get; set; }
        //public FieldUrlValue PathDocumentos { get; set; }
        //public string Usuario { get; set; }
        //public string Zona { get; set; }
        public int? Id { get; set; }
        //public bool? Sincronizado { get; set; }
        public DateTime? FechaAlta { get; set; }
        //public List<AdjuntoResearch> Adjuntos { get; set; }

        //public DateTime? Created { get; set; }
        public string tipoCarga { get; set; }
        public string Cultivo { get; set; }
        public string Antecesor { get; set; }
        public string Campana { get; set; }
        public string EstadioFenologico { get; set; }
        public string CondicionCultivo { get; set; }
        public string HumedadSuelo { get; set; }
        public string Comentarios { get; set; }
        public string Partido { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public int? Latitud { get; set; }
        public int? Longitud { get; set; }
        public double? rendimiento { get; set; }
        public string MuestraUno { get; set; }
        public string MedidasUno { get; set; }
        public int? PromedioMuestraUno { get; set; }
        public string MuestraDos { get; set; }
        public string MedidasDos { get; set; }
        public int? PromedioMuestraDos { get; set; }
        public string MuestraTres { get; set; }
        public string MedidasTres { get; set; }
        public int? PromedioMuestraTres { get; set; }
        public double? DistanciaHileras { get; set; }
        public double? Coeficiente { get; set; }
        public int? CapitulosGirasol { get; set; }
        public string estadoConectividad { get; set; }
        public bool? Attachments { get; set; }
        public string Author { get; set; }
        public string Editor { get; set; }
    }

    public class AdjuntoResearch
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Nombre { get; set; }
        public string Extension { get { return System.IO.Path.GetExtension(Nombre); } }
        public byte[] Data { get; set; }
    }
}
