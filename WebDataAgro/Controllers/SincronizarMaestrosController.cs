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
        private readonly ILogger logger;
        private readonly IComprasManager comprasManager;

        public SincronizarMaestrosController(ILogger log, IComprasManager comprasManager, IRG2300Manager rG2300Manager, IFacacopManager facacopManager, IEstadoProveedorManager estadoProveedorManager)
        {
            this.logger = log;
            this.comprasManager = comprasManager;
            this.rG2300Manager = rG2300Manager;
            this.facacopManager = facacopManager;
            this.estadoProveedorManager = estadoProveedorManager;
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
    }
}