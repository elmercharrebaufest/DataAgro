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
using System.Text.RegularExpressions;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Core;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class BoletoController : Controller
    {
        private readonly IBoletoManager boletoManager;
        private readonly IReportesManager reportesManager;
        private readonly string _logDir;

        public BoletoController(IBoletoManager boletoManager, IReportesManager reportesManager)
        {
            this.boletoManager = boletoManager;
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

        [HttpPost]
        public ActionResult GenerarBoletos(BoletoDto boleto)
        {
            CompletarVista();
            List<string> contratos = new List<string>();
            var tipoNegocios = new List<int>();
            boleto.ComercialId = GlobalVariables.ComercialId;
            if (boleto.TipoNegocioId == 1) // Contrato
            {
                tipoNegocios.Add((int)EnumTipoNegocio.A_FIJAR);
                tipoNegocios.Add((int)EnumTipoNegocio.A_PRECIO);
            }
            else // Fijación
            {
                tipoNegocios.Add((int)EnumTipoNegocio.FIJACION);
            }

            boleto.ContratoSAP = Regex.Replace(boleto.ContratoSAP, @"\s+", ";");

            foreach (string itemContrato in boleto.ContratoSAP.TrimEnd(';').Split(';').ToList())
            {
                contratos.Add(itemContrato.PadLeft(10, '0'));
            }
            var boletos = boletoManager.GrabarBoleto(contratos, tipoNegocios, boleto, GlobalVariables.EquipoReal);

            return new JsonResult()
            {
                Data = boletos.BoletosDto,
                MaxJsonLength = Int32.MaxValue
            };
        }

        private void CompletarVista()
        {
            var tipoNegocio = boletoManager.TraerDatosCombo(null);
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
            List<BoletoArchivoDto> boletos = new List<BoletoArchivoDto>();
            if (Directory.Exists(_logDir))
            {
                boletos = Directory.GetFiles(_logDir)
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
            }

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
            var identif = boletoManager.ObtenerIdentDescarga();

            return Json(Util.GetDownloadKey(identif));
        }

        //[HttpPost]
        public ActionResult DescargarArchivoBoleto(string nombre)
        {
            try
            {
                Byte[] fileBytes = boletoManager.BoletoEnByte(_logDir + "\\" + nombre);

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

        public ActionResult ReenviarEmailBoletos(List<string> contratosBoletos, List<string> nombresArchivos)
        {
            try
            {
                boletoManager.ReenviarBoletos(contratosBoletos, nombresArchivos, _logDir);
                return Json("Ok", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult FiltrarBoletos(string desde, string hasta, int negocio)
        {
            return new JsonResult()
            {
                Data = boletoManager.FiltrarNegociosPorFecha(desde, hasta, negocio)
            };
        }

        public ActionResult ListarContratos(List<int> listaContratoSap, int negocio)
        {
            listaContratoSap.Sort();
            if (listaContratoSap.First().Equals(0) || listaContratoSap.First() > listaContratoSap.Last())
            {
                return new JsonResult()
                {
                    Data = ""
                };
            }

            int contratoDesde = listaContratoSap.First();
            int contratoHasta = listaContratoSap.Last();


            return new JsonResult()
            {
                Data = boletoManager.FiltrarNegociosNumeroSAP(contratoDesde, contratoHasta, negocio)
            };
        }

        [HttpGet]
        public ActionResult GestionarClausulas(string numeroSap, int tipoNegocio)
        {
            ViewBag.Clausulas = boletoManager.ObtenerClausulasPorNegocio(numeroSap, GlobalVariables.EquipoReal);
            ViewBag.NegocioSAP = numeroSap;
            ViewBag.TipoNegocio = tipoNegocio;
            return View();
        }

    }
}
