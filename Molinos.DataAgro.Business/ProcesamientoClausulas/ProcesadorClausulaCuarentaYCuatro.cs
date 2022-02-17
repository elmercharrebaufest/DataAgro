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
    public class ProcesadorClausulaCuarentaYCuatro : ProcesadorClausula<ClausulaCuarentaYCuatro>
    {
        public ProcesadorClausulaCuarentaYCuatro(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCuarentaYCuatro clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.TipoNegocioId == 2 && (clausula.Basico.ClasificacionContrato == "ACOPIADOR" || (clausula.Basico.CorredorId > 0 && clausula.Basico.Moneda == "USD")))
            {
                res.Texto += "Toda vez que el Acopiador/Corredor no proceda a liquidar la mercadería dentro de las 72 horas desde que la misma fuera entregada y " +
                    "aplicada, las Partes acuerdan que quedará a opción del Comprador determinar el día que se tomará válido para establecer el tipo de cambio a " +
                    "utilizar en los términos dispuestos en el presente boleto";
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
