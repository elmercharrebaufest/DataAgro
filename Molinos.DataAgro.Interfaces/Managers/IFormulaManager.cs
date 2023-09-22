using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces.Managers
{
    public interface IFormulaManager
    {
        ResultIniCriterio TraerCriteriosGuardados(int MaterialId);
        Resultado GrabarCriterio(CriterioIni criterio);
        Resultado EliminarCriterio(CriterioIni criterio);
        ResultIniFormula UltimaFormula(int MaterialId);
        Resultado ActualizarDias(FormulaIni formulaDias);
        List<CriterioIni> TodosLosCriterios();
        Resultado ActualizarCierre(FormulaIni formulaDias);
        ResultIniTipoNegocioExcluido TraerTiposNegociosExcluidosGuardados(int MaterialId);
        Resultado ActualizarTiposNegociosExcluidos(int materialId, List<TipoNegocioDto> tiposNegociosExcluidos, FormulaIni formulaDias);
    }
}
