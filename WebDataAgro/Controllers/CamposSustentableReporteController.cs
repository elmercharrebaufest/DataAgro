using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report.Clases;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class CamposSustentablesController : Controller
    {
        private readonly IReportesManager reportesManager;
        private ILogger logger;

        public CamposSustentablesController(IReportesManager reportesManager, ILogger logger)
        {
            this.reportesManager = reportesManager;
            this.logger = logger;
        }

        public async Task<ActionResult> Generar(DeclaracionCampoSustentable datos)
        {
            try
            {
                logger.Debug("CamposSustentables Generar datos:");
                logger.Debug(datos.ToJson());
            }
            catch (Exception)
            {
            }


            int i = 1;
            foreach (var item in datos.Campos)
            {
                item.N = i++;
            }
            var model = new ReportesModel();

            var cartaDePresentacion = new LstCamposSustentables(reportesManager);

            var identif = await cartaDePresentacion.GenerarAsync(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);


            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


    }
}


