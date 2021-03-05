using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using KendoGridBinder.Containers;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Extensions;
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

        public CompraNetTerceroController(IProveedorManager oProveedorManager, IContratoManager oContratoManager,
            IComercialManager oComercialManager, ILogger oLogger, IFijacionDePrecioContratoManager oFijacionDePrecioContratoManager, IConfiguracionInternaManager configuracionInternaManager)
        {
            mobjComercialManager = oComercialManager;
            mobjContratoManager = oContratoManager;
            mobjProveedorManager = oProveedorManager;
            mobjLogger = oLogger;
            mobjFijacionDePrecioContratoManager = oFijacionDePrecioContratoManager;
            this.configuracionInternaManager = configuracionInternaManager;

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
            var proveedor = mobjProveedorManager.TraerProveedor(contrato.ProveedorCreadorId.Value, comercial.IdActiveDirectory, new List<int>()).BasicoProveedorTraerPorProveedores.First();
            contrato.UsuarioId = proveedor.RazonSocial;
            contrato.PorcentajeComision = proveedor.Comision;
            contrato.PrecioNeto = contrato.Precio;

            if (contrato.PorcentajeComision.HasValue && contrato.PorcentajeComision > 0)
            {
                contrato.PrecioNeto = contrato.Precio + (contrato.Precio * contrato.PorcentajeComision.Value / 100);
                contrato.AperturaPrecio = new List<AperturaPrecio>();
                contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 1, Importe = 0, MonedaId = null, Porcentaje = 0 });
                contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 2, Importe = 0, MonedaId = null, Porcentaje = 0 });
                contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 3, Importe = 0, MonedaId = null, Porcentaje = contrato.PorcentajeComision.Value });
                contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 4, Importe = 0, MonedaId = null, Porcentaje = 0 });
            }

            if (contrato.PagoDiferidoTerceroId.HasValue && contrato.PagoDiferidoTerceroId.Value > 0)
            {
                var pago = configuracionInternaManager.TraerPagosDiferido(contrato.TipoNegocioId, contrato.MaterialId).First(x => x.Id == contrato.PagoDiferidoTerceroId.Value);

                contrato.DiasPesificado = pago.CantidadDia;

                if(contrato.AperturaPrecio == null)
                {
                    contrato.AperturaPrecio = new List<AperturaPrecio>();
                
                    contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 1, Importe = 0, MonedaId = null, Porcentaje = 0 });
                    contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 2, Importe = 0, MonedaId = null, Porcentaje = 0 });
                    contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 3, Importe = 0, MonedaId = null, Porcentaje = 0 });
                    contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 4, Importe = 0, MonedaId = null, Porcentaje = 0 });
                }

                if (contrato.AperturaPrecio.Any(x=>x.ConceptoAperturaPrecioId == 1)){
                    contrato.AperturaPrecio.First(x => x.ConceptoAperturaPrecioId == 1).Importe += pago.Importe;
                }
                else
                {
                    contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 1, Importe = pago.Importe, MonedaId = null, Porcentaje = 0 });
                }
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
            contrato.PagoDiferido = contrato.PagoDiferidoTercero;
            contrato.PagoDiferidoTerceroId = contrato.PagoDiferidoTerceroId == -1 ? (int?)null : contrato.PagoDiferidoTerceroId;
            return new JsonResult()
            {
                Data = mobjContratoManager.GrabarContrato(contrato),
                MaxJsonLength = Int32.MaxValue
            };
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
            var proveedor = mobjProveedorManager.TraerProveedor(contrato.ProveedorCreadorId.Value, comercial.IdActiveDirectory, new List<int>()).BasicoProveedorTraerPorProveedores.First();
            contrato.UsuarioId = proveedor.RazonSocial;
            contrato.PorcentajeComision = proveedor.Comision;
            //if (contrato.PorcentajeComision.HasValue && contrato.PorcentajeComision > 0)
            //{
            //    contrato.PrecioNeto = contrato.Precio + (contrato.Precio * contrato.PorcentajeComision.Value / 100);
            //    contrato.AperturaPrecio = new List<AperturaPrecio>();
            //    contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 1, Importe = 0, MonedaId = null, Porcentaje = 0 });
            //    contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 2, Importe = 0, MonedaId = null, Porcentaje = 0 });
            //    contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 3, Importe = 0, MonedaId = null, Porcentaje = 1 });
            //    contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 4, Importe = 0, MonedaId = null, Porcentaje = 0 });
            //}
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
            var proveedor = mobjProveedorManager.TraerProveedor(contrato.ProveedorCreadorId.Value, comercial.IdActiveDirectory, new List<int>()).BasicoProveedorTraerPorProveedores.First();
            contrato.UsuarioId = proveedor.RazonSocial;

            if (contrato.PagoDiferidoTerceroId.HasValue && contrato.PagoDiferidoTerceroId.Value > 0)
            {
                var pago = configuracionInternaManager.TraerPagosDiferido(contrato.TipoNegocioId, contrato.MaterialId).First(x => x.Id == contrato.PagoDiferidoTerceroId.Value);

                contrato.DiasPesificado = pago.CantidadDia;

                if (contrato.AperturaPrecio == null)
                {
                    contrato.AperturaPrecio = new List<AperturaPrecio>();

                    contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 1, Importe = 0, MonedaId = null, Porcentaje = 0 });
                    contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 2, Importe = 0, MonedaId = null, Porcentaje = 0 });
                    contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 3, Importe = 0, MonedaId = null, Porcentaje = 0 });
                    contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 4, Importe = 0, MonedaId = null, Porcentaje = 0 });
                }

                if (contrato.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == 1))
                {
                    contrato.AperturaPrecio.First(x => x.ConceptoAperturaPrecioId == 1).Importe += pago.Importe;
                }
                else
                {
                    contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = 1, Importe = pago.Importe, MonedaId = null, Porcentaje = 0 });
                }
            }

            contrato.PagoDiferido = contrato.PagoDiferidoTercero;
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

        public ActionResult TraerPagosDiferido(int tipoNegocio, int material)
        {
            return new JsonResult()
            {
                Data = configuracionInternaManager.TraerPagosDiferido(tipoNegocio, material),
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
    }
}