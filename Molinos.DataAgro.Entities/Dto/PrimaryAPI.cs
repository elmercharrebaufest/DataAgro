using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class TokenPrimary
    {
        public string Status { get; set; }
        public string Code { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public string Value { get; set; }
    }

    public class TradeCaptureReportResult
    {
        public string Status { get; set; }
        public string Code { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public List<TradeCaptureReportValue> Value { get; set; } = new List<TradeCaptureReportValue>();
    }
    public class TradeCaptureReportValue
    {
        public string Currency { get; set; }//Moneda de negociación de la security
        public int? ExecID { get; set; } //Identificación de la Operación de Mercado
        public decimal? LastPx { get; set; }//Precio de la Operación
        public decimal? LastQty { get; set; }//Cantidad de Operación
        public string MarketID { get; set; }//Identificación del Mercado donde se lista la security* valores:
        //MATBA ROFEX(MarketID= ROFX)
        //ByMA(MarketID= XMEV) 
        //MATBA ROFEX OTC(MarketID= 11) 
        //a MAE(MarketID= XMAB)
        //MAV(MarketID= XROX) 
        //MATBA(MarketID= XMTB) 
        public string MarketSegmentID { get; set; }//Indica el segmento donde esta listada la security en el mercado* valoes:
        //Valores permitidos para MATBA ROFEX(MarketID= ROFX) :
        // Entrega de Mercadería
        // Fuera de Rueda
        // Rueda Electrónica
        // Rueda Piso
        //Valores permitidos para ByMA(MarketID= XMEV):
        // Fuera de Rueda
        //Valores permitidos para MATBA ROFEX OTC(MarketID= 11) :
        // Fuera de Rueda
        //Valores permitidos para MAE(MarketID= XMAB) :
        // Rueda Electrónica
        // Fuera de Rueda
        //Valores permitidos para MAV(MarketID= XROX) :
        // Rueda Electrónica
        // Fuera de Rueda
        //Valores permitidos para MATBA(MarketID= XMTB) :
        // Rueda Electrónica
        // Fuera de Rueda
        public int? OrderType { get; set; }//Para el método TradeCaptureRepor/ExecutionReport es el Tipo de
        //Operación, valores posibles:
        //LIMIT
        //MARKET
        //MARKET_TO_LIMIT
        //STOP_LIMIT
        public string SettlCurrency { get; set; }//Tipo de moneda del precio de Ajuste
        //Posibles valores:
        //Dólar Gtía.MATBA ROFEX
        //Dólar MEP
        //Dólar Cable
        //148
        //Pesos
        //Pesos BCRA
        public string SettlDate { get; set; }//Fecha de liquidación del contrato
        public string SettlType { get; set; }//Indica el plazo de liquidación de la operación, valores posibles:
        //0 = Regular
        //B = Broken date
        //1 = Cash(T+0)
        //2 = 24hs(T+1)
        //3 = 48hs(T+2)
        public string TradeDate { get; set; }//Fecha en la que se realizó la operación
        public int? TradeID { get; set; }//Identificación de Boleta de la operación
        public int? TradeNumber { get; set; }//Identificación de Boleta de la operación
        public string TransactTime { get; set; }//Hora de Transacción en la que se realizó la operación en formato Ejemplo: 2017-01-25T10:38:51 – YYYY-MM-DDTHH:MM:SS
        public string TrdRptStatus { get; set; }//Indica el estado de la operación en el momento de la consulta
        //Posibles Valores:
        //0 = Definitiva
        //3 = Anulada
        //4 = Transitoria
        public int? TrdType { get; set; }//Tipo de operación:
        //Posibles valores:
        //'0'=Interferencia de ofertas
        //'1'=Block Trade
        //'2'=Apertura entrega
        public string VenueType { get; set; }//Tipo de Mercado
        //Posibles valores:
        //C = Clearinghouse(Identifica Operaciones Anuladas o modificadas por        ACSA)
        //O = Off-market(Operaciones OTC)
        //R = Registered market(Operaciones registradas dentro del Horario de        negociación)
        public List<TradeCaptureReportInstrument> Instrument { get; set; }//Especifica el Activo involucrado en el movimiento
        public List<TradeCaptureReportRootParties> RootParties { get; set; }
        public List<TradeCaptureReportTrdCapRptSideGrp> TrdCapRptSideGrp { get; set; }
    }
    public class TradeCaptureReportInstrument
    {
        public string CFICode { get; set; }
        public string SecurityID { get; set; }
        public string SecurityIDSource { get; set; }
    }
    public class TradeCaptureReportRootParties
    {
        public string RootPartyID { get; set; }
        public string RootPartyIDSource { get; set; }
        public string RootPartyRole { get; set; }
    }
    public class TradeCaptureReportTrdCapRptSideGrp
    {
        public string Account { get; set; }
        public string Side { get; set; }
    }

    public class SecurityListResult
    {
        public string Status { get; set; }
        public string Code { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public List<Market> Value { get; set; } = new List<Market>();
    }
    public class Market
    {
        public string MarketID { get; set; }
        public string MarketSegmentID { get; set; }
        public string SecurityRequestResult { get; set; }
        public List<SecListGrp> SecListGrp { get; set; } = new List<SecListGrp>();
    }
    public class SecListGrp
    {
        public string Currency { get; set; }
        public List<InstrmtLegSecListGrp> InstrmtLegSecListGrp { get; set; } = new List<InstrmtLegSecListGrp>();
        public List<Instrument> Instrument { get; set; } = new List<Instrument>();

    }
    public class InstrmtLegSecListGrp
    {
        public string LegSecurityID { get; set; }
    }
    public class Instrument
    {
        public string Symbol { get; set; }
        public string SecurityID { get; set; }
        public string SecurityIDSource { get; set; }
        public string CFICode { get; set; }
        public string SecurityType { get; set; }
        public string MaturityMonthYear { get; set; }
        public string MaturityDate { get; set; }
        public string SecurityExchange { get; set; }
        public string SecurityStatus { get; set; }
        public int? UnitOfMeasureQty { get; set; }
        public string UnitOfMeasure { get; set; }
        public string SecurityGroup { get; set; }
        public string SettlMethod { get; set; }
        public List<MDFullGrp> MDFullGrp { get; set; } = new List<MDFullGrp>();        
    }
    public class MDFullGrp
    {
        public string MDEntryType { get; set; }
        public decimal MDEntryPx { get; set; }
        public string MDEntryDate { get; set; }
        public string MDEntryTime { get; set; }
        public string Currency { get; set; }
    }

    public class MarketDataResult
    {
        public string Status { get; set; }
        public string Code { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public List<MarketDataValue> Value { get; set; } = new List<MarketDataValue>();
    }
    public class MarketDataValue
    {
        public string MarketDepth { get; set; }
        public string ClearingBusinessDate { get; set; }
        public string MarketID { get; set; }
        public string MarketSegmentID { get; set; }
        public List<Instrument> Instrument { get; set; } = new List<Instrument>();
    }

}
