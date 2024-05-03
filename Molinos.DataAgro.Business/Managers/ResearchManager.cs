using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces.Agent;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace Molinos.DataAgro.Business.Managers
{
    public class ResearchManager : IResearchManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IClienteResearchAgent clienteResearchAgent;

        public ResearchManager(ILogger logger, IRepositorio repositorio, IClienteResearchAgent clienteResearchAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.clienteResearchAgent = clienteResearchAgent;
        }

        public Resultado EliminarResearchAvanceSiembra(int researchAvanceSiembraId)
        {
            var oEntityErrors = new Resultado();
            var tc = repositorio.Obtener<ResearchAvanceSiembra>(x => x.Id == researchAvanceSiembraId);
            try
            {
                repositorio.Remover(tc);
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            if (!oEntityErrors.HayError)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se eliminó correctamente"));
            }
            return oEntityErrors;
        }

        public Resultado GrabarResearchAvanceSiembra(ResearchAvanceSiembra researchAvanceSiembra, int comercialId)
        {
            var oEntityErrors = ValidarAvanceSiembra(researchAvanceSiembra);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            try
            {
                researchAvanceSiembra.ComercialId = comercialId;
                researchAvanceSiembra.FechaHora = DateTime.Now;
                repositorio.Agregar(researchAvanceSiembra);
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            if (!oEntityErrors.HayError)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se guardó correctamente"));
            }
            return oEntityErrors;
        }

        public List<ResearchAvanceSiembraDto> TraerTodoResearchAvanceSiembra()
        {
            var hoy = DateTime.Now.Date;
            return repositorio.Listar<ResearchAvanceSiembra, ResearchAvanceSiembraDto>(x => new ResearchAvanceSiembraDto
            {
                Id = x.Id,
                Avance = x.Avance,
                CambioAA = x.CambioAA,
                IntencionSiembra = x.IntencionSiembra,
                LocalidadId = x.LocalidadId,
                Observaciones = x.Observaciones,
                MaterialId = x.MaterialId,
                FechaHora = x.FechaHora,
                Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                Material = x.Material.Descripcion + "",
                Localidad = x.Localidad.Nombre + "",
                Campania = x.Campania.Descripcion + "",
                CampaniaId = x.CampaniaId
            }, x => DbFunctions.TruncateTime(x.FechaHora) == hoy);
        }

        public List<ResearchAvanceCosechaDto> TraerTodoResearchAvanceCosecha()
        {
            var hoy = DateTime.Now.Date;
            return repositorio.Listar<ResearchAvanceCosecha, ResearchAvanceCosechaDto>(x => new ResearchAvanceCosechaDto
            {
                Id = x.Id,
                Avance = x.Avance,
                Rendimiento = x.Rendimiento,
                RangoDesde = x.RangoDesde,
                RangoHasta = x.RangoHasta,
                LocalidadId = x.LocalidadId,
                Observaciones = x.Observaciones,
                MaterialId = x.MaterialId,
                FechaHora = x.FechaHora,
                Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                Material = x.Material.Descripcion + "",
                Localidad = x.Localidad.Nombre + "",
                Campania = x.Campania.Descripcion + "",
                CampaniaId = x.CampaniaId
            }, x => DbFunctions.TruncateTime(x.FechaHora) == hoy);
        }

        public Resultado GrabarResearchAvanceCosecha(ResearchAvanceCosecha researchAvanceCosecha, int comercialId)
        {
            var oEntityErrors = ValidarAvanceCosecha(researchAvanceCosecha);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            try
            {
                researchAvanceCosecha.ComercialId = comercialId;
                researchAvanceCosecha.FechaHora = DateTime.Now;
                repositorio.Agregar(researchAvanceCosecha);
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            if (!oEntityErrors.HayError)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se guardó correctamente"));
            }
            return oEntityErrors;
        }

        public Resultado EliminarResearchAvanceCosecha(int researchAvanceCosechaId)
        {
            var oEntityErrors = new Resultado();
            var tc = repositorio.Obtener<ResearchAvanceCosecha>(x => x.Id == researchAvanceCosechaId);
            try
            {
                repositorio.Remover(tc);
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            if (!oEntityErrors.HayError)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se eliminó correctamente"));
            }
            return oEntityErrors;
        }
        
        private Resultado ValidarAvanceSiembra(ResearchAvanceSiembra researchAvanceSiembra)
        {
            var error = new Resultado();
            if (researchAvanceSiembra.MaterialId == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Cultivo no puede estar vacío"));
            }
            if (researchAvanceSiembra.LocalidadId <= 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Localidad no debe estar vacío"));
            }
            if (researchAvanceSiembra.IntencionSiembra == 0 && researchAvanceSiembra.Avance == 0 &&
                researchAvanceSiembra.CambioAA == 0 && researchAvanceSiembra.CampaniaId <= 0)
            {
                error.Errores.Add(new ErrorMessage(400, "Debe llenar todos los campos"));
            }
            else if (researchAvanceSiembra.IntencionSiembra == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Intención Siembra no debe estar vacío"));
            }
            else if (researchAvanceSiembra.Avance == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Avance no debe estar vacío"));
            }
            else if (researchAvanceSiembra.CambioAA == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Cambio vs AA no debe estar vacío"));
            }
            else if (researchAvanceSiembra.CampaniaId <= 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Campaña no debe estar vacío"));
            }
            return error;
        }

        private Resultado ValidarAvanceCosecha(ResearchAvanceCosecha researchAvanceCosecha)
        {
            var error = new Resultado();

            if (researchAvanceCosecha.MaterialId == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Cultivo no puede estar vacío"));
            }
            if (researchAvanceCosecha.LocalidadId <= 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Localidad no debe estar vacío"));
            }
            if (researchAvanceCosecha.RangoDesde == 0 && researchAvanceCosecha.RangoHasta == 0 && researchAvanceCosecha.Rendimiento == 0 && researchAvanceCosecha.Avance == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "Debe llenar todos los campos"));
            }
            if (researchAvanceCosecha.RangoDesde == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Rango Desde no debe estar vacío"));
            }
            if (researchAvanceCosecha.RangoHasta == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Rango Hasta Desde no debe estar vacío"));
            }
            if (researchAvanceCosecha.RangoHasta < researchAvanceCosecha.RangoDesde)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Rango Hasta no debe ser menor que Rango Desde"));
            }
            if (researchAvanceCosecha.Avance == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Avance no debe estar vacío"));
            }
            if (researchAvanceCosecha.Rendimiento == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Rendimiento no debe estar vacío"));
            }
            if (researchAvanceCosecha.CampaniaId <= 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Campaña no debe estar vacío"));
            }
            return error;
        }

        private Resultado ValidarSituacionCultivo(ResearchSituacionCultivo researchSituacionCultivo)
        {
            var error = new Resultado();

            if (researchSituacionCultivo.MaterialId == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Cultivo no puede estar vacío"));
            }
            if (researchSituacionCultivo.LocalidadId <= 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Localidad no debe estar vacío"));
            }
            if (String.IsNullOrEmpty(researchSituacionCultivo.Situacion))
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Situacion no debe estar vacío"));
            }
            if (researchSituacionCultivo.EstadioId <= 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Estadío no debe estar vacío"));
            }
            if (researchSituacionCultivo.CampaniaId <= 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Campaña no debe estar vacío"));
            }



            return error;
        }

        private Resultado ValidarVentaStock(ResearchVentaStock researchVentaStock)
        {
            var error = new Resultado();
            if (researchVentaStock.MaterialId == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Cultivo no puede estar vacío"));
            }
            if (researchVentaStock.LocalidadId <= 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Localidad no debe estar vacío"));
            }
            if (researchVentaStock.Almacenado == 0 && researchVentaStock.VendidoAPrecio == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "Debe llenar todos los campos"));
            }
            if (researchVentaStock.Almacenado == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Almacenado no debe estar vacío"));
            }
            if (researchVentaStock.VendidoAPrecio == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Vendido a Precio no debe estar vacío"));
            }
            if (researchVentaStock.CampaniaId <= 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El campo Campaña no debe estar vacío"));
            }
            return error;
        }

        public List<ResearchSituacionCultivoDto> TraerTodoResearchSituacionCultivo()
        {
            var hoy = DateTime.Now.Date;
            return repositorio.Listar<ResearchSituacionCultivo, ResearchSituacionCultivoDto>(x => new ResearchSituacionCultivoDto
            {
                Id = x.Id,
                Estadio = x.Estadio.Descripcion + "",
                EstadioId = x.EstadioId,
                Situacion = x.Situacion,
                LocalidadId = x.LocalidadId,
                Observaciones = x.Observaciones,
                MaterialId = x.MaterialId,
                FechaHora = x.FechaHora,
                Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                Material = x.Material.Descripcion + "",
                Localidad = x.Localidad.Nombre + "",
                Campania = x.Campania.Descripcion + "",
                CampaniaId = x.CampaniaId
            }, x => DbFunctions.TruncateTime(x.FechaHora) == hoy);
        }

        public Resultado GrabarResearchSituacionCultivo(ResearchSituacionCultivo researchSituacionCultivo, int comercialId)
        {
            var oEntityErrors = ValidarSituacionCultivo(researchSituacionCultivo);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            try
            {
                researchSituacionCultivo.ComercialId = comercialId;
                researchSituacionCultivo.FechaHora = DateTime.Now;
                repositorio.Agregar(researchSituacionCultivo);
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            if (!oEntityErrors.HayError)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se guardó correctamente"));
            }
            return oEntityErrors;
        }

        public Resultado EliminarResearchSituacionCultivo(int researchSituacionCultivoId)
        {
            var oEntityErrors = new Resultado();
            var tc = repositorio.Obtener<ResearchSituacionCultivo>(x => x.Id == researchSituacionCultivoId);
            try
            {
                repositorio.Remover(tc);
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            if (!oEntityErrors.HayError)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se eliminó correctamente"));
            }
            return oEntityErrors;
        }

        public List<ResearchVentaStockDto> TraerTodoResearchVentaStock()
        {
            var hoy = DateTime.Now.Date;
            return repositorio.Listar<ResearchVentaStock, ResearchVentaStockDto>(x => new ResearchVentaStockDto
            {
                Id = x.Id,
                Almacenado = x.Almacenado,
                VendidoAPrecio = x.VendidoAPrecio,
                LocalidadId = x.LocalidadId,
                Observaciones = x.Observaciones,
                MaterialId = x.MaterialId,
                FechaHora = x.FechaHora,
                Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                Material = x.Material.Descripcion + "",
                Localidad = x.Localidad.Nombre + "",
                Campania = x.Campania.Descripcion + "",
                CampaniaId = x.CampaniaId
            }, x => DbFunctions.TruncateTime(x.FechaHora) == hoy);
        }

        public Resultado GrabarResearchVentaStock(ResearchVentaStock researchVentaStock, int comercialId)
        {
            var oEntityErrors = ValidarVentaStock(researchVentaStock);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            try
            {
                researchVentaStock.ComercialId = comercialId;
                researchVentaStock.FechaHora = DateTime.Now;
                repositorio.Agregar(researchVentaStock);
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            if (!oEntityErrors.HayError)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se guardó correctamente"));
            }
            return oEntityErrors;
        }

        public Resultado EliminarResearchVentaStock(int researchVentaStockId)
        {
            var oEntityErrors = new Resultado();
            var tc = repositorio.Obtener<ResearchVentaStock>(x => x.Id == researchVentaStockId);
            try
            {
                repositorio.Remover(tc);
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            if (!oEntityErrors.HayError)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se eliminó correctamente"));
            }
            return oEntityErrors;
        }

        public KendoGrid<ResearchAvanceSiembraDto> TraerAvanceSiembra(KendoGridMvcRequest request)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerAvanceSiembra(request));
        }

        public KendoGrid<ResearchAvanceCosechaDto> TraerAvanceCosecha(KendoGridMvcRequest request)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerAvanceCosecha(request));
        }

        public KendoGrid<ResearchSituacionCultivoDto> TraerSituacionCultivoParcial(KendoGridMvcRequest request)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerSituacionCultivoParcial(request));
        }

        public KendoGrid<ResearchVentaStockDto> TraerVentaStock(KendoGridMvcRequest request)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerVentaStock(request));
        }

        public NotificacionResearchDto TraerNotificaciones(int id)
        {
            return repositorio.Obtener<NotificacionResearch, NotificacionResearchDto>(x => x.Id == id,
                x => new NotificacionResearchDto
                {
                    Id = x.Id,
                    CampanaId = x.CampanaId,
                    CampanaDescripcion = x.Campana.Descripcion,
                    MaterialId = x.MaterialId,
                    MaterialDescripcion = x.Material.Descripcion,
                    FechaDesde = SqlFunctions.DateName("day", x.FechaDesde).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaDesde.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaDesde),
                    FechaHasta = SqlFunctions.DateName("day", x.FechaHasta).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaHasta.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaHasta),
                    TipoResearchDescripcion = x.TipoResearch.Descripcion,
                    Mensaje = x.Mensaje
                });
        }

        public List<TipoResearch> TraerTipoResearch()
        {
            return repositorio.Listar<TipoResearch>();
        }

        public List<NotificacionResearchDto> TraerTodasNotificaciones()
        {
            return repositorio.Listar<NotificacionResearch, NotificacionResearchDto>(x => new NotificacionResearchDto
            {
                Id = x.Id,
                CampanaId = x.CampanaId,
                CampanaDescripcion = x.Campana.Descripcion,
                MaterialId = x.MaterialId,
                MaterialDescripcion = x.Material.Descripcion,
                TipoResearchId = x.TipoResearchId,
                TipoResearchDescripcion = x.TipoResearch.Descripcion,
                FechaDesde = SqlFunctions.DateName("day", x.FechaDesde).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaDesde.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaDesde),
                FechaHasta = SqlFunctions.DateName("day", x.FechaHasta).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaHasta.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaHasta),
                Mensaje = x.Mensaje
            });
        }

        public Resultado GrabarNotificacion(NotificacionResearch notificacion)
        {
            var error = Validar(notificacion);
            if (error.HayError)
            {
                return error;
            }
            try
            {
                if (notificacion.Id != 0)
                {
                    var notificacionSave = repositorio.Obtener<NotificacionResearch>(notificacion.Id);
                    notificacionSave.MaterialId = notificacion.MaterialId;
                    notificacionSave.CampanaId = notificacion.CampanaId;
                    notificacionSave.TipoResearchId = notificacion.TipoResearchId;
                    notificacionSave.FechaDesde = notificacion.FechaDesde;
                    notificacionSave.FechaHasta = notificacion.FechaHasta;
                    notificacionSave.Mensaje = notificacion.Mensaje;
                }
                else
                {
                    repositorio.Agregar(notificacion);
                }
                repositorio.GuardarCambios();
                return error;
            }
            catch (Exception e)
            {
                error.Error("", e.Message);
                return error;
            }
        }

        private Resultado Validar(NotificacionResearch notificacion)
        {
            var error = new Resultado();
            if (notificacion.MaterialId == 0)
            {
                error.Error("Material", "El Material no puede estar vacio");
            }
            if (notificacion.CampanaId == 0)
            {
                error.Error("Campana", "La Campaña no puede estar vacia");
            }
            if (notificacion.FechaDesde.Year == 1 || notificacion.FechaHasta.Year == 1)
            {
                error.Error("Fecha", "Las Fechas no pueden estar vacia");
            }
            if (notificacion.Mensaje == "" || notificacion.Mensaje == null)
            {
                error.Error("Mensaje", "El mensaje no puede estar vacio");
            }
            return error;
        }

        public Resultado EliminarNotificacion(int id)
        {
            var error = new Resultado();
            try
            {
                repositorio.Remover<NotificacionResearch>(id);
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                error.Error(ex.Source, ex.Message);
                throw;
            }
            if (!error.HayError)
            {
                error.Errores.Add(new ErrorMessage(200, "Se Eliminó Correctamente"));
            }
            return error;
        }

        public void SincronizarResearchPowerApp()
        {
            clienteResearchAgent.SincronizarDatosResearch();
        }

        public DataSourceResult BuscaDatosTabla(DataSourceRequest filtro)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerResearchPorFiltro(filtro));
        }

        public List<ResearchCondicion> TraerResearchCondicion()
        {
            return repositorio.Listar<ResearchCondicion>();
        }

        public List<ResearchEstadio> TraerResearchEstadio()
        {
            return repositorio.Listar<ResearchEstadio>();
        }

        public List<ResearchTipoCarga> TraerResearchTipoCarga()
        {
            return repositorio.Listar<ResearchTipoCarga>();
        }

        public List<ResearchTipoMuestra> TraerResearchTipoMuestra()
        {
            return repositorio.Listar<ResearchTipoMuestra>();
        }

        public List<ResearchHumedadSuelo> TraerResearchHumedadSuelo()
        {
            return repositorio.Listar<ResearchHumedadSuelo>();
        }

        public Resultado BorrarResearch(int id)
        {
            Resultado resultado = new Resultado();
            var registro = repositorio.Obtener<Research>(id);
            registro.Eliminado = true;
            repositorio.GuardarCambios();
            return resultado;
        }

        public List<int> TraerResearchId()
        {
            return repositorio.Listar<Research, int>(x => x.Id);
        }
    }
}
