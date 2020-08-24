using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Mvc;

namespace WebDataAgro.Controllers
{
    public class SincronizarMaestrosController : Controller
    {
        private readonly IRG2300Manager rG2300Manager;
        private readonly IFacacopManager facacopManager;
        private readonly IEstadoProveedorManager estadoProveedorManager;
        private readonly ISISAManager sisaManager;
        private readonly ILogger logger;
        private readonly IComprasManager comprasManager;

        public SincronizarMaestrosController(ILogger log, IComprasManager comprasManager, IRG2300Manager rG2300Manager, IFacacopManager facacopManager, IEstadoProveedorManager estadoProveedorManager, ISISAManager sisaManager)
        {
            this.logger = log;
            this.comprasManager = comprasManager;
            this.rG2300Manager = rG2300Manager;
            this.facacopManager = facacopManager;
            this.estadoProveedorManager = estadoProveedorManager;
            this.sisaManager = sisaManager;
        }

        // GET: SincronizarMaestros
        public ActionResult ProcessComprasAyer()
        {
            logger.Info($"ProcessComprasAyer - Iniciando");
            comprasManager.ActualizarComprasAyer();
            logger.Info($"ProcessComprasAyer - Fin");
            return Content("ok");
        }

        public ActionResult ProcessRg2300()
        {
            logger.Info("ProcessRg2300 - Iniciando");
            var str = ConfigurationManager.AppSettings["Rg2300"];

            var objReader = new StreamReader(str.ToString(), System.Text.Encoding.Default);

            string sLine = "";
            var arrText = new ArrayList();

            var lista = new List<RG2300>();

            var j = 0;

            while (sLine != null)
            {
                sLine = objReader.ReadLine();

                if (j == 0)
                {
                    j = 1;
                }
                else
                {
                    if (sLine != null)
                    {
                        var Rg = sLine.Split(';');
                        lista.Add(new RG2300
                        {
                            CUIT = !String.IsNullOrEmpty(Rg[0]) ? Rg[0].Replace('"', '\0').Replace('\\', '\0') : String.Empty,
                            RazonSocial = !String.IsNullOrEmpty(Rg[1]) ? Rg[1].Replace("\"", String.Empty) : String.Empty,
                            Categoria = !String.IsNullOrEmpty(Rg[2]) ? Rg[2].Replace("\"", String.Empty) : String.Empty,
                            Situacion = !String.IsNullOrEmpty(Rg[3]) ? Rg[3].Replace("\"", String.Empty) : String.Empty,
                            CBU = !String.IsNullOrEmpty(Rg[4]) ? Rg[4].Replace("\"", String.Empty) : String.Empty,
                            FechaActCBU = !String.IsNullOrEmpty(Rg[5]) ? (DateTime?)DateTime.Parse(Rg[5]) : null,
                            FechaPubInclusion = !String.IsNullOrEmpty(Rg[6]) ? (DateTime?)DateTime.Parse(Rg[6]) : null,
                            FechaPubSuspension = !String.IsNullOrEmpty(Rg[7]) ? (DateTime?)DateTime.Parse(Rg[7]) : null,
                            FechaLevSuspension = !String.IsNullOrEmpty(Rg[8]) ? (DateTime?)DateTime.Parse(Rg[8]) : null,
                            FechaNotExclusion = !String.IsNullOrEmpty(Rg[9]) ? (DateTime?)DateTime.Parse(Rg[9]) : null,
                            FechaActRegistro = !String.IsNullOrEmpty(Rg[10]) ? (DateTime?)DateTime.Parse(Rg[10]) : null,
                            Observaciones = !String.IsNullOrEmpty(Rg[11]) ? Rg[11].Replace("\"", String.Empty) : String.Empty,
                            FechaGeneracion = !String.IsNullOrEmpty(Rg[12]) ? (DateTime?)DateTime.Parse(Rg[12]) : null
                        });
                    }
                }
            }
            objReader.Close();
            logger.Info($"ProcessRg2300 - Lineas leidas: {lista.Count}");
            try
            {
                rG2300Manager.InsetarRG2300(lista);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            logger.Info($"ProcessRg2300 - Lineas INSERTADAS: {lista.Count}");
            return Content("ok");
        }

        public ActionResult ProcessSisa()
        {

            logger.Info("ProcessSisa - Iniciando");
            var str = ConfigurationManager.AppSettings["SISA"];

            var objReader = new StreamReader(str.ToString(), System.Text.Encoding.Default);

            string sLine = "";
            var arrText = new ArrayList();

            var lista = new List<SISA>();
            var cuits = new List<SISA>();
            var j = 0;

            while (sLine != null)
            {
                sLine = objReader.ReadLine();

                if (j == 0)
                {
                    j = 1;
                }
                else if (j == 1)
                {
                    j = 2;
                }
                else
                {

                    if (sLine != null)
                    {
                        var sisa = sLine.Split(';');
                        var sisaList = new SISA();

                        sisaList.CUIT = !String.IsNullOrEmpty(sisa[0]) ? sisa[0].Replace('"', '\0').Replace('\\', '\0') : String.Empty;
                        sisaList.RazonSocial = !String.IsNullOrEmpty(sisa[1]) ? sisa[1].Replace("\"", String.Empty) : String.Empty;
                        sisaList.EstadoCuit = !String.IsNullOrEmpty(sisa[2]) ? Int32.Parse(sisa[2].Replace("\"", String.Empty)) : 0;
                        sisaList.FechaVigenciaEstado = !String.IsNullOrEmpty(sisa[3]) ? (DateTime?)DateTime.Parse(sisa[3]) : null;
                        sisaList.FechaNotifDFEEstado = !String.IsNullOrEmpty(sisa[4]) ? (DateTime?)DateTime.Parse(sisa[4]) : null;
                        sisaList.CBU = !String.IsNullOrEmpty(sisa[5]) ? sisa[5].Replace("\"", String.Empty) : String.Empty;
                        sisaList.FechaActCBU = !String.IsNullOrEmpty(sisa[6]) ? (DateTime?)DateTime.Parse(sisa[6]) : null;
                        sisaList.CodCategoria = !String.IsNullOrEmpty(sisa[7]) ? Int32.Parse(sisa[7].Replace("\"", String.Empty)) : 0;
                        sisaList.Categoria = !String.IsNullOrEmpty(sisa[8]) ? sisa[8].Replace("\"", String.Empty) : String.Empty;
                        sisaList.SituacionCategoria = !String.IsNullOrEmpty(sisa[9]) ? sisa[9].Replace("\"", String.Empty) : String.Empty;
                        sisaList.FechaVigenciaCategoria = !String.IsNullOrEmpty(sisa[10]) ? (DateTime?)DateTime.Parse(sisa[10]) : null;
                        sisaList.FechaNotifDFECategoria = !String.IsNullOrEmpty(sisa[11]) ? (DateTime?)DateTime.Parse(sisa[11]) : null;
                        sisaList.Observaciones = !String.IsNullOrEmpty(sisa[12]) ? sisa[12].Replace("\"", String.Empty) : String.Empty;
                        sisaList.FechaGeneracion = !String.IsNullOrEmpty(sisa[13]) ? (DateTime?)DateTime.Parse(sisa[13]) : null;

                        if (sisaList.FechaVigenciaEstado > DateTime.Now.Date || sisaList.FechaVigenciaCategoria > DateTime.Now.Date)
                        {
                            cuits.Add(sisaList);
                        }
                        else
                        {
                            lista.Add(sisaList);
                        }
                    }
                }
            }
            objReader.Close();
            logger.Info($"ProcessSisa - Lineas leidas: {lista.Count}");
            int lineas = 0;
            try
            {
                lineas = sisaManager.InsertarSISA(lista, cuits);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            logger.Info($"ProcessRg2300 - Lineas INSERTADAS: {lineas}");
            return Content("ok");
        }

        public ActionResult ProcessFacacop()
        {
            logger.Info("ProcessFacacop - Iniciando");
            var str = ConfigurationManager.AppSettings["Facacop"];

            StreamReader objReader = new StreamReader(str.ToString());

            string sLine = "";
            ArrayList arrText = new ArrayList();

            var lista = new List<FACACOP>();

            var j = 0;

            while (sLine != null)
            {
                sLine = objReader.ReadLine();

                if (j < 3)
                {
                    j += 1;
                }
                else
                {
                    if (sLine != null)
                    {
                        var Fc = sLine.Split(',');
                        Fc = sLine.Split(new[] { ',' }, 4);
                        FACACOP obj = new FACACOP();

                        obj.CUIT = !String.IsNullOrEmpty(Fc[0]) ? Fc[0].Replace('"', '\0').Replace('\\', '\0') : String.Empty;
                        obj.Fecha1 = !String.IsNullOrEmpty(Fc[1]) ? DateTime.Parse(Fc[1]) : DateTime.Now;
                        obj.Fecha2 = !String.IsNullOrEmpty(Fc[2]) ? DateTime.Parse(Fc[2]) : DateTime.Now;
                        obj.ObservacionesEspeciales = !String.IsNullOrEmpty(Fc[3]) ? Fc[3] : String.Empty;

                        lista.Add(obj);
                    }
                }
            }
            objReader.Close();
            logger.Info($"ProcessFacacop - Lineas leidas: {lista.Count}");
            try
            {
                facacopManager.InsetarFacacop(lista);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            logger.Info($"ProcessRg2300 - Lineas INSERTADAS: {lista.Count}");
            return Content("ok");
        }

        public ActionResult ProcessEstado()
        {
            logger.Info($"ProcessEstado - Iniciando");
            estadoProveedorManager.ActualizarProveedores();
            logger.Info($"ProcessEstado - Finalizado");
            return Content("ok");
        }

        public ActionResult ProcessCompras()
        {
            logger.Info($"ProcessCompras - Iniciando");
            comprasManager.ActualizarCompras();
            logger.Info($"ProcessCompras - Finalizado");
            return Content("ok");
        }

        static readonly object _lockProcessComprasDetalle = new object();
        public ActionResult ProcessComprasDetalle(string comercialUsurarioAD = "", string cuit = "")
        {
            lock (_lockProcessComprasDetalle)
            {
                logger.Info($"ProcessComprasDetalle - Iniciando");
                comprasManager.ActualizarComprasDetalle(comercialUsurarioAD, cuit);
                logger.Info($"ProcessComprasDetalle - Finalizado");
                return Content("ok");
            }

        }
    }
}