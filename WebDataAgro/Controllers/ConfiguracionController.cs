using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
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
                RedespachoMaximoUSDM = conf != null ? conf.RedespachoMaximoUSDM : 0,
                RedespachoMaximoARP = conf != null ? conf.RedespachoMaximoARP : 0,
                ToleranciaPaseMin = conf != null ? conf.ToleranciaPaseMin : 0,
                ToleranciaPaseMax = conf != null ? conf.ToleranciaPaseMax : 0,
                ImporteSustentableEspecial = conf.ImporteSustentableEspecial,
                AlgoritmoKilosMinimosParaSugerencia = conf != null ? conf.AlgoritmoKilosMinimosParaSugerencia : 0,
                Actualizacion = conf != null ? conf.Actualizacion : 1,
                ApiKeyBolsaRosario = conf.ApiKeyBolsaRosario,
                SecretBolsaRosario = conf.SecretBolsaRosario,
                MinutosCronometroConDescarga = conf != null ? conf.MinutosCronometroConDescarga : 0,
                CantidadMaximaDiasNegocioConDescarga = conf != null ? conf.CantidadMaximaDiasNegocioConDescarga : 0,
                PorcentajeVolumenNegocioConDescarga = conf != null ? conf.PorcentajeVolumenNegocioConDescarga : 0,
                ExigirNegocioEnSolExt_Soja = conf?.ExigirNegocioEnSolExt_Soja ?? false,
                ExigirNegocioEnSolExt_Maiz = conf?.ExigirNegocioEnSolExt_Maiz ?? false,
                ExigirNegocioEnSolExt_Trigo = conf?.ExigirNegocioEnSolExt_Trigo ?? false,
                ExigirNegocioEnSolExt_Girasol = conf?.ExigirNegocioEnSolExt_Girasol ?? false,
                ApiKeyScoringCupos = conf.ApiKeyScoringCupos,
            });
        }

        [HttpPost]
        public ActionResult GrabarConfiguracion(ConfiguracionModel configuracion)
        {
            var configuracionGrabada = configuracionManager.GrabarFechaPesificacionDolarizado(TransformarAEntidad(configuracion));

            if (configuracionGrabada.HayError || !ModelState.IsValid)
            {
                foreach (var e in configuracionGrabada.Errores)
                {
                    ModelState.AddModelError(e.ErrorCode.ToString(), e.Message);
                }
            }
            return View("Index", configuracion);
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
                RedespachoMaximoUSDM = configuracion.RedespachoMaximoUSDM,
                RedespachoMaximoARP = configuracion.RedespachoMaximoARP,
                ToleranciaPaseMin = configuracion.ToleranciaPaseMin,
                ToleranciaPaseMax = configuracion.ToleranciaPaseMax,
                ImporteSustentableEspecial = configuracion.ImporteSustentableEspecial,
                AlgoritmoKilosMinimosParaSugerencia = configuracion.AlgoritmoKilosMinimosParaSugerencia,
                AlgoritmoProcMaxSugerenciasProveedorDia = configuracion.AlgoritmoProcMaxSugerenciasProveedorDia,
                Actualizacion = configuracion.Actualizacion,
                ApiKeyBolsaRosario = configuracion.ApiKeyBolsaRosario,
                SecretBolsaRosario = configuracion.SecretBolsaRosario,
                MinutosCronometroConDescarga = configuracion.MinutosCronometroConDescarga,
                CantidadMaximaDiasNegocioConDescarga = configuracion.CantidadMaximaDiasNegocioConDescarga,
                PorcentajeVolumenNegocioConDescarga = configuracion.PorcentajeVolumenNegocioConDescarga,
                ExigirNegocioEnSolExt_Soja = configuracion.ExigirNegocioEnSolExt_Soja,
                ExigirNegocioEnSolExt_Maiz = configuracion.ExigirNegocioEnSolExt_Maiz,
                ExigirNegocioEnSolExt_Trigo = configuracion.ExigirNegocioEnSolExt_Trigo,
                ExigirNegocioEnSolExt_Girasol = configuracion.ExigirNegocioEnSolExt_Girasol,
                ApiKeyScoringCupos = configuracion.ApiKeyScoringCupos,

            };

            return entidad;
        }
    }
}