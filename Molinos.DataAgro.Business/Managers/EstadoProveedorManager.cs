using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent;
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
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public EstadoProveedorManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        
        public void ActualizarProveedores()
        {
            try
            {                
                var comerciales = repositorio.Listar<Comercial>().ToDictionary(x => x.IdActiveDirectory.ToUpper().Trim());
                var estados = repositorio.Listar<Estado>().ToDictionary(x => x.Descripcion.ToLower());
                var proveedores = repositorio.Listar<Proveedor>().ToDictionary(x => x.CUIT.ToUpper().Trim());
                logger.Debug("Obteniendo datos de SAP");
                var list = new DatosProveedor(logger).ObtenerDatosDeProveedorEstado(proveedores.Keys.ToList(), comerciales.Keys.ToList());
                var crearEstadoProvedor = new List<ProveedorEstado>();

                logger.Debug("Resultado: " + list.Count);
                var proveedoresCuit = list.Select(x => x.CUIT.ToUpper().Trim()).Distinct().ToList();
                var comercialesAd = list.Select(x => x.USUARIO.ToUpper().Trim()).Distinct().ToList();
                var proveedoresEstado = repositorio.Listar<ProveedorEstado>(x => proveedoresCuit.Contains(x.Proveedor.CUIT.ToUpper().Trim()) && comercialesAd.Contains(x.Comercial.IdActiveDirectory.ToUpper().Trim()));

                foreach (var estado in list)
                {
                    if(!proveedores.ContainsKey(estado.CUIT.ToUpper()))
                    {
                        logger.Debug("El proveedor " + proveedores[estado.CUIT.ToUpper()] + " no existe en la base");
                    }
                    var proveedor = proveedores[estado.CUIT.ToUpper()];
                    var comercial = comerciales[estado.USUARIO.ToUpper()];
                    
                    var proveedorEstado = proveedoresEstado.FirstOrDefault(x => x.ComercialId == comercial.ComercialId && x.ProveedorId == proveedor.ProveedorId);
                    if(proveedorEstado != null)
                    {
                        proveedorEstado.EstadoId = estados[estado.STATUS.ToLower()].EstadoId;
                    }
                    else
                    {
                        crearEstadoProvedor.Add(new ProveedorEstado
                        {
                            ComercialId = comercial.ComercialId,
                            EstadoId = estados[estado.STATUS.ToLower()].EstadoId,
                            ProveedorId = proveedor.ProveedorId
                        });
                    }
                    proveedor.ClienteMOA = !string.IsNullOrEmpty(estado.CLIENTE_MOA) ? true : false;
                }
                logger.Debug("ActualizarProveedores - Proveedores a actualizar: " + crearEstadoProvedor.Count);
                if (crearEstadoProvedor.Count > 0)
                {
                    repositorio.AgregarTodos(crearEstadoProvedor);
                }
                logger.Debug("Actualizar Clientes MOA:" + list.Count);
                
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }            
        }

        private Proveedor TraerComercial(string CUIT)
        {
            return repositorio.Obtener<Proveedor>(x => x.CUIT == CUIT) ?? new Proveedor();
        }

    }
}
