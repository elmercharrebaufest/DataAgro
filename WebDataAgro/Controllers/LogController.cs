using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces.Managers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDataAgro.Models;

namespace WebDataAgro.Controllers
{
    public class LogController :Controller
    {
        private readonly ILogManager oLogManager;

        public LogController (ILogManager oLogManager)
        {
            this.oLogManager = oLogManager;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListaLogPartial(string fechaString)
        {
            DateTime fecha = DateTime.ParseExact(fechaString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            return PartialView("_ListaLog", new LogModel
            {
                Log = TransformarAModel(oLogManager.TraerTodoLog(fecha))

            });
        }

        public List<LogModel> TransformarAModel(List<LogDto> logDto)
        {
            var lista = new List<LogModel>();
            foreach (var i in logDto)
            {
                var logModel = new LogModel
                {
                    Xml = i.Xml, 
                    Fecha = i.Fecha
                };
                lista.Add(logModel);

            }

            return lista;
        }
    }
}