using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Business.Managers
{
    public class SISAManager : ISISAManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public SISAManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public int InsertarSISA(List<SISA> oDatos, List<SISA> Cuits)
        {
            try
            {
                var sisaViejos = repositorio.Listar<SISA>();
                foreach (var nuevo in Cuits)
                {
                    var viejo = sisaViejos.Where(x => x.CUIT == nuevo.CUIT && x.CodCategoria == nuevo.CodCategoria).FirstOrDefault();

                    if (viejo != null)
                    {
                        if (nuevo.FechaVigenciaCategoria > DateTime.Now.Date)
                        {
                            nuevo.FechaVigenciaCategoria = viejo.FechaVigenciaCategoria;
                            nuevo.SituacionCategoria = viejo.SituacionCategoria;
                        }
                        if (nuevo.FechaVigenciaEstado > DateTime.Now.Date)
                        {
                            nuevo.FechaVigenciaEstado = viejo.FechaVigenciaEstado;
                            nuevo.EstadoCuit = viejo.EstadoCuit;
                        }
                        oDatos.Add(nuevo);
                    }
                }
                repositorio.RemoverTodos<SISA>(x => true);
                repositorio.AgregarTodos<SISA>(oDatos);                
            }        
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return oDatos.Count();
        }
    }
}