
using Molinos.DataAgro.Interfaces;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Core;

namespace WebDataAgro.Controllers
{
    //[Authorize]
    public class DownLoadController : Controller
    {
        private readonly IReportesManager reportesManager;

        public DownLoadController(IReportesManager reportesManager)
        {
            this.reportesManager = reportesManager;
        }

        public async Task<ActionResult> Reporte(string key)
        {
            try
            {
                var param = Util.DecryptString(key);

                var identif = "";

                var cnPrefix = "";

                var partes = param.Split('|');

                if (partes.Length >= 1)
                {
                    identif = partes[0];
                }

                if (partes.Length >= 2)
                {
                    cnPrefix = partes[1];
                }

                var oMSContext = Util.GetMSContext();

                if (cnPrefix.Length > 1)
                {
                    oMSContext.CNPrefix = cnPrefix;
                }

                var oReporte = await reportesManager.ObtenerReporteAsync(identif);

               if (oReporte == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    return File(oReporte.Contenido, "application/pdf", oReporte.FileName);
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }


        public async Task<ActionResult> Excel(string key)
        {
            try
            {
                var param = Util.DecryptString(key);

                var identif = "";

                var cnPrefix = "";

                var partes = param.Split('|');

                if (partes.Length >= 1)
                {
                    identif = partes[0];
                }

                if (partes.Length >= 2)
                {
                    cnPrefix = partes[1];
                }

                var oMSContext = Util.GetMSContext();

                if (cnPrefix.Length > 1)
                {
                    oMSContext.CNPrefix = cnPrefix;
                }

                var oReporte = await reportesManager.ObtenerReporteAsync(identif);

                if (oReporte == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    return File(oReporte.Contenido, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", oReporte.FileName);
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

    }
}