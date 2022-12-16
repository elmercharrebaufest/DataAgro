using Autofac.Extras.NLog;
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
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.Managers
{
    public class PrecioPizarraManager : IPrecioPizarraManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;
        private IPrecioPizarraAgent precioPizarraAgent;
        private IClienteBolsaRosarioAPIAgent clienteBolsaRosarioAPIAgent;
        public PrecioPizarraManager(IRepositorio repositorio, ILogger logger, IPrecioPizarraAgent crearPrecioPizarraAgent, IClienteBolsaRosarioAPIAgent clienteBolsaRosarioAPIAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.precioPizarraAgent = crearPrecioPizarraAgent;
            this.clienteBolsaRosarioAPIAgent = clienteBolsaRosarioAPIAgent;
        }
        public Resultado GrabarPrecioPizarra(PrecioPizarra precioPizarra)
        {
            var oEntityErrors = ValidarPrecioPizarra(precioPizarra);
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
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se guardó correctamente"));
            }
            return oEntityErrors;
        }

        private Resultado ValidarPrecioPizarra(PrecioPizarra precioPizarra)
        {
            var precioMayorHasta = repositorio.ObtenerMayor<PrecioPizarra, DateTime>(x => x.MaterialId == precioPizarra.MaterialId, x => x.FechaHasta);
            var error = new Resultado();
            if (precioPizarra.MaterialId == 0) error.Errores.Add(new ErrorMessage(400, "El campo Cultivo no puede estar vacío"));
            if (string.IsNullOrEmpty(precioPizarra.MonedaId)) error.Errores.Add(new ErrorMessage(400, "El campo Moneda no puede estar vacío"));
            if (precioPizarra.FechaHasta.CompareTo(precioPizarra.FechaDesde) == -1) error.Errores.Add(new ErrorMessage(400, "El campo Fecha Hasta no puede ser menor que el campo Fecha Desde"));
            if (precioMayorHasta != null && precioMayorHasta.FechaHasta >= precioPizarra.FechaDesde) error.Errores.Add(new ErrorMessage(400, "El rango ingresado no puede ser menor que la fecha hasta del último registro " + precioMayorHasta.FechaHasta.ToString("dd/MM/yyyy")));
            if (precioPizarra.Precio == 0) error.Errores.Add(new ErrorMessage(400, "El campo Precio no puede estar vacío"));
            if (precioPizarra.ComercialId == null) error.Errores.Add(new ErrorMessage(400, "El campo Comercial no puede estar vacío"));
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

        public List<PrecioPizarraDto> TraerTodoPrecioPizarraPorMaterialYPizarra(int materialId, int pizarraId)
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
                UnidadMedida = x.UnidadMedida != null ? x.UnidadMedida : "TON",
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
                result.Errores.Add(new ErrorMessage(200, "Se elimino correctamente"));
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

        public void ActualizarPrecioPizarra(DateTime fecha)
        {
            List<DataBCR> listaPreciosBCR;
            List<int> listIdMaterialesBCR = new List<int>();

            string activeCreador = PermisosHelper.ObtenerUsuario();
            Comercial oComercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == activeCreador);
            var listMateriales = repositorio.Listar<Material, int>(x => x.MaterialId, y => y.MaterialId != 5);

            var precioPizarraFiltrado = repositorio.Listar<PrecioPizarra>(x => x.MaterialId != 5 &&
                                                                               x.PizarraId == 1 &&
                                                                               x.FechaDesde == fecha).Select(x => x.MaterialId).ToList();

            listMateriales = listMateriales.Where(x => !precioPizarraFiltrado.Contains(x)).ToList();

            // TRIGO PAN(1), MAÍZ(2), GIRASOL(20), SOJA(21)
            foreach (var p in listMateriales)
            {
                listIdMaterialesBCR.Add(p == 1 ? 2 : p == 2 ? 1 : p == 3 ? 21 : 20);
            }

            listaPreciosBCR = clienteBolsaRosarioAPIAgent.ConsultarPrecios(fecha, listIdMaterialesBCR.ToArray());

            Moneda oMoneda = repositorio.Obtener<Moneda>(x => x.Descripcion.Contains("ARP"));
            Pizarra oPizarra = repositorio.Obtener<Pizarra>(x => x.Descripcion.Contains("ROSARIO"));

            foreach (var lp in listaPreciosBCR)
            {
                PrecioPizarra pp = new PrecioPizarra();

                pp.Precio = (int)Math.Round(lp.precio_Cotizacion);
                pp.MaterialId = lp.id_MaterialDA;
                pp.PizarraId = oPizarra.Id;
                pp.FechaDesde = lp.fecha_Operacion_Pizarra;
                pp.FechaHasta = lp.fecha_Operacion_Pizarra;
                pp.MonedaId = oMoneda.MonedaId;
                pp.UnidadMedida = "TON";
                pp.ComercialId = oComercial.ComercialId;

                Resultado oEntityErrors = GrabarPrecioPizarra(pp);
            }

        }
    }
}
