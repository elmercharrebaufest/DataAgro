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
        ResultIniCriterio TraerCriteriosGuardados();
        Resultado GrabarCriterio(CriterioIni criterio);
        Resultado eliminarCriterio(CriterioIni criterio);
        ResultIniFormula ultimaFormula();
        Resultado actualizarDias(FormulaIni formulaDias);
        List<CriterioIni> todosLosCriterios();
    }
}
