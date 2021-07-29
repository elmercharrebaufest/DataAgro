using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorCriterioCumplimientoCupo : ProcesadorCriterio<CriterioCumplimientoCupo>
    {
        public ProcesadorCriterioCumplimientoCupo(IRepositorio repositorio, ILogger log)
           : base(repositorio, log)
        {

        }
        public override decimal Calcular(CriterioCumplimientoCupo criterio)
        {
            if (criterio.Dto.ProveedorId.HasValue)
            {
                var cumplidos = Repositorio.Contar<Cupo>(x => x.Cumplimiento == true && criterio.Dto.ProveedorId.Value == x.ProveedorId);
                var todos = Repositorio.Contar<Cupo>(x => criterio.Dto.ProveedorId.Value == x.ProveedorId);

                return cumplidos / 100 * todos;
            }

            return 100;
        }

    }
}
