using Autofac.Extras.NLog;
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
        private readonly IComercialManager oComercial;

        public SISAManager(ILogger logger, IRepositorio repositorio, IComercialManager oComercial)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.oComercial = oComercial;
        }

        public int InsertarSISA(List<SISA> oDatos)
        {
            try
            {
                var sisaViejos = repositorio.Listar<SISA>();
                if (sisaViejos.Count() > 0)
                {
                    var listaVieja = new List<SISA>();
                    foreach (var dato in oDatos)
                    {
                        var categoriaVieja = sisaViejos.Where(x => x.CUIT == dato.CUIT && x.FechaVigenciaCategoria < dato.FechaVigenciaCategoria && x.FechaVigenciaCategoria <= DateTime.Now.Date).OrderBy(x => x.FechaVigenciaCategoria).FirstOrDefault();
                        var estadoViejo = sisaViejos.Where(x => x.CUIT == dato.CUIT && x.FechaVigenciaEstado < dato.FechaVigenciaEstado && x.FechaVigenciaCategoria <= DateTime.Now.Date).OrderBy(x => x.FechaVigenciaEstado).FirstOrDefault();
                        var datoSisa = new SISA();
                        if (categoriaVieja != null && estadoViejo != null)
                        {
                            datoSisa = estadoViejo;
                            datoSisa.FechaVigenciaCategoria = estadoViejo.FechaVigenciaCategoria;
                        }
                        else if (categoriaVieja != null)
                        {
                            datoSisa = categoriaVieja;
                        }
                        else
                        {
                            datoSisa = estadoViejo;
                        }
                        if (datoSisa != null)
                        {
                            listaVieja.Add(datoSisa);
                        }
                    }
                    oDatos.AddRange(listaVieja);
                    repositorio.RemoverTodos<SISA>(x => true);
                    repositorio.AgregarTodos<SISA>(oDatos);
                }
                else
                {
                    repositorio.AgregarTodos<SISA>(oDatos);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return oDatos.Count();
        }
    }
}