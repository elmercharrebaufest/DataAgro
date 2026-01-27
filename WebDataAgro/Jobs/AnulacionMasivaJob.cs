using Hangfire;
using Molinos.DataAgro.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebDataAgro.Jobs
{
    public interface IAnulacionMasivaJob
    {
        Task Ejecutar(List<int> equipo, List<int> ids, string path, string comercialId);
    }
    public class AnulacionMasivaJob : IAnulacionMasivaJob
    {
        private readonly ICupoManager cupoManager;
        private readonly ILogger logger;

        public AnulacionMasivaJob(ICupoManager cupoManager, ILogger logger)
        {
            this.cupoManager = cupoManager;
            this.logger = logger;
        }

        [DisableConcurrentExecution(60 * 30)]
        public async Task Ejecutar(
            List<int> equipo,
            List<int> ids,
            string path,
            string comercialId)
        {
            try
            {
                await cupoManager.AnulacionMasiva2(
                    equipo, comercialId, ids, path);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error en AnulacionMasivaJob");
                throw; // Hangfire lo marca como fallido
            }
        }
    }
}
