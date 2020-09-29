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
    public class CartaDePresentacionController : Controller
    {
        private ICartaDePresentacionManager cartaDePresentacionManager;
        private readonly IReportesManager reportesManager;

        public CartaDePresentacionController(IReportesManager reportesManager, ICartaDePresentacionManager cartaDePresentacionManager)
        {
            this.reportesManager = reportesManager;
            this.cartaDePresentacionManager = cartaDePresentacionManager;
        }

        public async Task<ActionResult> Generar(RptCartaDePresentacionInfo oParam, List<NuevoProduccion> nuevosCampos, List<NuevoAcopio> nuevosAcopios)
        { 
            var model = new ReportesModel();

            var cartaDePresentacion = new LstCartaDePresentacion(reportesManager);

            var datos = cartaDePresentacionManager.GenerarCartaDePresentacion(oParam, nuevosCampos, nuevosAcopios);

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


