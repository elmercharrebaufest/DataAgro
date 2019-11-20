using Autofac.Extras.NLog;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.Managers
{
    public class CupoManager : ICupoManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly ICrearCupoAgent crearCupoAgent;
        private readonly IEliminarCupoAgent eliminarCupoAgent;
        private readonly IClienteStopAgent clienteStopAgent;
        private readonly IModificarCupoAgent modificarCupoAgent;

        public CupoManager(IRepositorio repositorio, ILogger logger, ICrearCupoAgent crearCupoAgent,
            IEliminarCupoAgent eliminarCupoAgent, IClienteStopAgent clienteStopAgent, IModificarCupoAgent modificarCupoAgent)
        {
            this.repositorio = repositorio;
            this.logger = logger;
            this.crearCupoAgent = crearCupoAgent;
            this.eliminarCupoAgent = eliminarCupoAgent;
            this.clienteStopAgent = clienteStopAgent;
            this.modificarCupoAgent = modificarCupoAgent;
        }
        public CupoResult GrabarCupo(Cupo cupo, int cantidadCupos, DateTime? fechaHasta)
        {
            var error = new CupoResult { ListaCupos = new List<string>()};
            try
            {
                cupo.Proveedor = repositorio.Obtener<Proveedor>(cupo.ProveedorId);
                cupo.Comercial = repositorio.Obtener<Comercial>(cupo.ComercialId);
                cupo.Material = repositorio.Obtener<Material>(cupo.MaterialId);
                cupo.Centro = repositorio.Obtener<Centro>(cupo.CentroId);
                cupo.ZonaCupo = repositorio.Obtener<ZonaCupo>(cupo.ZonaCupoId);
                if (cupo.Id == 0)
                {
                    fechaHasta = fechaHasta ?? cupo.FechaIngreso;
                    while (cupo.FechaIngreso <= fechaHasta)
                    {
                        var listaCupos = new List<string>();
                        var errorSap = new Resultado();
                        try
                        {
                            listaCupos = crearCupoAgent.Crear(cupo, cantidadCupos);
                        }
                        catch(Exception e)
                        {
                            errorSap.Error("CantidadCuposSAP", cupo.FechaIngreso.ToShortDateString() + ": " + e.Message);
                        }
                        if (errorSap.HayError)
                        {
                            error.Errores.AddRange(errorSap.Errores);
                            cupo.FechaIngreso = cupo.FechaIngreso.AddDays(1);
                            continue;
                        }
                        var cuposConSap = new List<Cupo>();

                        cupo.EstadoCupoId = cupo.Centro.CodigoSap == "1600" || cupo.Centro.CodigoSap == "1029" ? 6 : 8;
                        foreach (var cupoSap in listaCupos)
                        {
                            var nuevoCupo = (Cupo)cupo.Clone();
                            nuevoCupo.CupoSap = cupoSap;
                            cuposConSap.Add(nuevoCupo);
                        }
                        repositorio.AgregarTodos(cuposConSap);
                        repositorio.GuardarCambios();
                        
                        if (listaCupos.Count < cantidadCupos)
                        {
                            error.Error("CantidadCuposSAP", "Se generaron " + listaCupos.Count + " de " + cantidadCupos + " cupos solicitados para el dia " + cupo.FechaIngreso.ToShortDateString());
                        }
                        error.ListaCupos.AddRange(listaCupos);
                        cupo.FechaIngreso = cupo.FechaIngreso.AddDays(1);
                    }
                    return error;
                }
                else
                {
                    var datosConfiguracion = repositorio.Obtener<Configuracion>(1);
                    var cupoSave = repositorio.Obtener<Cupo>(cupo.Id);
                    cupoSave.ProveedorId = cupo.ProveedorId;
                    cupoSave.Proveedor = cupo.Proveedor;
                    cupoSave.Calidad = cupo.Calidad;
                    cupoSave.Observaciones = cupo.Observaciones;
                    cupoSave.Destinatario = cupo.Destinatario;
                    cupoSave.Fason = cupo.Fason;
                    cupoSave.FleteProcedencia = cupo.FleteProcedencia;
                    var res = modificarCupoAgent.Modificar(cupoSave);
                    if (res != "Ok")
                    {
                        error.Error("SAP", $"Error al grabar en SAP: {res}");
                    }
                    if (!cupoSave.Centro.Acopio)
                    {
                        if (datosConfiguracion.ConexionABMStop.HasValue && datosConfiguracion.ConexionABMStop.Value)
                        {
                            if (cupoSave.CupoStop != null)
                            {
                                clienteStopAgent.ModificarCupo(cupoSave);
                            }
                        }
                        else
                        {
                            error.Error("Stop", "Sin Conexión a Stop. Modificado en SAP");
                        }
                    }
                    repositorio.GuardarCambios();
                    return error;
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
                error.Errores.Add(new ErrorMessage(400, e.Message));
                return error;
            }
        }
        public Resultado Validar(Cupo cupo,int cantidadCupos, DateTime? fechaHasta)
        {
            var error = new Resultado();
            if (cupo.ProveedorId == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El Proveedor no debe estar vacío"));
            }
            if (cupo.ProveedorId != 0)
            {
                var cuit = repositorio.Obtener<Proveedor, string>(y => y.ProveedorId == cupo.ProveedorId, y => y.CUIT);
                var sisa = repositorio.Obtener<SISA>(x => x.CUIT == cuit);
                if ((sisa != null && (sisa.EstadoCuit == 0 || sisa.EstadoCuit == 3)) || sisa == null)
                {
                    error.Errores.Add(new ErrorMessage(400, "Proveedor con CUIT en estado No Operable"));
                }
            }    
            if(cupo.Id == 0 && cantidadCupos == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "La Cantidad no debe estar vacía"));
            }
            if (cupo.MaterialId == 3 && (cupo.Calidad == ""|| cupo.Calidad == null))
            {
                error.Errores.Add(new ErrorMessage(400, "La calidad no debe estar vacia para Soja"));
            }
            if (cupo.Fason == true && (cupo.Destinatario == ""|| cupo.Destinatario == null))
            {
                error.Errores.Add(new ErrorMessage(400, "CUIT Destinatario no debe estar vacío cuando elige Fasón/Préstamo Devolución"));
            }
            if(fechaHasta.HasValue && fechaHasta< cupo.FechaIngreso)
            {
                error.Errores.Add(new ErrorMessage(400, "La Fecha Hasta de entrega no puede ser menor a la Fecha Desde"));
            }

            return error;
        }
        public KendoGrid<CupoDto> TraerCuposTabla(KendoGridMvcRequest request, List<int> equipo)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerTodosCupos(request, equipo));
        }
        public Resultado EliminarCupo(int id, string comercial)
        {
            try
            {
                var nuevoResultado = new Resultado();
                
                var cupoSap = repositorio.Obtener<Cupo>(id);
                var datosConfiguracion = repositorio.Obtener<Configuracion>(1);
                var resultado = eliminarCupoAgent.Eliminar(cupoSap.CupoSap, comercial);
                if (resultado == "OK")
                {
                    cupoSap.EstadoCupoId = 4;
                    repositorio.GuardarCambios();
                }
                else
                {
                    nuevoResultado.Error("", $"Error al anular cupo en SAP: {resultado}"); ;
                }
                if (!cupoSap.Centro.Acopio)
                {
                    if (cupoSap.EstadoCupoId == 4 && cupoSap.CupoStop!= null)
                    {
                        if (datosConfiguracion.ConexionABMStop.HasValue && !datosConfiguracion.ConexionABMStop.Value)
                        {
                            nuevoResultado.Error("Stop", "Error al anular cupo en STOP: Sin conexión a STOP. Anulado en SAP Correctamente");
                            return nuevoResultado;
                        }
                        var resultadoStop = clienteStopAgent.EliminarCupo(cupoSap);
                        if (nuevoResultado.HayError)
                        {
                            foreach (var e in resultadoStop.Errores)
                            {
                                nuevoResultado.Error("", $"Error al anular cupo en STOP: {e.Message}. Anulado en SAP Correctamente"); ;
                            }
                            return nuevoResultado;
                        }
                    }
                }
                return nuevoResultado;
            }
            catch (Exception e)
            {
                var nuevoResultado = new Resultado();
                nuevoResultado.Error("eliminar",e.Message);
                return nuevoResultado;
            }
        }
        
        public Resultado TransmitirCupos(List<string> cupos)
        {
            var result = new Resultado();
            var datosConfiguracion = repositorio.Obtener<Configuracion>(1);
            if(datosConfiguracion.ConexionABMStop.HasValue&& !datosConfiguracion.ConexionABMStop.Value)
            {
                result.Error("Stop", "Sin conexión a STOP");
                return result;
            }
            try
            {
                clienteStopAgent.CrearCupo(cupos);
            }
            catch (Exception e)
            {
                result.Error("", e.Message);
            }
            return result;
        }
        public void TransmitirCupos()
        {
            clienteStopAgent.TransmitirJobCupos();
        }
        public List<RespuestaCupoStop> ConsultarCuposDiarios()
        {
            return clienteStopAgent.ConsultarCuposDiarios();
        }
        public CupoDto ObtenerCupo(int id)
        {
            return repositorio.Obtener<Cupo, CupoDto>(x => x.Id == id, x=> new CupoDto
            { 
                Id= x.Id,
                Calidad = x.Calidad,
                Centro = x.Centro.Descripcion,
                CentroId = x.CentroId,
                ComercialId = x.ComercialId,
                Destinatario = x.Destinatario,
                Fason = x.Fason,
                FleteProcedencia = x.FleteProcedencia,
                FechaIngreso = x.FechaIngreso,
                MaterialId = x.MaterialId,
                Material = x.Material.Descripcion,
                Observaciones = x.Observaciones,
                ProveedorId = x.ProveedorId,
                Proveedor = x.Proveedor.RazonSocial+ " (" + x.Proveedor.CUIT+ ")",
                ZonaCupoId = x.ZonaCupoId,
                ZonaCupo = x.ZonaCupo.Descripcion
            });
        }
    }
}
