using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using KendoGridBinder.Containers;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Extensions;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class CompraNetTerceroController : Controller
    {
        private IContratoManager mobjContratoManager;
        private IComercialManager mobjComercialManager;
        private IProveedorManager mobjProveedorManager;
        private IFijacionDePrecioContratoManager mobjFijacionDePrecioContratoManager;
        private ILogger mobjLogger;
        private IConfiguracionInternaManager configuracionInternaManager;
        private ITipoDeCambioAgent tipoDeCambioAgent;

        public CompraNetTerceroController(IProveedorManager oProveedorManager, IContratoManager oContratoManager,
            IComercialManager oComercialManager, ILogger oLogger, IFijacionDePrecioContratoManager oFijacionDePrecioContratoManager, IConfiguracionInternaManager configuracionInternaManager
            , ITipoDeCambioAgent tipoDeCambioAgent)
        {
            mobjComercialManager = oComercialManager;
            mobjContratoManager = oContratoManager;
            mobjProveedorManager = oProveedorManager;
            mobjLogger = oLogger;
            mobjFijacionDePrecioContratoManager = oFijacionDePrecioContratoManager;
            this.configuracionInternaManager = configuracionInternaManager;
            this.tipoDeCambioAgent = tipoDeCambioAgent;

        }

        [Autorizacion(PermisosDataAgro.NuevoNegocioExterno)]
        public ActionResult GrabarContratoAPrecio(Contrato contrato)
        {
            contrato.Base = false;
            contrato.NoInformaSio = false;
            contrato.TrigoEspecial = false;
            contrato.EsFason = false;
            if (contrato.ComercialId == null || contrato.ComercialId == 0)
            {
                contrato.ComercialId = mobjComercialManager.ComercialAsociado(contrato.CorredorId.HasValue && contrato.CorredorId != 0 ? contrato.CorredorId.Value : contrato.ProveedorId ?? 0);
            }

            var comercial = mobjComercialManager.TraerComercial(contrato.ComercialId.Value);
            var proveedorCreador = mobjProveedorManager.TraerProveedor(contrato.ProveedorCreadorId.Value, comercial.IdActiveDirectory, new List<int>()).BasicoProveedorTraerPorProveedores.First();
            var proveedor = mobjProveedorManager.TraerProveedor(contrato.ProveedorId.Value, comercial.IdActiveDirectory, new List<int>()).BasicoProveedorTraerPorProveedores.First();
            if (contrato.CorredorId > 0)
            {
                contrato.PorcentajeComision = 1;
            }
            contrato.UsuarioId = proveedorCreador.RazonSocial;
            contrato.PrecioNeto = contrato.Precio;
            if (contrato.AperturaPrecio == null)
            {
                contrato.AperturaPrecio = new List<AperturaPrecio>();
                foreach (EnumConceptoApertura concepto in (EnumConceptoApertura[])Enum.GetValues(typeof(EnumConceptoApertura)))
                {
                    contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = (int)concepto, Importe = 0, MonedaId = null, Porcentaje = 0 });
                }
            }

            if (contrato.PagoDiferidoTercero == true)
            {
                var pago = configuracionInternaManager.TraerPagosDiferido().Where(x => x.CantidadDia <= contrato.DiasPesificado).OrderByDescending(x => x.CantidadDia).FirstOrDefault();
                if (pago == null)
                {
                    return new JsonResult() { Data = new GrabarContratoResult { Errores = new List<ErrorMessage> { new ErrorMessage { Source = "PagoDiferido", Message = "No hay una tasa de pago diferido para esa cantidad de dias." } } }, MaxJsonLength = Int32.MaxValue };
                }

                contrato.PagoDiferido = contrato.PagoDiferidoTercero;
                //var ImporteFinanciero = Math.Round(contrato.Precio * (pago.Tasa / 100) * (contrato.DiasPesificado.Value - 3) / 365 * 2, MidpointRounding.AwayFromZero) / 2;
                decimal ImporteFinanciero = Math.Round(contrato.Precio * (pago.Tasa / 100) * (contrato.DiasPesificado.Value - 3) / 365);
                ImporteFinanciero = Redondear(ImporteFinanciero);

                contrato.AperturaPrecio.First(x => x.ConceptoAperturaPrecioId == 1).Importe = ImporteFinanciero;
                contrato.PrecioNeto = contrato.Precio + ImporteFinanciero;

            }

            if ((contrato.CorredorId == null || contrato.CorredorId == 0) && proveedor.Comision > 0)
            {
                contrato.PrecioNeto = contrato.Precio + ((contrato.Precio + contrato.AperturaPrecio.First(x => x.ConceptoAperturaPrecioId == 1).Importe) * proveedor.Comision.Value / 100);
                contrato.AperturaPrecio.First(x => x.ConceptoAperturaPrecioId == 3).Porcentaje = proveedor.Comision.Value;
            }

            if (contrato.ComercialId.HasValue)
            {
                contrato.GrupoCompra = comercial.GrupoDeComprasId ?? 0;
            }
            if (contrato.StandardDeCalidadId == 2)
            {
                contrato.Calidad = new List<Calidad> { new Calidad { StandardDeCalidadId = 2, CalidadEspecialId = 4, Valor = 2 } };
            }
            if (contrato.StandardDeCalidadId == 7)
            {
                contrato.Calidad = new List<Calidad> { new Calidad { StandardDeCalidadId = 7, CalidadEspecialId = 5, Valor = 2 } };
            }
            if (contrato.MaterialId == 5)
            {
                contrato.ZonaId = 1;
            }
            else
            {
                contrato.ZonaId = null;
            }

            contrato.PorcentajeDePago = 97.5m;
            contrato.PagoDiferidoTerceroId = contrato.PagoDiferidoTerceroId == -1 ? (int?)null : contrato.PagoDiferidoTerceroId;
            return new JsonResult()
            {
                Data = mobjContratoManager.GrabarContrato(contrato),
                MaxJsonLength = Int32.MaxValue
            };
        }

        private decimal Redondear(decimal numero)
        {
            double final;
            double d10 = decimal.ToDouble(numero) / 10.00;
            final = Math.Round(d10 * 2, MidpointRounding.AwayFromZero) / 2;
            final = final * 10;
            return Convert.ToDecimal(final);
        }

        [Autorizacion(PermisosDataAgro.NuevoNegocioExterno)]
        public ActionResult GrabarContratoAFijar(Contrato contrato)
        {
            contrato.Base = false;
            contrato.NoInformaSio = false;
            contrato.TrigoEspecial = false;
            contrato.EsFason = false;

            if (contrato.ComercialId == null || contrato.ComercialId == 0)
            {
                contrato.ComercialId = mobjComercialManager.ComercialAsociado(contrato.CorredorId.HasValue && contrato.CorredorId != 0 ? contrato.CorredorId.Value : contrato.ProveedorId ?? 0);
            }
            var comercial = mobjComercialManager.TraerComercial(contrato.ComercialId.Value);
            var proveedorCreaador = mobjProveedorManager.TraerProveedor(contrato.ProveedorCreadorId.Value, comercial.IdActiveDirectory, new List<int>()).BasicoProveedorTraerPorProveedores.First();
            var proveedor = mobjProveedorManager.TraerProveedor(contrato.ProveedorCreadorId.Value, comercial.IdActiveDirectory, new List<int>()).BasicoProveedorTraerPorProveedores.First();
            contrato.UsuarioId = proveedorCreaador.RazonSocial;
            if (contrato.CorredorId > 0)
            {
                contrato.PorcentajeComision = 1;
            }
            if (contrato.ComercialId.HasValue)
            {
                contrato.GrupoCompra = comercial.GrupoDeComprasId ?? 0;
            }
            if (contrato.StandardDeCalidadId == 2)
            {
                contrato.Calidad = new List<Calidad> { new Calidad { StandardDeCalidadId = 2, CalidadEspecialId = 4, Valor = 2 } };
            }
            if (contrato.StandardDeCalidadId == 7)
            {
                contrato.Calidad = new List<Calidad> { new Calidad { StandardDeCalidadId = 7, CalidadEspecialId = 5, Valor = 2 } };
            }
            if (contrato.MaterialId == 5)
            {
                contrato.ZonaId = 1;
            }
            else
            {
                contrato.ZonaId = null;
            }

            contrato.PorcentajeDePago = 97.5m;
            return new JsonResult()
            {
                Data = mobjContratoManager.GrabarContrato(contrato),
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
            var model = new GrabarFijacionResult();
            if (contrato.ComercialId == null || contrato.ComercialId == 0)
            {
                contrato.ComercialId = mobjComercialManager.ComercialAsociado(contrato.CorredorId.HasValue && contrato.CorredorId != 0 ? contrato.CorredorId.Value : contrato.ProveedorId ?? 0);
            }
            var comercial = mobjComercialManager.TraerComercial(contrato.ComercialId.Value);
            var proveedorCreador = mobjProveedorManager.TraerProveedor(contrato.ProveedorCreadorId.Value, comercial.IdActiveDirectory, new List<int>()).BasicoProveedorTraerPorProveedores.First();
            var proveedor = mobjProveedorManager.TraerProveedor(contrato.ProveedorId.Value, comercial.IdActiveDirectory, new List<int>()).BasicoProveedorTraerPorProveedores.First();
            var cuitCorredor = "";
            if (contrato.CorredorId > 0)
            {
                cuitCorredor = mobjProveedorManager.TraerProveedor(contrato.CorredorId.Value, comercial.IdActiveDirectory, new List<int>()).BasicoProveedorTraerPorProveedores.First().CUIT;
            }
            contrato.UsuarioId = proveedorCreador.RazonSocial;

            if (contrato.AperturaPrecio == null)
            {
                contrato.AperturaPrecio = new List<AperturaPrecio>();

                foreach (EnumConceptoApertura concepto in (EnumConceptoApertura[])Enum.GetValues(typeof(EnumConceptoApertura)))
                {
                    contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = (int)concepto, Importe = 0, MonedaId = null, Porcentaje = 0 });
                }
            }

            if (contrato.PagoDiferidoTercero == true)
            {
                var pago = configuracionInternaManager.TraerPagosDiferido().Where(x => x.CantidadDia <= contrato.DiasPesificado).OrderByDescending(x => x.CantidadDia).FirstOrDefault();
                if (pago == null)
                {
                    return new JsonResult() { Data = new GrabarContratoResult { Errores = new List<ErrorMessage> { new ErrorMessage { Source = "PagoDiferido", Message = "No hay una tasa de pago diferido para esa cantidad de dias." } } }, MaxJsonLength = Int32.MaxValue };
                }
                contrato.PagoDiferido = contrato.PagoDiferidoTercero;
                decimal ImporteFinanciero = Redondear(Math.Round(contrato.Precio * (pago.Tasa / 100) * (contrato.DiasPesificado.Value - 3) / 365));
                contrato.AperturaPrecio.First(x => x.ConceptoAperturaPrecioId == 1).Importe = ImporteFinanciero;
                contrato.PrecioNeto = contrato.Precio + ImporteFinanciero;


            }

            var afijar = mobjFijacionDePrecioContratoManager.TraerDatosFijacion(proveedor.CUIT, cuitCorredor, contrato.MaterialId, contrato.ContratoSAP.Remove(0, 3), contrato.Id);
            if (afijar != null && afijar.Count > 0)
            {
                if (afijar[0].ImporteSobrePrecio > 0)
                {
                    if (afijar[0].MonedaSobrePrecio == contrato.MonedaId)
                    {
                        contrato.PrecioNeto += afijar[0].ImporteSobrePrecio;
                    }
                    else
                    {
                        var cambio = tipoDeCambioAgent.TraerTipoDeCambio(contrato.FechaOperacion);
                        if (contrato.MonedaId == "ARP  ")
                        {
                            contrato.PrecioNeto += afijar[0].ImporteSobrePrecio * cambio;
                        }
                        else
                        {
                            contrato.PrecioNeto += afijar[0].ImporteSobrePrecio / cambio;
                        }
                    }

                }

                if (afijar[0].PorcentajeSobrePrecio > 0)
                {
                    contrato.PrecioNeto += contrato.PrecioNeto * afijar[0].PorcentajeSobrePrecio / 100;
                }
            }
            contrato.PagoDiferidoTerceroId = contrato.PagoDiferidoTerceroId == -1 ? (int?)null : contrato.PagoDiferidoTerceroId;

            model = mobjFijacionDePrecioContratoManager.GrabarFijacionDePrecio(contrato);

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

    }
}