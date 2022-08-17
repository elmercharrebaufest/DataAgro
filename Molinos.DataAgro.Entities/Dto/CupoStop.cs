using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class CupoStop
    {
        public int? nroPlantaRuca { get; set; }

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

        // V2

        public string cuitOrigen { get; set; }
        public string cuitIntermediario { get; set; }
        public string CuitRemComercialProductor { get; set; }
        public string CuitCorredorVentaSecundaria { get; set; }
        public string CuitCorredorVentaPrimaria { get; set; }
        //public int? nroPlantaRucaOrigen { get; set; }

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
        //public string cuitOrigen { get; set; }
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

    public class RespuestaCupoNoPropioStop : RespuestaCupoStop
    {
        public string cuitDestinatarioAfip { get; set; }
        public string cuitDestinoAfip { get; set; }
        public string cuitIntermediarioAfip { get; set; }
        public string cuitRemComercialAfip { get; set; }
        public string cuitRepresentanteEntregadorAfip { get; set; }
        public string esAnulado { get; set; }
        public string esRechazado { get; set; }
        public long? idCuitOrigen { get; set; }
        public long? idCuitIntermediario { get; set; }
        public long? idCuitMercadoATermino { get; set; }
        public long? idCuitRemComercial { get; set; }
        public long? idCuitCorredorV { get; set; }
        public long? idCuitCorredorC { get; set; }
        public long? idCuitRepresentanteEntregador { get; set; }
        public long? idCuitDestino { get; set; }
        public long? idCuitDestinatario { get; set; }
        public long? idCuitIntermediarioFlete { get; set; }
        public long? idCuitTransportista { get; set; }
        public long? idCuitChofer { get; set; }
        public string fechaCP_Carga { get; set; }//fecha
        public string fechaCP_Vto { get; set; }//fecha
        public long? idTurnoDetalle { get; set; }
        public string renspa { get; set; }
        public decimal? pesoOriginal { get; set; }
        public string validaKM { get; set; }
        public int? cantHorasSalidaCamion { get; set; }
        public string dominio { get; set; }
        public string dominio_1 { get; set; }
        public string dominio_2 { get; set; }
        public string nroContrato { get; set; }
        public string nroPlantaRuca { get; set; }
        public int? idEstadoEnPlanta { get; set; }
        public  string modificado { get; set; } // fecha
        public int? creadoPor { get; set; }
        public int? modificadoPor { get; set; }
        public string consultadoXAFIP { get; set; }
        public string fechaActivado { get; set; } // fecha
        public string fechaArribado { get; set; } // fecha
        public string fechaRechazado { get; set; } // fecha
        public string fechaDesviadoD { get; set; } // fecha
        public string fechaRegresado { get; set; } // fecha
        public string fechaDesviadoO { get; set; } // fecha
        public string fechaAnulado { get; set; } // fecha 
        public string fechaConfirmado { get; set; } // fecha
        public string fechaDescargado { get; set; } // fecha
        public string fechaReActivado { get; set; } // fecha
        public string fechaTomado { get; set; } // fecha
        public string ultima_latitud { get; set; }
        public string ultima_longitud { get; set; }

    }

    public partial class ConsultaCuposStop
    {
        public List<RespuestaCupoStop> results { get; set; }
    }

    public partial class ConsultaTurnosActivosStop 
    { 
        public int count { get; set; }
        public List<RespuestaCupoNoPropioStop> data { get; set; }
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



