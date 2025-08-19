using Autofac.Extras.NLog;
using Humanizer;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.ProcesamientoClausulas
{
    public class ProcesadorClausulaSesentaYDos : ProcesadorClausula<ClausulaSesentaYDos>
    {
        public ProcesadorClausulaSesentaYDos(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto) : base(repositorio, log, estadoBoleto)
        {
        }
        public override ResultadoClausula DevolverClausulas(ClausulaSesentaYDos clausula)
        {
            //No aplica a carta oferta porque no se sella. Es solo para boleto físico y confirma

            var res = new ResultadoClausula();

            if (clausula.Basico.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA && clausula.Basico.CorredorId == 0)
            {
                if (clausula.Basico.PorcentajeDePago != null)
                {
                    res.Texto += $"El pago se hará {clausula.Basico.PorcentajeDePago}% ({DevolverNumeroEnLetras(clausula.Basico.PorcentajeDePago.Value)} por ciento), 72 hs contra mercadería descargada. a la orden de ${clausula.Basico.Proveedor} en forma irrevocable.";
                }
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
