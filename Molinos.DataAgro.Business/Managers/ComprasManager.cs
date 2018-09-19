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
    public class ComprasManager : IComprasManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public ComprasManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public void ActualizarComprasAyer()
        {
            var a = new List<string>();
            var ayer = DateTime.Now.AddDays(-1);
            var oProveedor = repositorio.Listar<Proveedor>(x => x.FechaAlta >= ayer);
            var oComercial = repositorio.Listar<Comercial>();

            var listProve = new List<ComprasIniciales>();
            ComprasIniciales comp = null;
            logger.Debug("ProcessComprasAyer - Proveedores a actualziar: " + oProveedor.Count);
            if (oProveedor.Count > 0)
            {
                foreach (var comercial in oComercial)
                {
                    comp = new ComprasIniciales();
                    foreach (var proveedor in oProveedor)
                    {
                        comp.CUIT.Add(proveedor.CUIT);
                    }

                    comp.UsuarioDirectory = comercial.IdActiveDirectory;
                    listProve.Add(comp);
                }

                ActualizarComprasProveedorIniciales(listProve, oComercial);
            }
        }
        
        public void ActualizarCompras()
        {
            var a = new List<string>();

            var oProveedor = repositorio.Listar<Proveedor, string>(x => x.CUIT);
            var oComercial = repositorio.Listar<Comercial>(x => x.ComercialId == 4);

            List<ComprasIniciales> listProve = new List<ComprasIniciales>();
            ComprasIniciales comp = null;
            foreach (var comercial in oComercial)
            {
                comp = new ComprasIniciales();
                foreach (var proveedor in oProveedor)
                {
                    comp.CUIT.Add(proveedor);
                }

                comp.UsuarioDirectory = comercial.IdActiveDirectory;
                listProve.Add(comp);
            }

            ActualizarComprasProveedorIniciales(listProve, oComercial);
        }
        
        private void ActualizarComprasProveedorIniciales(List<ComprasIniciales> listProve, List<Comercial> oComercial)
        {
            var SapCompras = new ComprasAgent();

            if (ConfigurationManager.AppSettings["SinConexionSap"] == "0")
            {
                foreach (var item in listProve)
                {
                    try
                    {
                        List<Agent.Compras.ZMPES5130> hist = new List<Agent.Compras.ZMPES5130>();

                        hist = SapCompras.ComprarIniciales(item.CUIT, item.UsuarioDirectory);

                        if (hist.Count > 0)
                        {
                            foreach (var ii in hist)
                            {
                                var ComercialId = oComercial.FirstOrDefault(x => x.IdActiveDirectory.ToLower().Trim() == item.UsuarioDirectory.ToLower().Trim()).ComercialId;
                                try
                                {
                                    var aa = repositorio.SelStore<FakeValor>("DataAgro_CampañaMaterialPorMes_Actualizar", 0, ii.ANIO, ii.COSECHA, ii.MATERIAL, ii.TN_COMPRADAS, Helper.DevolverIdMes(ii.MES), ii.VENDEDOR, ComercialId).ToList();
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
                                    var aa = repositorio.SelStore<FakeValor>("DataAgro_CampañaMaterialPorMes_ToneladasActualizar", 0, jj.Key.COSECHA, jj.Key.MATERIAL, jj.Key.VENDEDOR).ToList();
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

    public class FakeValor
    {
        public int? Error { get; set; }
    }
}
