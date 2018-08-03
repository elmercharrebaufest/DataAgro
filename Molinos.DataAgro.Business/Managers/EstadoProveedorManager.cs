using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Molinos.DataAgro.Business.Managers
{
    public class EstadoProveedorManager : IEstadoProveedorManager
    {
        private readonly IRepositorio repositorio;
        private ILogger logger;

        public EstadoProveedorManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }


        public void ActualizarProveedores()
        {
            try
            {
                
                var listaDeCuit = new List<Datos>();
                
                var CUIT = repositorio.Listar<Proveedor,string>(x => x.CUIT);
                var usuario = repositorio.Listar<Comercial, string>(x => x.IdActiveDirectory, x => x.ComercialId == 10);

                if (ConfigurationManager.AppSettings["usuarioLaura"].ToString() == "1")
                {
                    string aux = ConfigurationManager.AppSettings["SapPruebaUser"].ToString();
                    usuario.Add(aux);

                }

                var list = new DatosProveedor(logger).ObtenerDatosDeProveedorEstado(CUIT, usuario);

                if (list.Count > 0)
                {
                    

                    foreach (var lista in list)
                    {
                        Comercial comercial = null;
                        if (ConfigurationManager.AppSettings["usuarioLaura"].ToString() == "1" && lista.USUARIO.ToLower() == ConfigurationManager.AppSettings["SapPruebaUser"].ToLower())
                        {
                            var auxNombreActual = ConfigurationManager.AppSettings["usuarioLaurastring"].ToString();
                            comercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == auxNombreActual);
                        }
                        else
                        {
                            comercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == lista.USUARIO.ToLower());
                        }
                        
                        if (comercial != null)
                        {
                            var est = repositorio.Obtener<Estado>(x => x.Descripcion == lista.STATUS) ?? repositorio.Obtener<Estado>(1);
                            var proveedor = repositorio.Obtener<Proveedor>(x => x.CUIT == lista.CUIT);
                            if (proveedor != null)
                            {
                                var proveedorEstado = repositorio.Obtener<ProveedorEstado>(x => x.Proveedor.CUIT == lista.CUIT && x.ComercialId == comercial.ComercialId);
                                if(proveedorEstado == null)
                                {
                                    proveedorEstado = repositorio.Agregar(new ProveedorEstado
                                    {
                                        Proveedor = proveedor,
                                        Comercial = comercial,
                                        Estado = est
                                    });
                                }
                                proveedor.ClienteMOA = !String.IsNullOrEmpty(lista.CLIENTE_MOA);
                            }
                        }
                    }
                }
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public void ActualizarProveedoresPorComercial(int comercialId, List<int> equipo)
        {
            try
            {
                var listaDeCuit = repositorio.Listar<ProveedorComercial, Datos>(x => 
                    new Datos()
                    {
                        CUIT = x.Proveedor.CUIT,
                        UsuarioDirectory = x.Comercial.IdActiveDirectory
                    }, x => equipo.Contains(x.ComercialId));

                var list = new DatosProveedor(logger).ObtenerDatosDeProveedor(listaDeCuit);

                if (list.Count > 0)
                {
                    var provEstados = repositorio.Listar<ProveedorEstado>(x => x.ComercialId == comercialId);
                    repositorio.RemoverTodos(provEstados);
                    foreach (var lista in list)
                    {
                        repositorio.Agregar(new ProveedorEstado
                        {
                            Comercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == lista.USUARIO),
                            Estado = repositorio.Obtener<Estado>(x => x.Descripcion == lista.STATUS) ?? repositorio.Obtener<Estado>(1),
                            Proveedor = repositorio.Obtener<Proveedor>(x => x.CUIT == lista.CUIT)
                        });                        
                    }
                    repositorio.GuardarCambios();
                }                
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

    }
}
