using NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;

namespace Molinos.DataAgro.Business.Managers
{
    public class PrecioPizarraManager : IPrecioPizarraManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IPrecioPizarraAgent precioPizarraAgent;
        private readonly IClienteBolsaRosarioAPIAgent clienteBolsaRosarioAPIAgent;
        private readonly IDiasHabilesAgent diasHabilesAgent;

        public PrecioPizarraManager(IRepositorio repositorio, ILogger logger, IPrecioPizarraAgent precioPizarraAgent, IClienteBolsaRosarioAPIAgent clienteBolsaRosarioAPIAgent,
            IDiasHabilesAgent diasHabilesAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.precioPizarraAgent = precioPizarraAgent;
            this.clienteBolsaRosarioAPIAgent = clienteBolsaRosarioAPIAgent;
            this.diasHabilesAgent = diasHabilesAgent;
        }

        public Resultado GrabarPrecioPizarra(PrecioPizarra precioPizarra, bool manual)
        {
            var oEntityErrors = ValidarPrecioPizarra(precioPizarra, manual);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            if (precioPizarra.Id == 0)
            {
                try
                {
                    string resultado = precioPizarraAgent.Crear(precioPizarra);
                    if (resultado != "OK")
                    {
                        oEntityErrors.Error("PrecioPizarraSAP", resultado);
                    }
                }
                catch (Exception e)
                {
                    oEntityErrors.Error("PrecioPizarraSAP", e.Message);
                }
                if (oEntityErrors.HayError)
                {
                    return oEntityErrors;
                }
            }

            try
            {
                repositorio.Agregar(precioPizarra);
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            if (!oEntityErrors.HayError)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(200, "El precio se guardó correctamente."));
            }
            return oEntityErrors;
        }

        private Resultado ValidarPrecioPizarra(PrecioPizarra precioPizarra, bool manual)
        {
            var error = new Resultado();
            if (precioPizarra.MaterialId == 0) error.Errores.Add(new ErrorMessage(400, "El campo Cultivo no puede estar vacío"));
            if (string.IsNullOrEmpty(precioPizarra.MonedaId)) error.Errores.Add(new ErrorMessage(400, "El campo Moneda no puede estar vacío"));
            if (precioPizarra.FechaHasta.CompareTo(precioPizarra.FechaDesde) == -1) error.Errores.Add(new ErrorMessage(400, "El campo Fecha Hasta no puede ser menor que el campo Fecha Desde"));

            if (manual != true)
            {
                var precioMayorHasta = repositorio.ObtenerMayor<PrecioPizarra, DateTime>(x => x.MaterialId == precioPizarra.MaterialId, x => x.FechaHasta);
                if (precioMayorHasta != null && precioMayorHasta.FechaHasta >= precioPizarra.FechaDesde) error.Errores.Add(new ErrorMessage(400, "El rango ingresado no puede ser menor que la fecha hasta del último registro " + precioMayorHasta.FechaHasta.ToString("dd/MM/yyyy")));
            }

            if (precioPizarra.Precio == 0) error.Errores.Add(new ErrorMessage(400, "El campo Precio no puede estar vacío"));
            if (precioPizarra.ComercialId == null) error.Errores.Add(new ErrorMessage(400, "El campo Comercial no puede estar vacío"));
            if (precioPizarra.FechaDesde.CompareTo(DateTime.Today) >= 0) error.Errores.Add(new ErrorMessage(400, "El campo Fecha Desde no puede ser igual o mayor a la fecha del día"));
            return error;
        }

        public List<PrecioPizarraDto> TraerTodoPrecioPizarra()
        {
            return repositorio.Listar<PrecioPizarra, PrecioPizarraDto>(x => new PrecioPizarraDto
            {
                Id = x.Id,
                MaterialId = x.MaterialId,
                Material = x.Material.Descripcion + "",
                PizarraId = x.PizarraId,
                Pizarra = x.Pizarra.Descripcion + "",
                FechaDesde = x.FechaDesde.ToString(),
                FechaHasta = x.FechaHasta.ToString(),
                Precio = x.Precio,
                MonedaId = x.MonedaId,
                Moneda = x.Moneda.Descripcion + "",
                UnidadMedida = x.UnidadMedida
            });
        }

        public List<PrecioPizarraDto> TraerPrecioPizarraPorMaterialYPizarra(int materialId, int pizarraId)
        {
            return repositorio.Listar<PrecioPizarra, PrecioPizarraDto>(x => new PrecioPizarraDto
            {
                Id = x.Id,
                MaterialId = x.MaterialId,
                Material = x.Material.Descripcion + "",
                PizarraId = x.PizarraId,
                Pizarra = x.Pizarra.Descripcion + "",
                FechaDesde = SqlFunctions.DateName("day", x.FechaDesde).Trim() + "/" +
                                           SqlFunctions.StringConvert((double)x.FechaDesde.Month).TrimStart() + "/" +
                                           SqlFunctions.DateName("year", x.FechaDesde),
                FechaHasta = SqlFunctions.DateName("day", x.FechaHasta).Trim() + "/" +
                                           SqlFunctions.StringConvert((double)x.FechaHasta.Month).TrimStart() + "/" +
                                           SqlFunctions.DateName("year", x.FechaHasta),
                Precio = x.Precio,
                MonedaId = x.MonedaId,
                Moneda = x.Moneda.Descripcion + "",
                UnidadMedida = x.UnidadMedida ?? "TON",
                Fecha = x.FechaHasta

            }, x => x.MaterialId == materialId && x.PizarraId == pizarraId, 0, "Fecha", Entities.Helpers.DirOrden.Desc);
        }

        public List<MonedaDto> TraerTodoMoneda()
        {
            return repositorio.Listar<Moneda, MonedaDto>(x => new MonedaDto
            {
                MonedaId = x.MonedaId,
                Descripcion = x.Descripcion

            });
        }

        public Resultado EliminarPizarra(int id)
        {
            var result = new Resultado();
            var precioPizarra = repositorio.Obtener<PrecioPizarra>(x => x.Id == id);
            try
            {
                string resultado = precioPizarraAgent.Anular(precioPizarra);
                if (resultado != "OK")
                {
                    result.Error("PrecioPizarraSAP", resultado);
                }
            }
            catch (Exception e)
            {
                result.Error("PrecioPizarraSAP", e.Message);
            }
            if (result.HayError)
            {
                return result;
            }

            try
            {
                repositorio.Remover<PrecioPizarra>(id);
                repositorio.GuardarCambios();
                result.Errores.Add(new ErrorMessage(200, "El precio se eliminó correctamente."));
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                result.Error("", e.Message);
            }
            return result;
        }

        public PrecioPizarraDto TraerPrecioPizarraPorId(int id)
        {
            return repositorio.Obtener<PrecioPizarra, PrecioPizarraDto>(x => x.Id == id, x => new PrecioPizarraDto
            {
                Id = x.Id,
                MaterialId = x.MaterialId,
                PizarraId = x.PizarraId
            });
        }

        private List<PrecioPizarraDto> TraerPrecioPizarraPorFecha(DateTime fecha)
        {
            return repositorio.Listar<PrecioPizarra, PrecioPizarraDto>(x => new PrecioPizarraDto
            {
                Id = x.Id,
                MaterialId = x.MaterialId,
                PizarraId = x.PizarraId,
                MonedaId = x.MonedaId,
                Precio = x.Precio,
                UnidadMedida = x.UnidadMedida
            }, x => x.FechaDesde == fecha);
        }

        public void ActualizarPrecioPizarra(DateTime fecha, bool manual)
        {
            List<DataBCR> listaPreciosBCR;
            List<int> listIdMaterialesBCR = new List<int>();
            List<int> precioPizarraFiltrado = new List<int>();
            List<PrecioPizarra> precioPizarraFiltrado2;

            DateTime fechaParam = manual == true ? fecha : DateTime.Now.Date;

            var diaHabilAnterior = diasHabilesAgent.UltimoDiaHabil(fechaParam);

            string activeCreador = PermisosHelper.ObtenerUsuario();
            Comercial oComercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == activeCreador);
            var listMateriales = repositorio.Listar<Material, int>(x => x.MaterialId, y => y.MaterialId != (int)EnumMateriales.GIRASOL_AO);

            if (!manual)
            {

                precioPizarraFiltrado = repositorio.Listar<PrecioPizarra>(x => x.MaterialId != (int)EnumMateriales.GIRASOL_AO &&
                                                                               x.PizarraId == 1 &&
                                                                               x.FechaDesde == diaHabilAnterior).Select(x => x.MaterialId).ToList();
            }

            listMateriales = listMateriales.Where(x => !precioPizarraFiltrado.Contains(x)).ToList();

            // TRIGO PAN(1), MAÍZ(2), SOJA(21), SORGO(3), GIRASOL(20)
            foreach (var p in listMateriales)
            {
                listIdMaterialesBCR.Add(p == (int)EnumMateriales.MAIZ ? 2 : p == (int)EnumMateriales.TRIGO ? 1 : p == (int)EnumMateriales.SOJA ? 21 : p == (int)EnumMateriales.SORGO ? 3 : 20);
            }

            listaPreciosBCR = clienteBolsaRosarioAPIAgent.ConsultarPrecios(diaHabilAnterior, listIdMaterialesBCR.ToArray());
            List<int> idsMaterialesNoGuardar = new List<int>();
            if (manual)
            {
                foreach (var lpBCR in listaPreciosBCR)
                {
                    List<PrecioPizarra> precioPizarraFiltrado1 = repositorio.Listar<PrecioPizarra>(x => x.MaterialId == lpBCR.id_MaterialDA &&
                                                                                                        x.PizarraId == 1 &&
                                                                                                        x.FechaDesde == diaHabilAnterior &&
                                                                                                        x.Precio == (int)Math.Round(lpBCR.precio_Cotizacion)).ToList();
                    if (precioPizarraFiltrado1.Count > 0)
                    {
                        idsMaterialesNoGuardar.Add(lpBCR.id_MaterialDA);
                        continue;
                    }

                    precioPizarraFiltrado2 = repositorio.Listar<PrecioPizarra>(x => x.MaterialId == lpBCR.id_MaterialDA &&
                                                                                    x.PizarraId == 1 &&
                                                                                    x.FechaDesde == diaHabilAnterior &&
                                                                                    x.Precio != (int)Math.Round(lpBCR.precio_Cotizacion)).ToList();

                    if (precioPizarraFiltrado2.Count > 0)
                    {
                        foreach (var ppf2 in precioPizarraFiltrado2)
                        {
                            var rta = EliminarPizarra(ppf2.Id);
                        }
                    }
                }
            }

            listaPreciosBCR = listaPreciosBCR.Where(x => !idsMaterialesNoGuardar.Contains(x.id_MaterialDA)).ToList();

            Moneda oMoneda = repositorio.Obtener<Moneda>(x => x.Descripcion.Contains("ARP"));
            Pizarra oPizarra = repositorio.Obtener<Pizarra>(x => x.Descripcion.Contains("ROSARIO"));

            foreach (var lp in listaPreciosBCR)
            {
                PrecioPizarra pp = new PrecioPizarra
                {
                    Precio = (int)Math.Round(lp.precio_Cotizacion),
                    MaterialId = lp.id_MaterialDA,
                    PizarraId = oPizarra.Id,
                    FechaDesde = lp.fecha_Operacion_Pizarra,
                    FechaHasta = lp.fecha_Operacion_Pizarra,
                    MonedaId = oMoneda.MonedaId,
                    UnidadMedida = "TON",
                    ComercialId = oComercial != null ? oComercial.ComercialId : repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == "DATAAGRO").ComercialId
                };

                Resultado oEntityErrors = GrabarPrecioPizarra(pp, manual);
            }

            CompletarPrecioPizarraEnNegocios(diaHabilAnterior);
        }

        public void CompletarPrecioPizarraEnNegocios(DateTime fecha)
        {
            var negocios = repositorio.Listar<Contrato>(x => x.Pizarra == true && x.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO && x.Precio == 0).Where(x => x.FechaOperacion == fecha);

            if (negocios.Any())
            {
                var preciosPizarra = TraerPrecioPizarraPorFecha(fecha);
                if (preciosPizarra.Any())
                {
                    foreach (var negocio in negocios)
                    {
                        decimal redespacho = negocio.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho)?.Importe ?? 0;
                        decimal comisionImporte = negocio.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones)?.Importe ?? 0;
                        decimal comisionPorcentaje = (negocio.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones)?.Porcentaje ?? 0) / 100;
                        decimal bonificacionImporte = negocio.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Bonificaciones)?.Importe ?? 0;
                        decimal bonificacionPorcentaje = (negocio.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Bonificaciones)?.Porcentaje ?? 0) / 100;
                        decimal financiero = negocio.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero)?.Importe ?? 0;
                        decimal tarifaFlete = negocio.TarifaFlete ?? 0;
                        decimal precioBase = preciosPizarra.Find(x => x.MaterialId == negocio.MaterialId)?.Precio ?? 0;
                        if (comisionImporte > 0 && comisionPorcentaje > 0) comisionImporte = 0;
                        if (bonificacionImporte > 0 && bonificacionImporte > 0) bonificacionImporte = 0; //para evitar error en el cálculo siguiente tomo solo uno de los valores que deberían ser equivalentes
                        decimal precioNegocio = precioBase + redespacho + comisionImporte + (precioBase * comisionPorcentaje) + bonificacionImporte + (precioBase * bonificacionPorcentaje) + financiero - tarifaFlete;
                        negocio.Precio = precioBase;
                        negocio.PrecioNeto = precioNegocio;
                    }
                    repositorio.GuardarCambios();
                    logger.Debug($"Se completó el precio pizarra de {negocios.Count()} negocios del día {fecha:dd/MM/yyyy}.");
                }
                else logger.Debug($"No hay precios pizarra guardados en la BD para el día {fecha:dd/MM/yyyy}.");
            }
            else logger.Debug($"No hay negocios del día {fecha:dd/MM/yyyy} sin precio para completar con el precio pizarra.");
        }
    }
}