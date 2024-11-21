using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Business.Managers
{
    public class EstadoProveedorManager : IEstadoProveedorManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IDatosProveedorAgent oDatosProveedorAgent;

        public EstadoProveedorManager(ILogger logger, IRepositorio repositorio, IDatosProveedorAgent oDatosProveedorAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.oDatosProveedorAgent = oDatosProveedorAgent;
        }

        public void ActualizarProveedores(string cuit)
        {
            try
            {
                var comerciales = repositorio.Listar<Comercial>().ToDictionary(x => x.IdActiveDirectory.ToUpper().Trim());
                var estados = repositorio.Listar<Estado>().ToDictionary(x => x.Descripcion.ToLower());
                var proveedores = repositorio.Listar<Proveedor>().GroupBy(x => x.CUIT.Trim()).ToDictionary(x => x.Key); //puede haber más de un registro con el mismo CUIT
                var proveedorEstados = repositorio.Listar<ProveedorEstado>().ToDictionary(x => (x.ComercialId, x.ProveedorId));
                var crearEstadoProveedor = new List<ProveedorEstado>();

                List<DatosProveedorAgentDto> datosDeProveedores = new List<DatosProveedorAgentDto>();

                if (string.IsNullOrEmpty(cuit))
                {
                    var listaComerciales = comerciales.Keys.ToList();
                    int tamanioLote = 40; //se seleccionan algunos porque si se consulta la RFC con todos, da server error

                    for (int i = 0; i < listaComerciales.Count; i += tamanioLote)
                    {
                        var loteComerciales = listaComerciales.Skip(i).Take(tamanioLote).ToList();

                        datosDeProveedores.AddRange(oDatosProveedorAgent.ObtenerDatosDeProveedorEstado(proveedores.Keys.ToList(), loteComerciales));
                    }
                }
                else
                {
                    datosDeProveedores = oDatosProveedorAgent.ObtenerDatosDeProveedorEstado(new List<string> { cuit.Trim() }, comerciales.Keys.ToList());
                }

                //se agrupan los resultados por CUIT y en cada grupo se conserva un solo usuario, ya que algunos pueden venir repetidos y eso generaría repeticiones en la tabla ProveedorEstado
                var datosAgrupados = datosDeProveedores.GroupBy(x => x.CUIT.Trim()).ToDictionary(d => d.Key, d => d.GroupBy(g => g.USUARIO.Trim()).Select(grp => grp.First()).ToList());
                logger.Debug($"Datos de {datosAgrupados.Count} proveedores obtenidos.");

                foreach (var cuitProveedor in datosAgrupados.Keys)
                {
                    var datos = datosAgrupados[cuitProveedor];

                    if (!proveedores.ContainsKey(cuitProveedor))
                    {
                        logger.Debug($"El proveedor con CUIT {cuitProveedor} no existe en la base.");
                        continue;
                    }

                    var proveedoresAgrupados = proveedores[cuitProveedor];

                    foreach (var estado in datos)
                    {
                        if (!comerciales.ContainsKey(estado.USUARIO.ToUpper().Trim()))
                        {
                            logger.Debug($"El usuario {estado.USUARIO} no existe en la base.");
                            continue;
                        }

                        var comercial = comerciales[estado.USUARIO.ToUpper().Trim()];

                        foreach (var proveedor in proveedoresAgrupados)
                        {
                            var clave = (comercial.ComercialId, proveedor.ProveedorId);

                            if (proveedorEstados.TryGetValue(clave, out var proveedorEstado))
                            {
                                proveedorEstado.EstadoId = estados[estado.STATUS.ToLower()].EstadoId;
                            }
                            else
                            {
                                if (!crearEstadoProveedor.Any(x => x.ComercialId == comercial.ComercialId && x.ProveedorId == proveedor.ProveedorId && x.EstadoId == estados[estado.STATUS.ToLower()].EstadoId))
                                {
                                    logger.Debug($"Creando nuevo estado para el proveedor {cuitProveedor} y ComercialId {comercial.ComercialId}");
                                    crearEstadoProveedor.Add(new ProveedorEstado
                                    {
                                        ComercialId = comercial.ComercialId,
                                        EstadoId = estados[estado.STATUS.ToLower()].EstadoId,
                                        ProveedorId = proveedor.ProveedorId
                                    });
                                }
                            }

                            proveedor.ClienteMOA = !string.IsNullOrEmpty(estado.CLIENTE_MOA);
                        }
                    }
                }

                if (crearEstadoProveedor.Any())
                {
                    repositorio.AgregarTodos(crearEstadoProveedor);
                }
                repositorio.GuardarCambios();

                logger.Debug($"ActualizarProveedores completado: {crearEstadoProveedor.Count} nuevos estados creados.");
            }
            catch (Exception ex)
            {
                logger.Error("Error en ActualizarProveedores", ex);
                throw;
            }
        }
    }
}
