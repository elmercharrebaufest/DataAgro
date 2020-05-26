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
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IComprasAgent oComprasAgent;

        public ComprasManager(ILogger logger, IRepositorio repositorio, IComprasAgent oComprasAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.oComprasAgent = oComprasAgent;
        }

        public void ActualizarComprasAyer()
        {
            var a = new List<string>();
            var ayer = DateTime.Now.AddDays(-1);
            var oProveedor = repositorio.Listar<Proveedor,string>(x => x.CUIT,x=> x.FechaAlta >= ayer).Distinct().ToList();
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
            repositorio.RemoverTodos<CampañaMaterial>(x=>true);
            repositorio.RemoverTodos<CampañaMaterialPorMes>(x=>true);
            repositorio.GuardarCambios();
            var a = new List<string>();

            var oProveedor = repositorio.Listar<Proveedor, string>(x => x.CUIT).Distinct();
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
            var histActual = repositorio.Listar<CampañaMaterialPorMes>();
            var campaniaMaterialActual = repositorio.Listar<CampañaMaterial>();
            var proveedores = repositorio.Listar<Proveedor, ProveedorBasicoDto>(x => new ProveedorBasicoDto {ProveedorId= x.ProveedorId,CUIT= x.CUIT }).GroupBy(x => x.CUIT.ToUpper().Trim()).ToDictionary(x => x.Key);
            var campanias = repositorio.Listar<Campaña,CampañaDto>(x => new CampañaDto { CampañaId=x.CampañaId,Descripcion=x.Descripcion }).ToDictionary(x => x.Descripcion.ToUpper().Trim());
            var materiales = repositorio.Listar<Material,MaterialBasicoDto>(x => new MaterialBasicoDto { MaterialId= x.MaterialId,Codigo= x.Codigo }).ToDictionary(x => x.Codigo.ToUpper().Trim());
            var campaniasMaterial = new List<CampañaMaterial>();
            var campaniaMaterialPorMes = new List<CampañaMaterialPorMes>();
            
            try
            {
                var contadorActualizacion = 0;
                foreach (var item in listProve)
                {
                    var hist = oComprasAgent.ComprarIniciales(item.CUIT, item.UsuarioDirectory);
                    logger.Debug("Campos a Acualizar para " + item.UsuarioDirectory + "-" + item.CUIT.Count + ": " + hist.Count);
                    if (hist.Count > 0)
                    {
                        var listHistorial = hist.GroupBy(x => new { x.VENDEDOR, x.MATERIAL, x.COSECHA });
                        var ComercialId = oComercial.FirstOrDefault(x => x.IdActiveDirectory.ToLower().Trim() == item.UsuarioDirectory.ToLower().Trim()).ComercialId;
                        foreach (var jj in listHistorial)
                        {
                            var proveedoresId = proveedores[jj.Key.VENDEDOR];
                            var materialId = materiales[jj.Key.MATERIAL].MaterialId;
                            var campaniaId = campanias[jj.Key.COSECHA].CampañaId;

                            foreach(var proveedor in proveedoresId)
                            {
                                int proveedorId = proveedor.ProveedorId;
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
                logger.Error("ProcessCompras ERROR");
                logger.Error(ex);
                throw;
            }
        }
    }    
}
