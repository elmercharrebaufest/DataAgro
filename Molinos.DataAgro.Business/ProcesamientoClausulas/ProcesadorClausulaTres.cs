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
            var esFijacionConContratoCanje = clausula.Basico.TipoNegocioId == 3 && contrato.Canje == true;
            if ((clausula.Basico.TipoNegocioId == 1 && clausula.Basico.Canje != true) || (esFijacionConContratoCanje) || clausula.Basico.TipoNegocioId == 2)
            {
                if (contrato.PorcentajeDePago != null)
                {
                    res.Texto += $"El pago se hará {  contrato.PorcentajeDePago  } " +
                      $"({ DevolverNumeroEnLetras(contrato.PorcentajeDePago.Value) } por ciento), ";
                }
                if ((clausula.Basico.TipoNegocioId == 3 && contrato.Canje == true) || clausula.Basico.TipoNegocioId == 1)
                {
                    res.Texto += contrato.CondicionFijacion.Descripcion;
                }
                if (clausula.Basico.TipoNegocioId == 2)
                {
                    if (clausula.Basico.Dias_Pesificado > 0)
                    {
                        res.Texto += $"los {clausula.Basico.Dias_Pesificado.Value } días";
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
                    res.Texto += $", con mercadería descargada en planta, liquidándose el { (100 - contrato.PorcentajeDePago.Value) } ({ DevolverNumeroEnLetras(100 - contrato.PorcentajeDePago.Value) } por ciento) " +
                    $"restante a los 30 (treinta) días del cumplimiento del contrato.";
                }               
                if (contrato.PagoDirectoVendedor == true)
                {
                    res.Texto += "El pago se hará en su totalidad al vendedor. ";
                }
            }
            return res;
        }

        public string DevolverNumeroEnLetras(decimal numero)
        {
            var letras = ((int)Math.Abs(numero)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper();
            var fraccion = numero - Math.Floor(numero);
            if (fraccion > 0)
            {
                letras += $" con {((int)(fraccion * 100)).ToWords(CultureInfo.GetCultureInfo("es-AR")).ToUpper() }";

            }
            return letras;
        }
    }
}
