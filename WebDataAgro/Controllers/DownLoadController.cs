
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Molinos.DataAgro.Business;

using WebDataAgro.Core;
using Molinos.DataAgro.Business.Managers;

namespace WebDataAgro.Controllers
{
    //[Authorize]
    public class DownLoadController : Controller
    {
        //-----------------------------------------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------------------------------------

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

                var oReportesManager = new ReportesManager();
                oReportesManager.Inicializar(oMSContext);

                var oReporte = await oReportesManager.ObtenerReporteAsync(identif);

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

                var oReportesManager = new ReportesManager();
                oReportesManager.Inicializar(oMSContext);

                var oReporte = await oReportesManager.ObtenerReporteAsync(identif);

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