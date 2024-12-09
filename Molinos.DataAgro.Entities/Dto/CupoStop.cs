using System.Collections.Generic;

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
        public string CuitIntermediario { get; set; }
        public string CuitRemComercialProductor { get; set; }
        public string CuitCorredorVentaSecundaria { get; set; }
        public string CuitCorredorVentaPrimaria { get; set; }
        //public int? nroPlantaRucaOrigen { get; set; }

    }
    public partial class ModificarCupo : CupoStop
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
        public string pesoNetoEstimado { get; set; }
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
        public string CuitDestinatarioAfip { get; set; }
        public string CuitDestinoAfip { get; set; }
        public string CuitIntermediarioAfip { get; set; }
        public string CuitRemComercialAfip { get; set; }
        public string CuitRepresentanteEntregadorAfip { get; set; }
        public string EsAnulado { get; set; }
        public string EsRechazado { get; set; }
        public long? IdCuitOrigen { get; set; }
        public long? IdCuitIntermediario { get; set; }
        public long? IdCuitMercadoATermino { get; set; }
        public long? IdCuitRemComercial { get; set; }
        public long? IdCuitCorredorV { get; set; }
        public long? IdCuitCorredorC { get; set; }
        public long? IdCuitRepresentanteEntregador { get; set; }
        public long? IdCuitDestino { get; set; }
        public long? IdCuitDestinatario { get; set; }
        public long? IdCuitIntermediarioFlete { get; set; }
        public long? IdCuitTransportista { get; set; }
        public long? IdCuitChofer { get; set; }
        public string FechaCP_Carga { get; set; }//fecha
        public string FechaCP_Vto { get; set; }//fecha
        public long? IdTurnoDetalle { get; set; }
        public string Renspa { get; set; }
        public decimal? PesoOriginal { get; set; }
        public string ValidaKM { get; set; }
        public int? CantHorasSalidaCamion { get; set; }
        public string Dominio { get; set; }
        public string Dominio_1 { get; set; }
        public string Dominio_2 { get; set; }
        public string NroContrato { get; set; }
        public string NroPlantaRuca { get; set; }
        public int? IdEstadoEnPlanta { get; set; }
        public string Modificado { get; set; } // fecha
        public int? CreadoPor { get; set; }
        public int? ModificadoPor { get; set; }
        public string ConsultadoXAFIP { get; set; }
        public string FechaActivado { get; set; } // fecha
        public string FechaArribado { get; set; } // fecha
        public string FechaRechazado { get; set; } // fecha
        public string FechaDesviadoD { get; set; } // fecha
        public string FechaRegresado { get; set; } // fecha
        public string FechaDesviadoO { get; set; } // fecha
        public string FechaAnulado { get; set; } // fecha 
        public string FechaConfirmado { get; set; } // fecha
        public string FechaDescargado { get; set; } // fecha
        public string FechaReActivado { get; set; } // fecha
        public string FechaTomado { get; set; } // fecha
        public string Ultima_latitud { get; set; }
        public string Ultima_longitud { get; set; }

    }

    public partial class ConsultaCuposStop
    {
        public List<RespuestaCupoStop> results { get; set; }
    }

    public partial class ConsultaTurnosActivosStop
    {
        public int Count { get; set; }
        public List<RespuestaCupoNoPropioStop> data { get; set; }
    }
    public partial class ResultadoStop
    {
        public bool isError { get; set; }
        //public int ticks { get; set; }
    }
    public partial class ErrorStop
    {
        public string Status { get; set; }
        public string DeveloperMessage { get; set; }
        public string userMessage { get; set; }
        public string errorCode { get; set; }
        public string MoreInfo { get; set; }
    }
}



