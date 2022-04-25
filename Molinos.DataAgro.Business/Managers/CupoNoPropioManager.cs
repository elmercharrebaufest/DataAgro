using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Agent.Helpers;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Criterios;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.IO;
using Molinos.DataAgro.Agent;

namespace Molinos.DataAgro.Business.Managers
{
    public class CupoNoPropioManager : ICupoNoPropioManager
    {
        private IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly IProveedorManager proveedorManager;
        private IMailManager mailManager;
        private IHttpContextManager httpContextManager;
        private IConfiguracionCupoManager configuracionCupoManager;
        private ICentroManager centroManager;

        public CupoNoPropioManager(IRepositorio repositorio, ILogger logger, IProveedorManager proveedorManager, IMailManager mailManager,
            IHttpContextManager httpContextManager, IConfiguracionCupoManager configuracionCupoManager, ICentroManager centroManager)
        {
            this.repositorio = repositorio;
            this.logger = logger;
            this.proveedorManager = proveedorManager;
            this.mailManager = mailManager;
            this.httpContextManager = httpContextManager;
            this.configuracionCupoManager = configuracionCupoManager;
            this.centroManager = centroManager;
        }
        public CupoResult GrabarCupoNoPropio(CupoDto cupoDto)
        {
            var resultado = ValidarCupo(cupoDto);
            if (resultado.HayError)
            {
                return resultado;
            }
            cupoDto.CentroId = centroManager.ObtenerCentroPorCodigoSap(cupoDto.Centro).Id;
            var cuposEstado = new List<CupoNoPropioDto>();
            var cuposSave = new List<CupoNoPropio>();
            try
            {
                var codigos = cupoDto.Codigo.Replace(" ", "").ToUpper().Split(';').Distinct().ToList();
                codigos.RemoveAll(codigo => string.IsNullOrEmpty(codigo));
                var estado = new List<string>();
                var cantidadCupos = 0;
                var cupos = repositorio.ListarEntidadMasiva<CupoNoPropio>("Codigo", codigos);
                foreach (var c in codigos)
                {
                    if (cupos.Exists(x => x.Codigo.Equals(c)))
                    {
                        cuposEstado.Add(new CupoNoPropioDto { Codigo = c, EstadoId = 1 });
                        resultado.Error("Error", "El cupo ingresado ya se encuentra registrado");
                    }
                    else
                    {
                        cuposSave.Add(new CupoNoPropio()
                        {
                            MaterialId = cupoDto.MaterialId,
                            CentroId = cupoDto.CentroId,
                            Codigo = c,
                            FechaAlta = DateTime.Now,
                            FechaIngreso = cupoDto.FechaIngreso,
                            Disponible = true,
                            Estado = 6 
                        });
                        cuposEstado.Add(new CupoNoPropioDto { Codigo = c, EstadoId = 0 });
                        cantidadCupos = cantidadCupos + 1;
                    }
                }
                repositorio.AgregarTodos(cuposSave);
                if (cantidadCupos > 0)
                {
                    CrearCuperaNoPropio(cupoDto, cantidadCupos);
                }
                resultado.CupoNoPropios = cuposEstado.OrderBy(x => x.EstadoId == 1).ToList();

                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                logger.Error(e);
                resultado.Error(e.Source, e.Message);
                throw;
            }
            return resultado;
        }

        private CupoResult ValidarCupo(CupoDto cupoModel)
        {
            var resultado = new CupoResult();
            if (cupoModel.FechaIngreso == null)
            {
                resultado.Error("", "La Fecha de Ingreso es requerida");
                return resultado;
            }
            if (cupoModel.FechaIngreso.Date < DateTime.Now.Date)
            {
                resultado.Error("", "La Fecha de Ingreso no puede ser menor a la fecha actual");
                return resultado;
            }
            if (cupoModel.MaterialId == 0)
            {
                resultado.Error("", "El Material es requerido");
                return resultado;
            }
            if (string.IsNullOrEmpty(cupoModel.Centro))
            {
                resultado.Error("", "La Planta es requerida");
                return resultado;
            }
            if (string.IsNullOrEmpty(cupoModel.Codigo))
            {
                resultado.Error("", "Debe ingresar el codigo de cupos");
                return resultado;
            }
            return resultado;
        }
        private void CrearCuperaNoPropio(CupoDto cupo, int cantidad)
        {

            var configuracionSave = repositorio.Obtener<ConfiguracionCupo>(x => x.Fecha == cupo.FechaIngreso &&
            x.CentroId == cupo.CentroId && x.MaterialId == cupo.MaterialId);
            var dias = new List<DiaCupo> { new DiaCupo { Fecha = cupo.FechaIngreso, CantidadAlgoritmo = 0, Cantidad = (configuracionSave == null ? cantidad : (configuracionSave.LimiteCupo + cantidad)) } };

            var configuracion = new ConfiguracionCupo
            {
                Id = configuracionSave != null ? configuracionSave.Id : 0,
                CentroId = cupo.CentroId,
                MaterialId = cupo.MaterialId,
                Fecha = cupo.FechaIngreso,
                LimiteCupo = configuracionSave != null ? (configuracionSave.LimiteCupo + cantidad) : 0,

            };
            configuracionCupoManager.GrabarConfiguracionCupo(configuracion, dias);
        }

        public DataSourceResult TraerCuposNoPropioTabla(DataSourceRequest request, List<int> equipo)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerTodosCupoNoPropio(request, equipo));
        }

        public Resultado GrabarDisponibilidadCupoNoPropio(int id, bool disponibilidad)
        {
            var resultado = new Resultado();
            try
            {
                var cupo = repositorio.Obtener<CupoNoPropio>(id);
                cupo.Disponible = disponibilidad;
                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                logger.Error(e);
                resultado.Error(e.Source, e.Message);
                throw;
            }
            return resultado;
        }

    }
}