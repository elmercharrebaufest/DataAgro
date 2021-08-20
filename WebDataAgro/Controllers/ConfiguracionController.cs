using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Models;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class ConfiguracionController : Controller
    {
        private readonly IConfiguracionManager configuracionManager;

        public ConfiguracionController(IConfiguracionManager configuracionManager)
        {
            this.configuracionManager = configuracionManager;
        }

        [Autorizacion(PermisosDataAgro.ConfiguracionConstantes)]
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
                CodigoLocalidadStop = conf.CodigoLocalidadStop,
                ImporteSustentable = conf.ImporteSustentable,
                ContratoAperturaPrecioPorcentajeDeComisionMaximo = conf.ContratoAperturaPrecioPorcentajeDeComisionMaximo,
                DiasDiferimiento = conf != null ? conf.DiasDiferimiento.Value : 0,
                CantidadAcuerdo = conf != null ? conf.CantidadAcuerdo.Value : 0,
                CantidadDiasDolarizadoLimiteMaximo = conf != null ? conf.CantidadDiasDolarizadoLimiteMaximo : 0,
                CantidadMaxima = conf != null ? conf.CantidadMaxima : 0,
                CantidadDiasPesificadoLimite = conf != null ? conf.CantidadDiasPesificadoLimite : 0,
                RedespachoMaximo = conf != null ? conf.RedespachoMaximo : 0,
            });
        }
        [HttpPost]
        public ActionResult GuardarPesificacionDolarizado(ConfiguracionModel configuracion)
        {
            var configuracionGrabada = configuracionManager.GrabarFechaPesificacionDolarizado(TransformarAEntidad(configuracion));
           
            if (configuracionGrabada.HayError || !ModelState.IsValid )
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
                ConexionABMStop = configuracion.ConexionABMStop,
                ConexionConsultaStop = configuracion.ConexionConsultaStop,
                ClaveStop = configuracion.ClaveStop,
                CuitDestinoStop = configuracion.CuitDestinoStop,
                TerminalStopId = configuracion.TerminalStopId,
                CodigoLocalidadStop = configuracion.CodigoLocalidadStop,
                ImporteSustentable = configuracion.ImporteSustentable,
                ContratoAperturaPrecioPorcentajeDeComisionMaximo = configuracion.ContratoAperturaPrecioPorcentajeDeComisionMaximo,
                DiasDiferimiento = configuracion.DiasDiferimiento,
                CantidadAcuerdo = configuracion.CantidadAcuerdo,
                CantidadDiasDolarizadoLimiteMaximo = configuracion.CantidadDiasDolarizadoLimiteMaximo,
                CantidadMaxima = configuracion.CantidadMaxima,
                CantidadDiasPesificadoLimite = configuracion.CantidadDiasPesificadoLimite,
                RedespachoMaximo = configuracion.RedespachoMaximo
            };
            return entidad;
        }
    }
}