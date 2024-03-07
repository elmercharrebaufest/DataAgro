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
    public class ProcesadorClausulaCuarentaYNueve : ProcesadorClausula<ClausulaCuarentaYNueve>
    {
        public ProcesadorClausulaCuarentaYNueve(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCuarentaYNueve clausula)
        {
            var res = new ResultadoClausula();

            if (clausula.Basico.MaterialId == (int)EnumMateriales.SOJA &&  clausula.Basico.StandardCalidadId == (int)EnumStandarCalidad.ESPECIAL)
            {
                res.Texto += $" Granos verdes: De 0% a 20% 0 descuento. Del 20,1 en adelante se descontará 0,2% por punto porcentual " + "excedido. Granos verdes: De 0,5% a 30% descuento 0,20. De 30,1% a 45% 0 descuento. De 45,1% a 60% descuento 0,20. Del 60,1% " + "en adelante se descontará 0,2% por punto porcentual excedido.";
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
