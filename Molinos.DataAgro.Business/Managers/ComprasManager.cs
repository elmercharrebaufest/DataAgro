using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace Molinos.DataAgro.Business.Managers
{
    public class ComprasManager : IComprasManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IComprasAgent oComprasAgent;
        private readonly IComprasDetalleAgent comprasDetalleAgent;

        public ComprasManager(ILogger logger, IRepositorio repositorio, IComprasAgent oComprasAgent, IComprasDetalleAgent comprasDetalleAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.oComprasAgent = oComprasAgent;
            this.comprasDetalleAgent = comprasDetalleAgent;
        }

        public void ActualizarComprasAyer()
        {
            var a = new List<string>();
            var ayer = DateTime.Now.AddDays(-1);
            var oProveedor = repositorio.Listar<Proveedor, string>(x => x.CUIT, x => x.FechaAlta >= ayer).Distinct().ToList();
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
            ActualizarComprasProveedorIniciales(listProve, oComercial);// fix para que grabe los CampañaMaterialPorMes
        }

        private void ActualizarComprasProveedorIniciales(List<ComprasIniciales> listProve, List<Comercial> oComercial)
        {
            var histActual = repositorio.Listar<CampañaMaterialPorMes>();
            var campaniaMaterialActual = repositorio.Listar<CampañaMaterial>();
            var proveedores = repositorio.Listar<Proveedor, ProveedorBasicoDto>(x => new ProveedorBasicoDto { ProveedorId = x.ProveedorId, CUIT = x.CUIT }).GroupBy(x => x.CUIT.ToUpper().Trim()).ToDictionary(x => x.Key);
            var campanias = repositorio.Listar<Campaña, CampañaDto>(x => new CampañaDto { CampañaId = x.CampañaId, Descripcion = x.Descripcion }).ToDictionary(x => x.Descripcion.ToUpper().Trim());
            var materiales = repositorio.Listar<Material, MaterialBasicoDto>(x => new MaterialBasicoDto { MaterialId = x.MaterialId, Codigo = x.Codigo }).ToDictionary(x => x.Codigo.ToUpper().Trim());
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

                            foreach (var proveedor in proveedoresId)
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

        public void ActualizarComprasDetalle(string comercialUsurarioAD, string cuit)
        {
            var a = new List<string>();
            
            var oProveedor = repositorio.Listar<Proveedor, string>(x => x.CUIT).Distinct();
            if (!string.IsNullOrEmpty(cuit))
            {
                oProveedor = oProveedor.Where(x => x == cuit);
            }
            var oComercial = repositorio.Listar<Comercial>();

            List<ComprasIniciales> listProve = new List<ComprasIniciales>();
            ComprasIniciales comp = null;
            if (!string.IsNullOrEmpty(comercialUsurarioAD))
            {
                oComercial = oComercial.Where(x => x.IdActiveDirectory == comercialUsurarioAD).ToList();
            }
            foreach (var comercial in oComercial)
            {
                comp = new ComprasIniciales();
                comp.CUIT.AddRange(oProveedor);

                comp.UsuarioDirectory = comercial.IdActiveDirectory;
                listProve.Add(comp);
            }


            ActualizarComprasDetalleProveedorIniciales(listProve, oComercial);
            //ActualizarComprasDetalleProveedorIniciales(listProve, oComercial);// fix para que grabe los CampañaMaterialPorMes        
        }
        private void ActualizarComprasDetalleProveedorIniciales(List<ComprasIniciales> listProve, List<Comercial> oComercial)
        {
            var histActual = repositorio.Listar<CampanaMaterialDetallePorMes>();
            var campaniaMaterialActual = repositorio.Listar<CampanaMaterialDetalle>();
            var proveedores = repositorio.Listar<Proveedor, ProveedorBasicoDto>(x => new ProveedorBasicoDto { ProveedorId = x.ProveedorId, CUIT = x.CUIT }).GroupBy(x => x.CUIT.ToUpper().Trim()).ToDictionary(x => x.Key);
            var campanias = repositorio.Listar<Campaña, CampañaDto>(x => new CampañaDto { CampañaId = x.CampañaId, Descripcion = x.Descripcion }).ToDictionary(x => x.Descripcion.ToUpper().Trim());
            var materiales = repositorio.Listar<Material, MaterialBasicoDto>(x => new MaterialBasicoDto { MaterialId = x.MaterialId, Codigo = x.Codigo }).ToDictionary(x => x.Codigo.ToUpper().Trim());
            var campaniasMaterial = new List<CampanaMaterialDetalle>();
            var campaniaMaterialPorMes = new List<CampanaMaterialDetallePorMes>();

            try
            {
                var contadorActualizacion = 0;
                foreach (var item in listProve)
                {
                    var hist = comprasDetalleAgent.ComprarIniciales(item.CUIT, item.UsuarioDirectory);
                    logger.Debug("Campos a Acualizar para " + item.UsuarioDirectory + "-" + item.CUIT.Count + ": " + hist.Count);
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    if (hist.Count > 0)
                    {
                        var listHistorial = hist.GroupBy(x => new { x.VENDEDOR, x.MATERIAL, x.COSECHA });
                        var ComercialId = oComercial.FirstOrDefault(x => x.IdActiveDirectory.ToLower().Trim() == item.UsuarioDirectory.ToLower().Trim()).ComercialId;
                        foreach (var jj in listHistorial)
                        {
                            var proveedoresId = proveedores[jj.Key.VENDEDOR];
                            var materialId = materiales[jj.Key.MATERIAL].MaterialId;
                            var campaniaId = campanias[jj.Key.COSECHA].CampañaId;

                            foreach (var proveedor in proveedoresId)
                            {
                                int proveedorId = proveedor.ProveedorId;
                                var campaniaMaterial = campaniaMaterialActual.Where(x => x.CampanaId == campaniaId && x.ProveedorId == proveedorId && x.MaterialId == materialId).FirstOrDefault();
                                if (campaniaMaterial != null)
                                {
                                    //campaniaMaterial.ToneladasCompradas = jj.Sum(x => (double)x.TN_COMPRADAS);
                                    contadorActualizacion++;
                                    foreach (var ii in jj)
                                    {
                                        var fechaDetalle = DateTime.ParseExact(ii.FECHA, "yyyy-MM-dd", provider);
                                        var campaniaMaterialMes = histActual.Where(x => x.CampanaMaterialDetalleId == campaniaMaterial.Id && x.Fecha.Month == fechaDetalle.Month && x.Fecha.Year == fechaDetalle.Year && x.ComercialId == ComercialId).FirstOrDefault();
                                        if (campaniaMaterialMes != null)
                                        {
                                            campaniaMaterialMes.PendienteAFijar = (double)ii.PEND_FIJAR;
                                            campaniaMaterialMes.PendienteAplicar = (double)ii.PEND_APLICAR;
                                            campaniaMaterialMes.ToneladaAmpliada = (double)ii.TN_AMPLIADAS;
                                            campaniaMaterialMes.ToneladaAnulada = (double)ii.TN_ANULADAS;
                                            campaniaMaterialMes.ToneladaAplicada = (double)ii.TN_APLICADAS;
                                            campaniaMaterialMes.ToneladaContrato = (double)ii.TN_CONTRATO;
                                            campaniaMaterialMes.ToneladaFijada = (double)ii.TN_FIJADAS;
                                            campaniaMaterialMes.ClaseDoc = ii.CLASE_DOC;
                                            campaniaMaterialMes.Clasificacion = ii.CLASIFICACION;
                                        }
                                        else
                                        {
                                            campaniaMaterialPorMes.Add(new CampanaMaterialDetallePorMes()
                                            {
                                                Fecha = fechaDetalle,
                                                PendienteAFijar = (double)ii.PEND_FIJAR,
                                                PendienteAplicar = (double)ii.PEND_APLICAR,
                                                ToneladaAmpliada = (double)ii.TN_AMPLIADAS,
                                                ToneladaAnulada = (double)ii.TN_ANULADAS,
                                                ToneladaAplicada = (double)ii.TN_APLICADAS,
                                                ToneladaContrato = (double)ii.TN_CONTRATO,
                                                ToneladaFijada = (double)ii.TN_FIJADAS,
                                                ClaseDoc = ii.CLASE_DOC,
                                                Clasificacion = ii.CLASIFICACION,
                                                ComercialId = ComercialId,
                                                CampanaMaterialDetalleId = campaniaMaterial.Id

                                            });
                                        }
                                    }

                                }
                                else
                                {
                                    campaniasMaterial.Add(
                                        new CampanaMaterialDetalle()
                                        {
                                            CampanaId = campaniaId,
                                            ProveedorId = proveedorId,
                                            MaterialId = materialId
                                        });
                                }
                            }
                        }
                    }
                }
                logger.Debug("Campos a actualizados compra detalle: " + contadorActualizacion);
                logger.Debug("Nuevos CampanaMaterialDetalle por mes: " + campaniaMaterialPorMes.Count);
                repositorio.AgregarTodos(campaniaMaterialPorMes);

                logger.Debug("Nuevos CampanaMaterialDetalle: " + campaniasMaterial.Count);
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
