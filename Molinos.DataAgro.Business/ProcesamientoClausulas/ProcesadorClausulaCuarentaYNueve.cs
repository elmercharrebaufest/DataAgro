using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
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

            //EL MATERIAL ES SOJA Y TIENE CALIDAD ESPECIAL GRANOS VERDES
            if (clausula.Basico.MaterialId == (int)EnumMateriales.SOJA && clausula.Basico.StandardDeCalidadId == (int)EnumStandarCalidad.ESPECIAL)
            {
                res.Texto += $"Granos verdes: De 0% a 20% 0 descuento. Del 20,1 en adelante se descontará 0,2% por punto porcentual excedido. " +
                    $"Granos verdes: De 0,5% a 30% descuento 0,20. De 30,1% a 45% 0 descuento. De 45,1% a 60% descuento 0,20. Del 60,1% en adelante se descontará 0,2% por punto porcentual excedido.";
            }

            //EL MATERIAL ES MAIZ O TRIGO Y TIENE CALIDAD ESPECIAL
            else if ((clausula.Basico.MaterialId == (int)EnumMateriales.MAIZ || clausula.Basico.MaterialId == (int)EnumMateriales.TRIGO) && clausula.Basico.StandardDeCalidadId == (int)EnumStandarCalidad.ESPECIAL)
            {
                res.Texto += $"Condición de la mercadería Grado 2: No bonifica Grado 1, no bonifica ni rebaja Grado 2, rebaja Grado 3 y demás condiciones cámara.";
            }

            return res;
        }
    }
}
