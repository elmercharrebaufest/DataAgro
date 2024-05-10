using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using Humanizer;
using System.Globalization;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaCincuentaYUno : ProcesadorClausula<ClausulaCincuentaYUno>
    {
        public ProcesadorClausulaCincuentaYUno(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCincuentaYUno clausula)
        {
            var res = new ResultadoClausula();

            // EL MATERIAL ES SOJA Y ES SUSTENTABLE FUERA DE PRECIO EN USDM
            if (clausula.Basico.MaterialId == (int)EnumMateriales.SOJA && (bool)clausula.Basico.Sustentable && clausula.Basico.MonedaId_Sustentable.Equals("USDM ") && clausula.Basico.SustentableTipoDBId == (int)EnumTipoDB.POR_FUERA_DEL_PRECIO)
            {
                res.Texto += $" Las Partes establecen que, para el caso que al momento de la entrega se constate que la mercadería entregada no cumple con los requisitos de soja SUSTENTABLE, el precio convenido será bonificado en la cantidad de USD {clausula.Basico.Importe_Sustentable?.ToString("N", new CultureInfo("es-AR"))} (dólares estadounidenses {DevolverNumeroEnLetras((decimal)clausula.Basico.Importe_Sustentable)}) por tonelada de soja no SUSTENTABLE, a favor del Comprador. Dicha bonificación será descontada al momento del pago de cada una de las liquidaciones parciales y/o finales según corresponda.”";
            }
            // EL MATERIAL ES SOJA Y ES SUSTENTABLE FUERA DE PRECIO EN ARP
            else if (clausula.Basico.MaterialId == (int)EnumMateriales.SOJA && (bool)clausula.Basico.Sustentable && clausula.Basico.MonedaId_Sustentable.Equals("ARP  ") && clausula.Basico.SustentableTipoDBId == (int)EnumTipoDB.POR_FUERA_DEL_PRECIO)
            {
                res.Texto += $" Las Partes establecen que, para el caso que al momento de la entrega se constate que la mercadería entregada no cumple con los requisitos de soja SUSTENTABLE, el precio convenido será bonificado en la cantidad de $ {clausula.Basico.Importe_Sustentable?.ToString("N", new CultureInfo("es-AR"))} (pesos {DevolverNumeroEnLetras((decimal)clausula.Basico.Importe_Sustentable)}) por tonelada de soja no SUSTENTABLE, a favor del Comprador. Dicha bonificación será descontada al momento del pago de cada una de las liquidaciones parciales y/o finales según corresponda.”";
            }
            // EL MATERIAL ES SOJA Y ES SUSTENTABLE SIN TARIFA
            else if (clausula.Basico.MaterialId == (int)EnumMateriales.SOJA && (bool)clausula.Basico.Sustentable && clausula.Basico.MonedaId_Sustentable == null && clausula.Basico.SustentableTipoDBId == null)
            {
                res.Texto = $"Al precio convenido se le adicionará la tarifa que corresponda por tonelada de soja sustentable / EPA entregada, siempre que la entrega de la mercadería fijada haya sido recibida dentro de los plazos de entrega y recibos pautados en el presente Boleto. La tarifa será fijada entre ambas partes y comunicada en oportunidad de recibo de mercadería.";
            }
            // EL MATERIAL ES SOJA Y ES EPA SOBRE PRECIO EN USDM
            else if (clausula.Basico.MaterialId == (int)EnumMateriales.SOJA && (bool)clausula.Basico.EPA && clausula.Basico.MonedaId_Sustentable.Equals("USDM ") && clausula.Basico.SustentableTipoDBId == (int)EnumTipoDB.SOBRE_EL_PRECIO)
            {
                res.Texto = $"Las Partes establecen que, para el caso que al momento de la entrega se constate que la mercadería entregada no cumple con las especificaciones y certificaciones establecidas para la Soja Sustentable “EPA” (certificación de establecimiento e indicación “EPA” en Observaciones de la Carta de Porte) y condiciones de entrega (fecha de entrega desde {clausula.Basico.FechaDesde.GetValueOrDefault():dd'/'MM'/'yyyy} hasta {clausula.Basico.FechaHasta.GetValueOrDefault():dd'/'MM'/'yyyy} y código de cupo), el precio será bonificado en la cantidad de USD {clausula.Basico.Monto?.ToString("N", new CultureInfo("es-AR"))} (dólares estadounidenses {DevolverNumeroEnLetras((decimal)clausula.Basico.Monto)}) por tonelada, a favor del Comprador. Dicha bonificación será descontada al momento del pago de cada una de las liquidaciones parciales y/o finales según corresponda.";
            }
            // EL MATERIAL ES SOJA Y ES EPA SOBRE PRECIO EN ARP
            else if (clausula.Basico.MaterialId == (int)EnumMateriales.SOJA && (bool)clausula.Basico.EPA && clausula.Basico.MonedaId_Sustentable.Equals("ARP  ") && clausula.Basico.SustentableTipoDBId == (int)EnumTipoDB.SOBRE_EL_PRECIO)
            {
                res.Texto = $"Las Partes establecen que, para el caso que al momento de la entrega se constate que la mercadería entregada no cumple con las especificaciones y certificaciones establecidas para la Soja Sustentable “EPA” (certificación de establecimiento e indicación “EPA” en Observaciones de la Carta de Porte) y condiciones de entrega (fecha de entrega desde {clausula.Basico.FechaDesde.GetValueOrDefault():dd'/'MM'/'yyyy} hasta {clausula.Basico.FechaHasta.GetValueOrDefault():dd'/'MM'/'yyyy} y código de cupo), el precio será bonificado en la cantidad de $ {clausula.Basico.ImporteSobrePrecioContrato?.ToString("N", new CultureInfo("es-AR"))} (pesos {DevolverNumeroEnLetras((decimal)clausula.Basico.ImporteSobrePrecioContrato)}) por tonelada, a favor del Comprador. Dicha bonificación será descontada al momento del pago de cada una de las liquidaciones parciales y/o finales según corresponda.";
            }
            // EL MATERIAL ES SOJA Y ES EPA FUERA DE PRECIO EN USDM
            else if (clausula.Basico.MaterialId == (int)EnumMateriales.SOJA && (bool)clausula.Basico.EPA && clausula.Basico.MonedaId_Sustentable.Equals("USDM ") && clausula.Basico.SustentableTipoDBId == (int)EnumTipoDB.POR_FUERA_DEL_PRECIO)
            {
                res.Texto = $" Al precio se le adicionará la tarifa convenida de {clausula.Basico.Importe_Sustentable?.ToString("N", new CultureInfo("es-AR"))} USDM por tonelada de soja sustentable “EPA” (certificación de establecimiento e indicación “EPA” en Observaciones de la Carta de Porte) entregada, siempre cuando la mercadería fijada haya sido entregada conforme los plazos de recibo pautados en el presente Boleto.";
            }
            // EL MATERIAL ES SOJA Y ES EPA FUERA DE PRECIO EN ARP
            else if (clausula.Basico.MaterialId == (int)EnumMateriales.SOJA && (bool)clausula.Basico.EPA && clausula.Basico.MonedaId_Sustentable.Equals("ARP  ") && clausula.Basico.SustentableTipoDBId == (int)EnumTipoDB.POR_FUERA_DEL_PRECIO)
            {
                res.Texto += $"Al precio se le adicionará la tarifa convenida de {clausula.Basico.Importe_Sustentable?.ToString("N", new CultureInfo("es-AR"))} PESOS por tonelada de soja sustentable “EPA” (certificación de establecimiento e indicación “EPA” en Observaciones de la Carta de Porte) entregada, siempre cuando la mercadería fijada haya sido entregada conforme los plazos de recibo pautados en el presente Boleto.";
            }
            // EL MATERIAL ES SOJA Y ES EPA SIN TARIFA
            else if (clausula.Basico.MaterialId == (int)EnumMateriales.SOJA && (bool)clausula.Basico.EPA && clausula.Basico.MonedaId_Sustentable == null && clausula.Basico.SustentableTipoDBId == null)
            {
                res.Texto += $"En caso que el Vendedor entregue soja sustentable “EPA” (certificación de establecimiento e indicación “EPA” en Observaciones de la Carta de Porte), al precio convenido se le adicionará un monto por tonelada de soja sustentable “EPA” entregada, que será convenido entre las partes al momento de efectuar cada pago, siempre y cuando la mercadería fijada haya sido entregada conforme los plazos de recibo pautados en el presente Boleto.";
            }
            return res;
        }

        private string DevolverNumeroEnLetras(decimal numero)
        {
            numero = Math.Round(numero, 2);
            var letras = ((int)Math.Abs(numero)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper();
            var fraccion = numero - Math.Floor(numero);
            if (fraccion > 0)
            {
                letras += $" con {((int)(fraccion * 100)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper()}";
            }
            return letras;
        }
    }
}
