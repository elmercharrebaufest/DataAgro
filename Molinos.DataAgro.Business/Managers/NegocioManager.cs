using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace Molinos.DataAgro.Business.Managers
{
    public class NegocioManager : INegocioManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;


        public NegocioManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public Resultado OcultarEnTablero(Negocio negocio)
        {
            var error = new Resultado();
            try
            {
                var negocioAMarcar = repositorio.Obtener<Negocio>(x => x.Id == negocio.Id);
                negocioAMarcar.OcultarEnTablero = negocio.OcultarEnTablero;
                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                error.Error("", e.Message);
            }
            return error;
        }
    }
}
