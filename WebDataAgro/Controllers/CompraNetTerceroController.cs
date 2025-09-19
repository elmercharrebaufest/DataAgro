using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebDataAgro.Atributos;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class CompraNetTerceroController : Controller
    {
        private readonly IContratoManager mobjContratoManager;
        private readonly IProveedorManager mobjProveedorManager;
        private readonly IFijacionDePrecioContratoManager mobjFijacionDePrecioContratoManager;
        private readonly IConfiguracionInternaManager configuracionInternaManager;

        public CompraNetTerceroController(IProveedorManager oProveedorManager, IContratoManager oContratoManager,
            IFijacionDePrecioContratoManager oFijacionDePrecioContratoManager, IConfiguracionInternaManager configuracionInternaManager)
        {
            mobjContratoManager = oContratoManager;
            mobjProveedorManager = oProveedorManager;
            mobjFijacionDePrecioContratoManager = oFijacionDePrecioContratoManager;
            this.configuracionInternaManager = configuracionInternaManager;
        }

        [Autorizacion(PermisosDataAgro.NuevoNegocioExterno)]
        public ActionResult GrabarContratoAPrecio(Contrato contrato)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.GrabarContratoAPrecioTercero(contrato),
                MaxJsonLength = Int32.MaxValue
            };
        }

        [Autorizacion(PermisosDataAgro.NuevoNegocioExterno)]
        public ActionResult GrabarContratoAFijar(Contrato contrato)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.GrabarContratoAFijarTercero(contrato),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ValidarDirecto(string cuit)
        {
            var directo = mobjProveedorManager.ValidarDirecto(cuit);
            return new JsonResult()
            {
                Data = directo,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [Autorizacion(PermisosDataAgro.NuevoNegocioExterno)]
        public ActionResult GrabarFijacion(FijacionDePrecioContrato contrato)
        {
            var model = mobjFijacionDePrecioContratoManager.GrabarFijacionDePrecioTercero(contrato);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult HabilitarPizarra(int material, int tiponegocio)
        {
            return new JsonResult()
            {
                Data = configuracionInternaManager.HabilitarPizarraExterno(material, tiponegocio),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult HabilitarCampaña(int material)
        {
            return new JsonResult()
            {
                Data = configuracionInternaManager.HabilitarCampañaExterno(material),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerPrecioMoa(int material, int tiponegocio)
        {

            var data = configuracionInternaManager.TraerPrecioCompraNet(material, tiponegocio);
            return new JsonResult()
            {
                Data = data,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerPagosDiferido()
        {
            return new JsonResult()
            {
                Data = configuracionInternaManager.TraerPagosDiferido(),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult AnularContrato(int negocioId, string MotivoRechazo)
        {

            var data = mobjContratoManager.AnularContratoCarga(negocioId, MotivoRechazo);
            return new JsonResult()
            {
                Data = data,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult AnularFijacion(int negocioId, string MotivoRechazo)
        {

            var data = mobjFijacionDePrecioContratoManager.AnularFijacionCarga(negocioId, MotivoRechazo);
            return new JsonResult()
            {
                Data = data,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public JsonResult TraerContratosAcuerdoPorCorredor(int corredorId)
        {
            return Json(mobjContratoManager.TraerContratosAcuerdoPorCorredor(corredorId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GrabarContratoMasivo(List<BasicoContrato> contratos)
        {
            return Json(mobjContratoManager.GrabarContratoMasivo(contratos), JsonRequestBehavior.AllowGet);
        }

        public ActionResult HabilitarSustentable()
        {
            return new JsonResult()
            {
                Data = configuracionInternaManager.TraerSustentables(),
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}