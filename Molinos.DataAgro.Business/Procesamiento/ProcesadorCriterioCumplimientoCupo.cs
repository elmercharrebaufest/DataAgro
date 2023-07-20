using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;

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
                var todos = Repositorio.Contar<Cupo>(x => criterio.Dto.ProveedorId.Value == x.ProveedorId && x.FechaIngreso < DateTime.Today && x.Cumplimiento != null);
                decimal result = todos != 0 ? (cumplidos * 100.00m / todos) / 100.00m : 0.00m;
                return Math.Round(result, 2);
            }

            return 1;
        }
    }
}