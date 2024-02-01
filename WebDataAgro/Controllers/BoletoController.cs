using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class BoletoController : Controller
    {
        private IBoletoManager mobjBoletoManager;
        private readonly IReportesManager reportesManager;
        private readonly string _logDir;

        public BoletoController(IBoletoManager oBoletoManager, IReportesManager reportesManager)
        {
            this.mobjBoletoManager = oBoletoManager;
            this.reportesManager = reportesManager;
            _logDir = ConfigurationManager.AppSettings["PathBoletos"].ToString();
        }
        public ActionResult Index()
        {
            CompletarVista();
            return View();
        }

        [HttpGet]
        public ActionResult GenerarBoletos()
        {
            CompletarVista();
            return View();
        }

        public ActionResult InicializarContrato(int? tipoNegocioId)
        {
            return new JsonResult()
            {
                Data = new ContratoModel_prueba
                {
                    Datos = mobjBoletoManager.TraerDatosCombo(tipoNegocioId)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpPost]
        public ActionResult GenerarBoletos(BoletoGeneradoDto boleto)
        {
            CompletarVista();
            //List<string> contratos = new List<string>();
            var tipoNegocios = new List<int>();
            if (boleto.TipoNegocioId == 1) // Contrato
            {
                tipoNegocios.Add((int)EnumTipoNegocio.A_FIJAR);
                tipoNegocios.Add((int)EnumTipoNegocio.A_PRECIO);
            }
            else // Fijación
            {
                tipoNegocios.Add((int)EnumTipoNegocio.FIJACION);
            }
            List<string> contratos = new List<string>();
            foreach (string itemContrato in boleto.ContratoSAP.Split(';').ToList())
            {
                contratos.Add(itemContrato.PadLeft(10, '0'));
            }
            var boletos = mobjBoletoManager.GrabarBoleto(tipoNegocios, GlobalVariables.ComercialId, contratos, boleto.Mail, GlobalVariables.EquipoReal);
            //var boletos = new BoletoGeneradoDto { ContratoSAP = "000036363", Generado = true };
            return new JsonResult()
            {
                Data = boletos.boletosGenerados,
                MaxJsonLength = Int32.MaxValue
            };
        }

        private void CompletarVista()
        {
            var tipoNegocio = mobjBoletoManager.TraerDatosCombo(null);
            var tiposListItems = tipoNegocio.tiponegocio.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.TipoNegocioId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.TipoNegocio = tiposListItems;
        }

        [HttpGet]
        public ActionResult DescargarBoletos()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ListarBoletos()
        {
            var boletos = Directory.GetFiles(_logDir)
                .Where(path => path.EndsWith(".pdf"))
                .Select(path => new FileInfo(path))
                .Select(file => new BoletoArchivoDto
                {
                    Nombre = file.Name,
                    FechaUltimaEscritura = file.LastWriteTime.ToString("yyyy/MM/dd HH:mm"),
                    Url = _logDir + file.Name,
                    Tamano = GetFriendlyFileSize(file.Length)
                })
                .OrderByDescending(x => x.Nombre)
                .ToList();

            return Json(boletos);
        }

        private string GetFriendlyFileSize(long lengthInBytes)
        {
            var kb = Math.Round(lengthInBytes / 1024d);
            var groupSeparator = NumberFormatInfo.CurrentInfo.NumberGroupSeparator;
            var friendly = kb.ToString("N0").Replace(groupSeparator, " ") + " KB";
            return friendly;
        }

        public ActionResult ObtenerDownloadKey(oParamBusqueda filtro)
        {

            var identif = mobjBoletoManager.ObtenerIdentDescarga();

            return Json(Util.GetDownloadKey(identif));
        }

        //[HttpPost]
        public ActionResult DescargarArchivoBoleto(string nombre)
        {
            try
            {
                Byte[] fileBytes = mobjBoletoManager.BoletoEnByte(_logDir + "\\" + nombre);

                if (fileBytes == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    //return Json(fileBytes); 
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, nombre);
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
    }
}
