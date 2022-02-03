using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class BoletoController : Controller
    {
        private IBoletoManager mobjBoletoManager;
        private readonly string _logDir ;

        public BoletoController(IBoletoManager oBoletoManager)
        {
            mobjBoletoManager = oBoletoManager;
            _logDir = ConfigurationManager.AppSettings["BoletosGeneradosPDF"].ToString();
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
        public ActionResult GenerarBoletos(/*bool enviarMail, DataSourceRequest request*/ BoletoGeneradoDto boleto)
        {
            CompletarVista();
            //List<string> contratos = new List<string>();
            var tipoNegocios = new List<int>();
            if(boleto.TipoNegocioId == 1)
            {
                tipoNegocios.Add(1);
                tipoNegocios.Add(2);
            }
            else
            {
                tipoNegocios.Add(3);
            }
            var boletos = mobjBoletoManager.GrabarBoleto(tipoNegocios, GlobalVariables.ComercialId, boleto.ContratoSAP.Split(';').ToList(), boleto.Mail, GlobalVariables.EquipoReal);
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
            //if (!string.IsNullOrEmpty(log))
            //{
            //    return File(Path.Combine(_logDir, log), "text/plain");
            //}

            //var boletos = Directory.GetFiles(_logDir)
            //    .Where(path => path.EndsWith(".pdf"))
            //    .Select(path => new FileInfo(path))
            //    .Select(file => new BoletoArchivoDto
            //    {
            //        Nombre = file.Name,
            //        FechaUltimaEscritura = file.LastWriteTime.ToString("yyyy/MM/dd HH:mm"),
            //        Url = _logDir + "\\\\" + file.Name,
            //        Tamano = GetFriendlyFileSize(file.Length)
            //    })
            //    .OrderByDescending(x => x.Nombre)
            //    .ToList();

            //CorrectSortOrder(logs);

            return Json(null);
        }

        private string GetFriendlyFileSize(long lengthInBytes)
        {
            var kb = Math.Round(lengthInBytes / 1024d);
            var groupSeparator = NumberFormatInfo.CurrentInfo.NumberGroupSeparator;
            var friendly = kb.ToString("N0").Replace(groupSeparator, " ") + " KB";
            return friendly;
        }
    }
}