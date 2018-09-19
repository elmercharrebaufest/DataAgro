using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
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
                var listaDeCuit = new List<Datos>();
                var usuario = repositorio.Listar<Comercial, string>(x => x.IdActiveDirectory, x => x.ComercialId == 10);
                var comerciales = repositorio.Listar<Comercial>();
                var estados = repositorio.Listar<Estado>();
                if (ConfigurationManager.AppSettings["usuarioLaura"].ToString() == "1")
                {
                    string aux = ConfigurationManager.AppSettings["SapPruebaUser"].ToString();
                    usuario.Add(aux);
                }

                var CUIT = repositorio.Listar<Proveedor, string>(x => x.CUIT);
                var list = new DatosProveedor(logger).ObtenerDatosDeProveedorEstado(CUIT, usuario);
                logger.Debug("ActualizarProveedores - Proveedores a actualziar: " + list.Count);
                if (list.Count > 0)
                {

                    Comercial comercial = null;
                    Estado Est = null;

                    foreach (var lista in list)
                    {
                        if (ConfigurationManager.AppSettings["usuarioLaura"].ToString() == "1")
                        {
                            string aux = ConfigurationManager.AppSettings["SapPruebaUser"].ToString();
                            if (lista.USUARIO.ToLower() == aux.ToLower())
                            {
                                string auxNombreActual = ConfigurationManager.AppSettings["usuarioLaurastring"].ToString();
                                comercial = comerciales.FirstOrDefault(x => x.IdActiveDirectory.ToLower() == auxNombreActual.ToLower());
                            }
                            else
                            {
                                comercial = comerciales.FirstOrDefault(x => x.IdActiveDirectory.ToLower() == lista.USUARIO.ToLower());
                            }
                        }
                        else
                        {
                            comercial = comerciales.FirstOrDefault(x => x.IdActiveDirectory.ToLower() == lista.USUARIO.ToLower());
                        }


                        if (comercial != null)
                        {
                            Est = estados.Where(x => x.Descripcion.ToLower() == lista.STATUS.ToLower()).FirstOrDefault();
                            var EstadoId = Est != null ? Est.EstadoId : 1;

                            var Actualizar = repositorio.SelStore<FakeClass>("DataAgro_ActualizarEstadoProveedor", 0, comercial.ComercialId, lista.CUIT,
                                (!String.IsNullOrEmpty(lista.CLIENTE_MOA) ? true : false), EstadoId);

                            var oResult = Actualizar.ToList();
                        }
                    }
                }
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

    public class FakeClass
    {
        public int id { get; set; }
    }
}
