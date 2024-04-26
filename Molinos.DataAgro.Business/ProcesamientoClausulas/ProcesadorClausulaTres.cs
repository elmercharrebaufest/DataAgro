using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using Humanizer;
using System.Globalization;
using System.Linq;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaTres : ProcesadorClausula<ClausulaTres>
    {
        public ProcesadorClausulaTres(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {
        }

        public override ResultadoClausula DevolverClausulas(ClausulaTres clausula)
        {
            var res = new ResultadoClausula();
            var contrato = Repositorio.Obtener<Contrato>(x => x.ContratoSAP == clausula.Basico.ContratoSAP);
            // var esFijacionConContratoCanje = clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.FIJACION && contrato.Canje == true;
            var esFijacionContratoConvenio = clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.FIJACION && contrato.Madre == true;
            // SI EL BOLETO ES DE CONTRATO CON PRECIO
            if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
            {
                if (contrato.PorcentajeDePago != null)
                {
                    res.Texto += $"El pago se hará {contrato.PorcentajeDePago} " +
                      $"({DevolverNumeroEnLetras(contrato.PorcentajeDePago.Value)} por ciento), ";
                }
                if ((clausula.Basico.TipoNegocioId == 3 && contrato.Canje == true) || clausula.Basico.TipoNegocioId == 1)
                {
                    res.Texto += contrato.CondicionFijacion.Descripcion;
                }
                if (clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                {
                    if (clausula.Basico.Dias_Pesificado > 0)
                    {
                        res.Texto += $"los {clausula.Basico.Dias_Pesificado.Value} días";
                    }
                    if (clausula.Basico.CD == true)
                    {
                        res.Texto += $"anticipado contra entrega de CD/CG";
                    }
                    if (clausula.Basico.Warrant == true)
                    {
                        res.Texto += $"anticipado contra entrega de WARRANT";
                    }
                    if (clausula.Basico.Dias_Pesificado == null && clausula.Basico.CD != true && clausula.Basico.Warrant != true)
                    {
                        res.Texto += "las 72 hs";
                    }
                }
                if (contrato.PorcentajeDePago != null)
                {
                    res.Texto += $", con mercadería descargada en planta, liquidándose el {(100 - contrato.PorcentajeDePago.Value)} ({DevolverNumeroEnLetras(100 - contrato.PorcentajeDePago.Value)} por ciento) " +
                    $"restante a los 30 (treinta) días del cumplimiento del contrato.";
                }
                if (contrato.PagoDirectoVendedor == true)
                {
                    res.Texto += "El pago se hará en su totalidad al vendedor. ";
                }
            }
            // SI EL BOLETO ES DE CONTRATO SIN PRECIO
            else if ((clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && clausula.Basico.Canje != true) || esFijacionContratoConvenio /* || (esFijacionConContratoCanje)*/)
            {
                if (contrato.PorcentajeDePago != null)
                {
                    res.Texto += $"El pago se hará {contrato.PorcentajeDePago} " +
                      $"({DevolverNumeroEnLetras(contrato.PorcentajeDePago.Value)} por ciento), ";
                }
                if ((clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.FIJACION && contrato.Canje == true) || clausula.Basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
                {
                    res.Texto += contrato.CondicionFijacion.Descripcion;
                }
                res.Texto += $", con mercadería descargada en fabrica,  liquidándose el 2.5% (dos y medio por ciento) restando a los 30 (treinta) días del cumplimiento " + "del contrato. “El precio de la mercadería objeto del presente contrato, se fijará cualquier día hábil a elección del vendedor." +
                $"restante a los 30 (treinta) días del cumplimiento del contrato." +
                $" “El precio de la mercadería objeto del presente contrato, se fijará cualquier día hábil a elección del vendedor. El " + "vendedor comunicara al comprador el día elegido para la fijación de precio por Mercado Disponible de Molinos Agro SA hasta el " + $"{contrato.FechaHasta.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}" + " cualquier día hábil a elección del vendedor”. (También la fijación puede ser por Pizarra Rosario Ciega o por ambas). " + "“A los efectos del impuesto de sellos, únicamente, las partes acuerdan que el precio de referencia corresponde a Pizarra Rosario " + "Soja/Ciega” Las Partes acuerdan la posibilidad de prorrogar el plazo de pago indicado en las cláusulas previas, a opción del " + "Vendedor. En caso de ejercer la opción al momento de fijar el precio, el Vendedor deberá notificar a Molinos Agro que deberá diferir " + "el pago de la Mercadería por el plazo determinado, el mismo no podrá extenderse de los 120 días. El Precio a pagar será neto de " + "los impuestos y retenciones impositivas que correspondieran y se hubieran practicado según la condición del Vendedor y las " + "particularidades del negocio. Toda vez que las Partes han acordado la opción de prorrogar la fecha de pago de la Mercadería, queda " + "expresamente establecido el Vendedor no podrá invocar mora ni reclamar intereses y/o multas y/o cualquier tipo de penalidad por " + "el tiempo transcurrido entre el plazo de pago originario y el del ejercicio de la opción de prórroga acordada en la presente Cláusula" + "";
            }
            return res;
        }

        public string DevolverNumeroEnLetras(decimal numero)
        {
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