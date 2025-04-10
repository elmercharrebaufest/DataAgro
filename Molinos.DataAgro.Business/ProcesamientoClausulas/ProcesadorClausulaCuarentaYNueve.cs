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

            if ((clausula.Basico.MaterialId == (int)EnumMateriales.MAIZ || clausula.Basico.MaterialId == (int)EnumMateriales.TRIGO) && clausula.Basico.StandardDeCalidadId == (int)EnumStandarCalidad.ESPECIAL)
            {
                res.Texto += $"Condición de la mercadería Grado 2: No bonifica Grado 1, no bonifica ni rebaja Grado 2, rebaja Grado 3 y demás condiciones cámara.";
            }

            return res;
        }
    }
}
