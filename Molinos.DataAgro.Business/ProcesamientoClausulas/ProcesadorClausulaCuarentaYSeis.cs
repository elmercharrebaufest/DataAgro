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
    public class ProcesadorClausulaCuarentaYSeis : ProcesadorClausula<ClausulaCuarentaYSeis>
    {
        public ProcesadorClausulaCuarentaYSeis(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCuarentaYSeis clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == 1)
            {
                res.Texto += "Las Partes acuerdan la posibilidad de prorrogar el plazo de pago indicado en las cláusulas previas. El precio a pagar será neto de " +
                    "los impuestos y retenciones impositivas que correspondieran y se hubieran practicado según la condición del Vendedor y las particularidades " +
                    "del negocio. Toda vez que las Partes han acordado la posibilidad de prorrogar la fecha de pago de la Mercadería, queda expresamente establecido " +
                    "que el Vendedor no podrá invocar mora ni reclamar intereses y/o multas y/o cualquier tipo de penalidad por el tiempo transcurrido entre el " +
                    "plazo de pago originario y el del ejercicio de la opción de prórroga acordada en la presente Cláusula.";
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
