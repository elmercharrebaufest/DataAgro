using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
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
    public class FormularioAltaNoGranosController : Controller
    {
        private readonly IReportesManager reportesManager;

        public FormularioAltaNoGranosController(IReportesManager reportesManager)
        {
            this.reportesManager = reportesManager;
        }

        public async Task<ActionResult> Generar(ProveedorAltaDto oParam)
        {
            if (oParam.CUIT.Length == 11)
            {
                oParam.CUIT = oParam.CUIT.Substring(0, 2) + "-" + oParam.CUIT.Substring(2, 8) + "-" + oParam.CUIT.Substring(9, 1);
            }
            var model = new ReportesModel();

            var formularioAltaNoGranos = new LstFormularioAltaNoGranos(reportesManager);

            var identif = await formularioAltaNoGranos.GenerarAsync(oParam);

            model.DownloadKey = Util.GetDownloadKey(identif);


            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


    }
}


