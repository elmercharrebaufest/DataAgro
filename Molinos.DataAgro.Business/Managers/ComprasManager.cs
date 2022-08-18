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
                                    campaniasMaterial.Add(new CampañaMaterial()
                                    {
                                        CampañaId = campaniaId,
                                        NroItem = 1,
                                        ProveedorId = proveedorId,
                                        MaterialId = materialId,
                                        ToneladasCompradas = jj.Sum(x => (double)x.TN_COMPRADAS)
                                    });
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
                oComercial = oComercial.Where(x => x.IdActiveDirectory == comercialUsurarioAD).Take(5).ToList();
            }
            foreach (var comercial in oComercial)
            {
                //var oProveedorComercial = repositorio.Listar<ProveedorComercial, string>(x => x.Proveedor.CUIT, x => x.ComercialId == comercial.ComercialId).Distinct();
                //if (!string.IsNullOrEmpty(cuit))
                //{
                //    oProveedorComercial = oProveedorComercial.Where(x => x == cuit);
                //}
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
            var proveedores = repositorio.Listar<Proveedor, ProveedorBasicoDto>(x => new ProveedorBasicoDto { ProveedorId = x.ProveedorId, CUIT = x.CUIT }).GroupBy(x => x.CUIT.ToUpper().Trim()).ToDictionary(x => x.Key);
            var corredores = repositorio.Listar<Proveedor, ProveedorBasicoDto>(x => new ProveedorBasicoDto { ProveedorId = x.ProveedorId, CUIT = x.CUIT },x=>x.SegmentacionId== 5 || x.SegmentacionId==7).ToList();
            var campanias = repositorio.Listar<Campaña, CampañaDto>(x => new CampañaDto { CampañaId = x.CampañaId, Descripcion = x.Descripcion }).ToDictionary(x => x.Descripcion.ToUpper().Trim());
            var materiales = repositorio.Listar<Material, MaterialBasicoDto>(x => new MaterialBasicoDto { MaterialId = x.MaterialId, Codigo = x.Codigo }).ToDictionary(x => x.Codigo.ToUpper().Trim());
            var campaniaMaterialPorMes = new List<CampanaMaterialDetallePorMes>();

            try
            {

                foreach (var item in listProve)
                {
                    List<CompraDetalleAgentDto> hist = comprasDetalleAgent.ComprarIniciales(item.CUIT, item.UsuarioDirectory);
                    logger.Debug("Campos a Acualizar para " + item.UsuarioDirectory + "-" + item.CUIT.Count + ": " + hist.Count);
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    if (hist.Count > 0)
                    {
                        var ComercialId = oComercial.FirstOrDefault(x => x.IdActiveDirectory.ToLower().Trim() == item.UsuarioDirectory.ToLower().Trim()).ComercialId;
                        
                        foreach (var ii in hist)
                        {
                            
                            //if (!proveedores.Any(a => a.Key == ii.VENDEDOR))
                            //{
                            //    logger.Debug("El cuit Vendedor " + ii.VENDEDOR + " no existe en DataAgro. Comercial:" + item.UsuarioDirectory + " .Contrato:" + ii.CONTRATO + " .Vendedor:" + ii.VENDEDOR);
                            //    continue;
                            //}
                            if (!materiales.Any(a => a.Key == ii.MATERIAL))
                            {
                                logger.Debug("El material " + ii.MATERIAL + " no existe en DataAgro. Comercial:" + item.UsuarioDirectory + " .Contrato:" + ii.CONTRATO + " .Vendedor:" + ii.VENDEDOR);
                                continue;
                            }
                            if (!campanias.Any(a => a.Key == ii.COSECHA))
                            {
                                logger.Debug("La COSECHA " + ii.COSECHA + " no existe en DataAgro. Comercial:" + item.UsuarioDirectory + " .Contrato:" + ii.CONTRATO + " .Vendedor:" + ii.VENDEDOR);
                                continue;
                            }


                            var materialId = materiales[ii.MATERIAL].MaterialId;
                            var campaniaId = campanias[ii.COSECHA].CampañaId;
                            var fechaDetalle = DateTime.ParseExact(ii.FECHA, "yyyy-MM-dd", provider);
                            int? CorredorId = null;
                            if (!string.IsNullOrWhiteSpace(ii.CORREDOR))
                            {
                                var corredor = corredores.Where(a => a.CUIT == ii.CORREDOR).FirstOrDefault();
                                if (corredor != null && corredor.ProveedorId > 0)
                                {
                                    CorredorId = corredor.ProveedorId;
                                }
                            }
                            if (proveedores.Any(a => a.Key == ii.VENDEDOR))
                            {
                                var proveedoresId = proveedores[ii.VENDEDOR];
                                foreach (var proveedorId in proveedoresId)
                                {
                                    var campaniaMaterialMes = histActual.Where(x => x.Contrato == ii.CONTRATO && x.ProveedorId == proveedorId.ProveedorId).FirstOrDefault();

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
                                        campaniaMaterialMes.Contrato = ii.CONTRATO;
                                        campaniaMaterialMes.CorredorCuit = ii.CORREDOR;
                                        campaniaMaterialMes.CorredorId = CorredorId;
                                        campaniaMaterialMes.CampanaSAPId = campaniaId;
                                        campaniaMaterialMes.CampanaId = CalcularCampanaId(DateTime.ParseExact(ii.FECHA_DESDE, "yyyy-MM-dd", provider))?? campaniaId;
                                        campaniaMaterialMes.ProveedorId = proveedorId.ProveedorId;
                                        campaniaMaterialMes.MaterialId = materialId;
                                        campaniaMaterialMes.FechaDesde = DateTime.ParseExact(ii.FECHA_DESDE, "yyyy-MM-dd", provider);
                                        campaniaMaterialMes.FechaHasta = DateTime.ParseExact(ii.FECHA_HASTA, "yyyy-MM-dd", provider);
                                        campaniaMaterialMes.CampanaDesc = CalcularCampana(DateTime.ParseExact(ii.FECHA_DESDE, "yyyy-MM-dd", provider));
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
                                            Contrato = ii.CONTRATO,
                                            CorredorCuit = ii.CORREDOR,
                                            ComercialId = ComercialId,
                                            CorredorId = CorredorId,
                                            CampanaSAPId = campaniaId,
                                            CampanaId = CalcularCampanaId(DateTime.ParseExact(ii.FECHA_DESDE, "yyyy-MM-dd", provider))?? campaniaId,
                                            ProveedorId = proveedorId.ProveedorId,
                                            MaterialId = materialId,
                                            FechaDesde = DateTime.ParseExact(ii.FECHA_DESDE, "yyyy-MM-dd", provider),
                                            FechaHasta = DateTime.ParseExact(ii.FECHA_HASTA, "yyyy-MM-dd", provider),
                                            CampanaDesc = CalcularCampana(DateTime.ParseExact(ii.FECHA_DESDE, "yyyy-MM-dd", provider))
                                    });
                                    }
                                }
                            }
                            else
                            {
                                if (CorredorId == null)
                                {
                                    logger.Debug("El cuit Vendedor " + ii.VENDEDOR + " y el cuit Corredor " + ii.CORREDOR + " no existe en DataAgro. Comercial:" + item.UsuarioDirectory + " .Contrato:" + ii.CONTRATO + " .Vendedor:" + ii.VENDEDOR + " .Corredor:" + ii.CORREDOR);
                                    continue;
                                }
                                else
                                {
                                    var campaniaMaterialMes = histActual.Where(x => x.Contrato == ii.CONTRATO ).FirstOrDefault();

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
                                        campaniaMaterialMes.Contrato = ii.CONTRATO;
                                        campaniaMaterialMes.CorredorCuit = ii.CORREDOR;
                                        campaniaMaterialMes.CorredorId = CorredorId;
                                        campaniaMaterialMes.CampanaSAPId = campaniaId;
                                        campaniaMaterialMes.CampanaId = CalcularCampanaId(DateTime.ParseExact(ii.FECHA_DESDE, "yyyy-MM-dd", provider))?? campaniaId;
                                        campaniaMaterialMes.ProveedorId = null;
                                        campaniaMaterialMes.MaterialId = materialId;
                                        campaniaMaterialMes.FechaDesde = DateTime.ParseExact(ii.FECHA_DESDE, "yyyy-MM-dd", provider);
                                        campaniaMaterialMes.FechaHasta = DateTime.ParseExact(ii.FECHA_DESDE, "yyyy-MM-dd", provider);
                                        campaniaMaterialMes.CampanaDesc = CalcularCampana(DateTime.ParseExact(ii.FECHA_DESDE, "yyyy-MM-dd", provider));
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
                                            Contrato = ii.CONTRATO,
                                            CorredorCuit = ii.CORREDOR,
                                            ComercialId = ComercialId,
                                            CorredorId = CorredorId,
                                            CampanaSAPId = campaniaId,
                                            CampanaId = CalcularCampanaId(DateTime.ParseExact(ii.FECHA_DESDE, "yyyy-MM-dd", provider))?? campaniaId,
                                            ProveedorId = null,
                                            MaterialId = materialId,
                                            FechaDesde = DateTime.ParseExact(ii.FECHA_DESDE, "yyyy-MM-dd", provider),
                                            FechaHasta = DateTime.ParseExact(ii.FECHA_HASTA, "yyyy-MM-dd", provider),
                                            CampanaDesc = CalcularCampana(DateTime.ParseExact(ii.FECHA_DESDE, "yyyy-MM-dd", provider))

                                        });
                                    }
                                }
                            }


                        }
                    }
                }

                logger.Debug("Nuevos CampanaMaterialDetalle por mes: " + campaniaMaterialPorMes.Count);
                repositorio.AgregarTodos(campaniaMaterialPorMes);

                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error("ProcessCompras ERROR");
                logger.Error(ex);
                throw;
            }
        }

        private int? CalcularCampanaId(DateTime fecha)
        {
            var campaña = CalcularCampana(fecha);
            var campana = repositorio.Obtener<Campaña>(a => a.Descripcion == campaña);
            if(campana == null)
            {
                return null;
            }
            return campana.CampañaId;
        }

        private string CalcularCampana(DateTime fecha)
        {
            int anio = fecha.Year;
            return fecha.Month > 3 ? 
                (anio -1).ToString().Substring(2,2) + "-" + anio.ToString().Substring(2, 2) : 
                (anio - 2).ToString().Substring(2, 2) + "-" + (anio -1).ToString().Substring(2, 2);
        }

    }
}
