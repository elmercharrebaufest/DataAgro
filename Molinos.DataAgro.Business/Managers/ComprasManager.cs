using Autofac.Extras.NLog;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Molinos.DataAgro.Agent;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Mapping.Context;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;

namespace Molinos.DataAgro.Business
{
    public class ComprasManager : IComprasManager
    {
        private IUnitOfWorkAsync mobjUnitOfWork;
        private ILogger logger;

        public ComprasManager(ILogger logger, IMSContextProvider oMSContextProvider)
        {
            this.logger = logger;
            mobjUnitOfWork = new UnitOfWork(oMSContextProvider.GetMSContext(), new DataAgroContext(oMSContextProvider.GetMSContext()));
        }

        public void ActualizarCompras()
        {
            var a = new List<string>();

            var oProveedor = mobjUnitOfWork.Repository<Proveedor>().Queryable().AsNoTracking().ToList();
            var oProveedorComercial = mobjUnitOfWork.Repository<ProveedorComercial>().Queryable().AsNoTracking();
            var oComercial = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking().Where(x=> x.ComercialId == 4).ToList();

            List<ComprasIniciales> listProve = new List<ComprasIniciales>();
            ComprasIniciales comp = null;
            foreach (var comercial in oComercial)
            {
                var i = 0;
                comp = new ComprasIniciales();
                foreach (var proveedor in oProveedor)
                {
                    comp.CUIT.Add(proveedor.CUIT);
                    //i++;

                    //if (i==1)
                    //{
                    //    listProve.Add(comp);
                    //    ActualizarComprasProveedorIniciales(listProve);
                    //}
                }

                comp.UsuarioDirectory = comercial.IdActiveDirectory;
                listProve.Add(comp);
            }
            
            ActualizarComprasProveedorIniciales(listProve);

        }


        public void ActualizarComprasAyer()
        {
            var a = new List<string>();
            var ayer = DateTime.Now.AddDays(-1);
            var oProveedor = mobjUnitOfWork.Repository<Proveedor>().Queryable().AsNoTracking().Where(x=> x.FechaAlta >= ayer).ToList();
            var oProveedorComercial = mobjUnitOfWork.Repository<ProveedorComercial>().Queryable().AsNoTracking();
            var oComercial = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking().ToList();
            //var oEstados = mobjUnitOfWork.Repository<Estado>().Queryable().AsNoTracking().ToList();

            List<ComprasIniciales> listProve = new List<ComprasIniciales>();
            ComprasIniciales comp = null;
            if (oProveedor.Count > 0) { 
                foreach (var comercial in oComercial)
                {
                    var i = 0;
                    comp = new ComprasIniciales();
                    foreach (var proveedor in oProveedor)
                    {
                        comp.CUIT.Add(proveedor.CUIT);
                    }

                    comp.UsuarioDirectory = comercial.IdActiveDirectory;
                    listProve.Add(comp);
                }

                ActualizarComprasProveedorIniciales(listProve);
            }

        }



        public void ActualizarComprasProveedorIniciales(List<ComprasIniciales> listProve)
        {
            var SapCompras = new ComprasAgent();
            var oComercial = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking();

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
                                    var aa = mobjUnitOfWork.SelStore<FakeValor>("DataAgro_CampañaMaterialPorMes_Actualizar", ii.ANIO, ii.COSECHA, ii.MATERIAL, ii.TN_COMPRADAS, Helper.DevolverIdMes(ii.MES), ii.VENDEDOR, ComercialId).ToList();
                                }
                                catch (Exception ex)
                                {

                                    throw;
                                }
                            }

                            var listHistorial = hist.GroupBy(x => new { x.VENDEDOR, x.MATERIAL, x.COSECHA });

                            foreach (var jj in listHistorial)
                            {
                                try
                                {
                                    var aa = mobjUnitOfWork.SelStore<FakeValor>("DataAgro_CampañaMaterialPorMes_ToneladasActualizar", jj.Key.COSECHA, jj.Key.MATERIAL, jj.Key.VENDEDOR).ToList();
                                }
                                catch (Exception ex)
                                {

                                    throw;
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
            }
        }

        public void ActualizarComprasProveedor(List<Datos> listProve)
        {
            var SapCompras = new ComprasAgent();
            var oComercial = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking();

            if (ConfigurationManager.AppSettings["SinConexionSap"] == "0")
            { 
                foreach (var item in listProve)
                {
                    try
                    {

                        List<Agent.Compras.ZMPES5130> hist = new List<Agent.Compras.ZMPES5130>();

                        if (ConfigurationManager.AppSettings["usuarioLaura"].ToString() == "1" && item.UsuarioDirectory.ToLower() == ConfigurationManager.AppSettings["usuarioLaurastring"].ToString().ToLower())
                        {
                            var aux = ConfigurationManager.AppSettings["SapPruebaUser"].ToString();
                            hist = SapCompras.Comprar(item.CUIT, aux);
                        }
                        else
                        {
                            hist = SapCompras.Comprar(item.CUIT, item.UsuarioDirectory);
                        }

                        if (hist.Count > 0)
                        {
                            foreach (var ii in hist)
                            {
                                
                                var ComercialId = oComercial.FirstOrDefault(x => x.IdActiveDirectory.ToLower().Trim() == item.UsuarioDirectory.ToLower().Trim()).ComercialId;
                                try
                                {
                                    var aa = mobjUnitOfWork.SelStore<FakeValor>("DataAgro_CampañaMaterialPorMes_Actualizar", ii.ANIO, ii.COSECHA, ii.MATERIAL, ii.TN_COMPRADAS, Helper.DevolverIdMes(ii.MES), item.CUIT,ComercialId).ToList();
                                }
                                catch (Exception ex)
                                {

                                    throw;
                                }

                            }

                            var listHistorial = hist.GroupBy(x => new { x.VENDEDOR, x.MATERIAL, x.COSECHA });

                            foreach (var jj in listHistorial)
                            {
                                try
                                {
                                    var aa = mobjUnitOfWork.SelStore<FakeValor>("DataAgro_CampañaMaterialPorMes_ToneladasActualizar", jj.Key.COSECHA, jj.Key.MATERIAL, item.CUIT).ToList();
                                }
                                catch (Exception ex)
                                {

                                    throw;
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
            }
        }

        public Proveedor TraerProveedor(string CUIT)
        {
            var oProveedor = new Proveedor();

            oProveedor = mobjUnitOfWork.Repository<Proveedor>()
                                 .Queryable()
                                 .Where(x => x.CUIT == CUIT)
                                 .SingleOrDefault();

            if (oProveedor != null)
            {
                oProveedor.ObjectState = Constants.Object_Modified;
            }

            return oProveedor;
        }

        public void ArmarCargaInicial()
        {
            var proveedorcomercial = mobjUnitOfWork.Repository<ProveedorComercial>().Queryable().AsNoTracking();
            var proveedor = mobjUnitOfWork.Repository<Proveedor>().Queryable().AsNoTracking();
            var comercial = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking();
            var datos = proveedorcomercial
                        .Join(proveedor, x => x.ProveedorId, c => c.ProveedorId, (x, c) => new { PC = x, P = c })
                        .Join(comercial, x => x.PC.ComercialId, c => c.ComercialId, (x, c) => new { x.PC, x.P, C = c })
                        .Select(x => new Datos
                        {
                            CUIT = x.P.CUIT,
                            UsuarioDirectory = x.C.IdActiveDirectory
                        })
                        .ToList();
            ActualizarComprasProveedor(datos);
        }


    }

    public class FakeValor
    {
        public int? Error { get; set; }
    }
}
