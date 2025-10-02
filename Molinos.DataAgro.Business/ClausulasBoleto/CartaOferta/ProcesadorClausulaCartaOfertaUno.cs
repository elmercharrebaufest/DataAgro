using Autofac.Extras.NLog;
using Humanizer;
using Molinos.DataAgro.Entities.ClausulasBoleto.CartaOferta;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Globalization;

namespace Molinos.DataAgro.Business.ClausulasBoleto.CartaOferta
{
    public class ProcesadorClausulaCartaOfertaUno : ProcesadorClausulaCartaOferta<ClausulaCartaOfertaUno>
    {
        public ProcesadorClausulaCartaOfertaUno(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto) : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCartaOfertaUno clausula)
        {
            var res = new ResultadoClausula();

            res.Texto = "La VENDEDORA vende a la COMPRADORA";
            res.Texto += $" la cantidad de {clausula.Basico.Cantidad.ToString("#,##0.##", CultureInfo.GetCultureInfo("es-ES"))} kg. " +
                         $"(kilogramos {((int)Math.Abs(clausula.Basico.Cantidad)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper()}) de {clausula.Basico.Material} {(clausula.Basico.CantidadCamiones != 0 ? " o el resultante de " + clausula.Basico.CantidadCamiones + "camiones" : "")}" +
                         $"y demás condiciones ";
            if (clausula.Basico.TrigoEspecial == true && clausula.Basico.StandardDeCalidadId != 7)
            {
                res.Texto += "CALIDAD ESPECIAL ";
            }
            else
            {
                res.Texto += $"{clausula.Basico.Material} " + clausula.Basico.StandardDeCalidadDescripcion.ToUpper() + " ";
            }
            if (clausula.Basico.Calidades != null && clausula.Basico.StandardDeCalidadId != 7)
            {
                foreach (var cal in clausula.Basico.Calidades)
                {
                    res.Texto += cal.CalidadEspecialDesc.ToUpper() + " " + cal.Valor.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR")) + "";
                    if (cal.PorcentajeDesde != null && cal.PorcentajeHasta != null)
                    {
                        res.Texto += "Porc. Desde " + cal.PorcentajeDesde + "% Hasta " + cal.PorcentajeHasta + "% ";
                    }
                }
            }
            res.Texto += $"de la cosecha {clausula.Basico.Campania}, ";
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje != true)
            {
                res.Texto += "con precio a fijar. ";
            }
            else if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje == true)
            {
                res.Texto += $"para cancelar {clausula.Basico.Insumo} - {clausula.Basico.Monto} {clausula.Basico.MonedaCanjeId} " +
                    $"(en adelante, el {"Insumo"}) así como también para cancelar los gastos asociados a los que el Vendedor hubiere incurrido para llevar a cabo la presente operación. " +
                    $"(en adelante, los {"Gastos Asociados"}). Las Partes acuerdan que el Insumo será a retirar en puerto por el Vendedor. ";

            }

            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
            {
                res.Texto += $"a {clausula.Basico.PrecioNeto.Value.ToString("#,##0.##", CultureInfo.GetCultureInfo("es-ES"))} {MetodosUtiles.DivisaSimbolica(clausula.Basico.Moneda)} ({MetodosUtiles.DivisaEnLetras(clausula.Basico.Moneda)} {MetodosUtiles.DevolverNumeroEnLetrasConDivisa(clausula.Basico.PrecioNeto.Value, clausula.Basico.Moneda)}) más IVA la tonelada .";
            }
            res.Texto += $"Procedencia de la mercadería: {clausula.Basico.Localidad} - {clausula.Basico.Provincia}.";
            res.Texto += $"A todos los efectos impositivos los vendedores declaran que la mercadería {(clausula.Basico.ClasificacionDescripcion == "Productor" ? "SI" : "NO")} es de su propia producción{(clausula.Basico.Consignatario == true ? ", actúa en carácter de consignatario." : ".")}";

            return res;
        }
    }

}
