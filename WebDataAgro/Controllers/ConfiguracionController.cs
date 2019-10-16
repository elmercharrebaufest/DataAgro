using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDataAgro.Models;

namespace WebDataAgro.Controllers
{
    public class ConfiguracionController : Controller
    {
        private readonly IConfiguracionManager configuracionManager;

        public ConfiguracionController(IConfiguracionManager configuracionManager)
        {
            this.configuracionManager = configuracionManager;
        }

        public ActionResult Index()
        {
            var conf = configuracionManager.TraerConfiguraciones();

            return View(new ConfiguracionModel
            {
                CantidadDias = conf != null ? conf.CantidadDias : 0,
                TerminalStopId = conf != null ? conf.TerminalStopId : 0,
                CuitDestinoStop = conf != null ? conf.CuitDestinoStop : "",
                ConexionConsultaStop = conf != null ? conf.ConexionConsultaStop.Value : true,
                ConexionABMStop = conf != null ? conf.ConexionABMStop.Value : true,
                ClaveStop = conf.ClaveStop,
                CodigoLocalidadStop = conf.CodigoLocalidadStop
            });
        }
        [HttpPost]
        public ActionResult GuardarPesificacionDolarizado(ConfiguracionModel configuracion)
        {
            var configuracionGrabada = configuracionManager.GrabarFechaPesificacionDolarizado(TransformarAEntidad(configuracion));
           
            if (configuracionGrabada.HayError ||!ModelState.IsValid )
            {
                foreach (var e in configuracionGrabada.Errores)
                {
                    ModelState.AddModelError(e.ErrorCode.ToString(), e.Message);
                }                
            }
            return View("Index",configuracion);
        }

        private Configuracion TransformarAEntidad(ConfiguracionModel configuracion)
        {
            var entidad = new Configuracion
            {
                Id = configuracion.Id,
                CantidadDias = configuracion.CantidadDias,
                ConexionABMStop= configuracion.ConexionABMStop,
                ConexionConsultaStop= configuracion.ConexionConsultaStop,
                ClaveStop =configuracion.ClaveStop,
                CuitDestinoStop = configuracion.CuitDestinoStop,
                TerminalStopId = configuracion.TerminalStopId,
                CodigoLocalidadStop = configuracion.CodigoLocalidadStop
            };
            return entidad;
        }
    }
}