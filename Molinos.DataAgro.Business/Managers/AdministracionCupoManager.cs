using Autofac.Extras.NLog;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Business.Managers
{
    public class AdministracionCupoManager : IAdministracionCupoManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;
        public AdministracionCupoManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public KendoGrid<AdministracionCupoDto> TraerTodaAdministracionCupo(KendoGridMvcRequest request)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerAdministracionCupoExcedente(request));
        }
        public AdministracionCupoDto TraerAdministracionCupo(int id)
        {
            return repositorio.Obtener<AdministracionCupo, AdministracionCupoDto>(x => x.Id == id, x => new AdministracionCupoDto
            {
                Id = x.Id,
                Fecha = x.Fecha
            });
        }

        public Resultado CambiarEstadoRechazado(int idAdministracion)
        {
            var resultado = new Resultado();
            try
            {
                var administacion = repositorio.Obtener<AdministracionCupo>(idAdministracion);
                administacion.EstadoId = (int)EnumEstadoAdministracionCupo.EstadoRechazadoAdministracionCupo;
                repositorio.GuardarCambios();
                return resultado;

            }catch(Exception e)
            {
                logger.Error(e.Message);
                resultado.Error("", e.Message);
                return resultado;
            }

        }
    }
}
