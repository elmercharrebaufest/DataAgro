using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class CupoStop
    {
        public string token { get; set; }
        public string cuitDestino { get; set; }
        public string cuitDestinatario { get; set; }
        public string idCupoTerminal { get; set; }
        public int idTerminal { get; set; }
        public string fecha { get; set; }
        public int codLocalidadDestino { get; set; }
        public string desvio { get; set; }
        public int codGrano { get; set; }

    }
    public partial class ModificarCupo :CupoStop
    {

        public int idCupo { get; set; }
        public int idCupoEstado { get; set; }
        public string estado { get; set; }
    }
    public partial class RespuestaCupoStop : ModificarCupo
    {     
        public string creado { get; set; }
    }
    public partial class ConsultaCuposStop
    {
        public List<RespuestaCupoStop> results { get; set; }
    }
    public partial class ResultadoStop
    {
        public bool isError { get; set; }
        //public int ticks { get; set; }
    }
    public partial class ErrorStop
    {
        public string status { get; set; }
        public string developerMessage { get; set; }
        public string userMessage { get; set; }
        public string errorCode { get; set; }
        public string moreInfo { get; set; }
    }
}



