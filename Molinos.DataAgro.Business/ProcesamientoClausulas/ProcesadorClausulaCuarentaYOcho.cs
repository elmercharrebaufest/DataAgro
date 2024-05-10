using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaCuarentaYOcho : ProcesadorClausula<ClausulaCuarentaYOcho>
    {
        public ProcesadorClausulaCuarentaYOcho(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCuarentaYOcho clausula)
        {
            // CONDICIÓN DE LA SIGUIENTE CLÁUSULA: SI EL CAMPO FECHA_CIERTA ESTÁ COMPLETO

            var res = new ResultadoClausula();
            if (clausula.Basico.FechaCierta.HasValue && clausula.Basico.PorcentajeDePago.HasValue)
            {
                res.Texto += $" El pago del {clausula.Basico.PorcentajeDePago.Value}% se efectuará en la fecha indicada en ‘Información sobre pagos’ con la condición que con 72 hs de anticipación se hayan cumplido los requisitos exigibles para el pago. " +
                    $"En caso contrario, el pago se realizará a las 72 hs de cumplidos los requisitos previamente mencionados. El {100 - clausula.Basico.PorcentajeDePago.Value}% restante se pagará durante los 30 días posteriores.";
            }
            return res;
        }
    }
}
