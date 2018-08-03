using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent;
using Molinos.DataAgro.Agent.Compras;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Business
{
    public class ComprasManager : IComprasManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public ComprasManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        
        public void ActualizarComprasProveedor(List<Datos> listProve)
        {
            var sapCompras = new ComprasAgent();

            if (ConfigurationManager.AppSettings["SinConexionSap"] == "0")
            { 
                foreach (var item in listProve)
                {
                    try
                    {
                        List<ZMPES5130> hist;

                        if (ConfigurationManager.AppSettings["usuarioLaura"].ToString() == "1" && item.UsuarioDirectory.ToLower() == ConfigurationManager.AppSettings["usuarioLaurastring"].ToString().ToLower())
                        {
                            var aux = ConfigurationManager.AppSettings["SapPruebaUser"].ToString();
                            hist = sapCompras.Comprar(item.CUIT, aux);
                        }
                        else
                        {
                            hist = sapCompras.Comprar(item.CUIT, item.UsuarioDirectory);
                        }

                        if (hist.Count > 0)
                        {
                            foreach (var ii in hist)
                            {
                                try
                                {
                                    var campaniaMaterial = repositorio.Obtener<CampañaMaterial>(x => x.Campaña.Descripcion == ii.COSECHA && x.Proveedor.CUIT == item.CUIT && x.Material.Codigo == ii.MATERIAL);
                                    if(campaniaMaterial == null)
                                    {
                                        campaniaMaterial = repositorio.Agregar(new CampañaMaterial
                                        {
                                            Campaña = repositorio.Obtener<Campaña>(x => x.Descripcion == ii.COSECHA),
                                            Material= repositorio.Obtener<Material>(x => x.Codigo == ii.MATERIAL),
                                            Proveedor = repositorio.Obtener<Proveedor>(x => x.CUIT == item.CUIT),
                                        });
                                    }

                                    var mes = Helper.DevolverIdMes(ii.MES);
                                    var campaniaMaterialPorMes = repositorio.Obtener<CampañaMaterialPorMes>(x => x.CampañaMaterialId == campaniaMaterial.CampañaMaterialId && x.Mes == mes && x.Comercial.IdActiveDirectory == item.UsuarioDirectory);
                                    if(campaniaMaterialPorMes == null)
                                    {
                                        repositorio.Agregar(new CampañaMaterialPorMes
                                        {
                                            NroItem = 1,
                                            Mes = mes,
                                            Toneladas = (double)ii.TN_COMPRADAS
                                        });
                                    }
                                    campaniaMaterialPorMes.Toneladas = (double)ii.TN_COMPRADAS;
                                    repositorio.GuardarCambios();
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex);
                                    throw;
                                }
                            }

                            var listHistorial = hist.GroupBy(x => new { x.VENDEDOR, x.MATERIAL, x.COSECHA });

                            foreach (var jj in listHistorial)
                            {
                                try
                                {
                                    var total = repositorio.Sumar<CampañaMaterialPorMes>(x => x.Toneladas.HasValue ? (decimal)x.Toneladas.Value : 0, x => x.CampañaMaterial.Campaña.Descripcion == jj.Key.COSECHA && x.CampañaMaterial.Proveedor.CUIT == item.CUIT && x.CampañaMaterial.Material.Codigo == jj.Key.MATERIAL);
                                    var campaniaMaterial = repositorio.Obtener<CampañaMaterial>(x => x.Campaña.Descripcion == jj.Key.COSECHA && x.Proveedor.CUIT == item.CUIT && x.Material.Codigo == jj.Key.MATERIAL);
                                    campaniaMaterial.ToneladasCompradas = (double)total;
                                    repositorio.GuardarCambios();
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex);
                                    throw;
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
            }
        }
    }
}
