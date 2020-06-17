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
        public string cuitCorredorC { get; set; }
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
        public string estadoEnPlanta { get; set; }
        public string cartaPorte { get; set; }
        public string ctg { get; set; }
        public string fechaCTG_Desde { get; set; }
        public string fechaCTG_Hasta { get; set; }
        public string cuitCorredorVAfip { get; set; }
        public string cuitCorredorCAfip { get; set; }
        public string cuitOrigen { get; set; }
        public string cuitRemComercial { get; set; }
        public string cuitMercadoATerminoAfip { get; set; }
        public string cosecha { get; set; }
        public string pesoNetoEstimado  { get; set; }
        public string kmRecorrer { get; set; }
        public string cuitIntermediarioFleteAfip { get; set; }
        public string cuitTransportistaAfip { get; set; }
        public string cuitChoferAfip { get; set; }
        public string cuitOrigenAfip { get; set; }
        public string codLocalidadOrigen { get; set; }
        public string nroEstablecimientoOrigen { get; set; }
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



