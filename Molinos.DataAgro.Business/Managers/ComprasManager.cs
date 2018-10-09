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
            var oProveedor = repositorio.Listar<Proveedor,string>(x => x.CUIT,x=> x.FechaAlta >= ayer);
            var oComercial = repositorio.Listar<Comercial>();

            var listProve = new List<ComprasIniciales>();
            ComprasIniciales comp = null;
            logger.Debug("ProcessComprasAyer - Proveedores a actualziar: " + oProveedor.Count);
            if (oProveedor.Count > 0)
            {
                foreach (var comercial in oComercial)
                {
                    comp = new ComprasIniciales();
                    comp.CUIT.AddRange(oProveedor);
                    
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
            var oComercial = repositorio.Listar<Comercial>();

            List<ComprasIniciales> listProve = new List<ComprasIniciales>();
            ComprasIniciales comp = null;
            foreach (var comercial in oComercial)
            {
                comp = new ComprasIniciales();
                comp.CUIT.AddRange(oProveedor);                

                comp.UsuarioDirectory = comercial.IdActiveDirectory;
                listProve.Add(comp);
            }

            ActualizarComprasProveedorIniciales(listProve, oComercial);
        }

        private void ActualizarComprasProveedorIniciales(List<ComprasIniciales> listProve, List<Comercial> oComercial)
        {
            var SapCompras = new ComprasAgent();
            var histActual = repositorio.Listar<CampañaMaterialPorMes>();
            var campaniaMaterialActual = repositorio.Listar<CampañaMaterial>();
            var proveedores = repositorio.Listar(x => new { x.ProveedorId, x.CUIT }, (Proveedor x) => true).ToDictionary(x => x.CUIT.ToUpper().Trim());
            var campanias = repositorio.Listar(x => new { x.CampañaId, x.Descripcion }, (Campaña x) => true).ToDictionary(x => x.Descripcion.ToUpper().Trim());
            var materiales = repositorio.Listar(x => new { x.MaterialId, x.Codigo }, (Material x) => true).ToDictionary(x => x.Codigo.ToUpper().Trim());
            var campaniasMaterial = new List<CampañaMaterial>();
            var campaniaMaterialPorMes = new List<CampañaMaterialPorMes>();
            
            try
            {
                var contadorActualizacion = 0;
                foreach (var item in listProve)
                {
                    var hist = SapCompras.ComprarIniciales(item.CUIT, item.UsuarioDirectory);
                    logger.Debug("Campos a Acualizar para " + item.UsuarioDirectory + ": " + hist.Count);
                    if (hist.Count > 0)
                    {
                        var listHistorial = hist.GroupBy(x => new { x.VENDEDOR, x.MATERIAL, x.COSECHA });
                        var ComercialId = oComercial.FirstOrDefault(x => x.IdActiveDirectory.ToLower().Trim() == item.UsuarioDirectory.ToLower().Trim()).ComercialId;
                        foreach (var jj in listHistorial)
                        {
                            var proveedorId = proveedores[jj.Key.VENDEDOR].ProveedorId;
                            var materialId = materiales[jj.Key.MATERIAL].MaterialId;
                            var campaniaId = campanias[jj.Key.COSECHA].CampañaId;

                            var campaniaMaterial = campaniaMaterialActual.Where(x => x.CampañaId == campaniaId && x.ProveedorId == proveedorId && x.MaterialId == materialId).FirstOrDefault();
                            if (campaniaMaterial != null)
                            {
                                campaniaMaterial.ToneladasCompradas = jj.Sum(x => (double)x.TN_COMPRADAS);
                                contadorActualizacion++;
                                foreach (var ii in jj)
                                {

                                    var campaniaMaterialMes = histActual.Where(x => x.CampañaMaterialId == campaniaMaterial.CampañaMaterialId && x.Mes == Helper.DevolverIdMes(ii.MES) && x.Año == Int32.Parse(ii.ANIO) && x.ComercialId == ComercialId).FirstOrDefault();
                                    if (campaniaMaterialMes != null)
                                    {
                                        campaniaMaterialMes.Toneladas = (double)ii.TN_COMPRADAS;
                                    }
                                    else
                                    {
                                        campaniaMaterialPorMes.Add(new CampañaMaterialPorMes()
                                        {
                                            NroItem = 1,
                                            Mes = Helper.DevolverIdMes(ii.MES),
                                            Toneladas = (double)ii.TN_COMPRADAS,
                                            CampañaMaterialId = campaniaMaterial.CampañaMaterialId,
                                            Año = Int32.Parse(ii.ANIO),
                                            ComercialId = ComercialId
                                        });
                                    }
                                }

                            }
                            else
                            {
                                campaniasMaterial.Add(new CampañaMaterial() { CampañaId = campaniaId, NroItem = 1, ProveedorId = proveedorId, MaterialId = materialId, ToneladasCompradas = jj.Sum(x => (double)x.TN_COMPRADAS) });
                            }
                        }
                    }
                }
                logger.Debug("Campos a actualizados: " + contadorActualizacion);
                logger.Debug("Nuevos CampañaMaterial por mes: " + campaniaMaterialPorMes.Count);
                repositorio.AgregarTodos(campaniaMaterialPorMes);

                logger.Debug("Nuevos CampañaMaterial: " + campaniasMaterial.Count);
                repositorio.AgregarTodos(campaniasMaterial);
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
