using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
namespace Molinos.DataAgro.Business
{
    public class CampañaMaterialManager : ICampañaMaterial
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public CampañaMaterialManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public Resultado TraerCampañasPorGrano(List<CampañaMaterialSAPDTO> oCampañaMaterialSAP)
        {
            var oEntityErrors = new Resultado();

            foreach (var camp in oCampañaMaterialSAP)
            {
                Proveedor proveedor = null;
                if (!string.IsNullOrEmpty(camp.CUIT))
                {
                    proveedor = repositorio.Obtener<Proveedor>(x => x.CUIT == camp.CUIT);
                    if (proveedor == null)
                    {
                        oEntityErrors.Error("Proveedor", "No existe el CUIT");
                        return oEntityErrors;
                    }
                }

                Campaña campania = null;
                if (!string.IsNullOrEmpty(camp.Campaña))
                {
                    campania = repositorio.Obtener<Campaña>(x => x.Descripcion == camp.Campaña);
                    if (campania == null)
                    {
                        oEntityErrors.Error("Campaña", "No existe la Campaña");
                        return oEntityErrors;
                    }
                }

                Material material = null;
                if (!string.IsNullOrEmpty(camp.Material))
                {
                    material = repositorio.Obtener<Material>(x => x.Codigo == camp.Material);
                    if (material == null)
                    {
                        oEntityErrors.Error("Material", "No existe el Material");
                        return oEntityErrors;
                    }
                }



                string comercialaux = ConfigurationManager.AppSettings["Comercial"].ToString();
                string comercialauxiliar = ConfigurationManager.AppSettings["ComercialAuxiliar"].ToString();
                Comercial comercial = null;
                if (!string.IsNullOrEmpty(camp.Comercial))
                {
                    var auxcomercial = camp.Comercial == comercialaux ? comercialauxiliar : camp.Comercial.ToLower().Trim();

                    comercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == auxcomercial);

                    if (comercial != null)
                    {
                        oEntityErrors.Error("Comercial", "No existe el Comercial");
                        return oEntityErrors;
                    }
                }

                var campaniamaterial = ObtenerCampañaIdMaterial(proveedor, material, campania, (double)camp.Toneladas, camp.Mes, (int)camp.Año);

                GuardarMes(campaniamaterial, camp.Mes, (double)camp.Toneladas, (int)camp.Año, comercial.ComercialId);

                try
                {
                    repositorio.GuardarCambios();
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw;
                }

            }
            return oEntityErrors;
        }


        private CampañaMaterialPorMes GuardarMes(CampañaMaterial campaniamaterial, string mes, double toneladas, int anio, int comercialId)
        {
            int mesId = DevolverIdMes(mes);
            var oCampañaMaterialPorMesSave = repositorio.Obtener<CampañaMaterialPorMes>(x => x.CampañaMaterialId == campaniamaterial.CampañaMaterialId
               && x.Mes == mesId && x.Año == anio && x.ComercialId == comercialId);

            if (oCampañaMaterialPorMesSave != null)
            {
                oCampañaMaterialPorMesSave.Toneladas = toneladas;
            }
            else
            {
                oCampañaMaterialPorMesSave = repositorio.Agregar(new CampañaMaterialPorMes()
                {
                    CampañaMaterial = campaniamaterial,
                    NroItem = 1,
                    Mes = mesId,
                    Año = anio,
                    ComercialId = comercialId,
                    Toneladas = toneladas
                });
            }

            return oCampañaMaterialPorMesSave;
        }

        private CampañaMaterial ObtenerCampañaIdMaterial(Proveedor proveedor, Material Material, Campaña Campania, double? total, string mes, int anio)
        {
            int mesId = DevolverIdMes(mes);

            var campaniamaterial = repositorio.Obtener<CampañaMaterial>(x => x.Campaña.CampañaId == Campania.CampañaId && x.Proveedor.ProveedorId == proveedor.ProveedorId && x.Material.MaterialId == Material.MaterialId);

            if (campaniamaterial != null)
            {
                double totalTonelada = repositorio.Listar<CampañaMaterialPorMes>
                            (x => x.CampañaMaterial.CampañaMaterialId == campaniamaterial.CampañaMaterialId && (!(x.Mes == mesId && x.Año == anio)))
                            .Sum(x => x.Toneladas) ?? 0;
                totalTonelada += (double)total;
                campaniamaterial.ToneladasCompradas = totalTonelada;
            }
            else
            {
                campaniamaterial = repositorio.Agregar(new CampañaMaterial()
                {
                    Campaña = Campania,
                    Material = Material,
                    Proveedor = proveedor,
                    NroItem = 1,
                    ToneladasCompradas = (double)total
                });
            }
            return campaniamaterial;
        }

        private int DevolverIdMes(string mES)
        {
            switch (mES.ToLower())
            {

                case "enero":
                    return 1;
                case "febrero":
                    return 2;
                case "marzo":
                    return 3;
                case "abril":
                    return 4;
                case "mayo":
                    return 5;
                case "junio":
                    return 6;
                case "julio":
                    return 7;
                case "agosto":
                    return 8;
                case "septiembre":
                    return 9;
                case "octubre":
                    return 10;
                case "noviembre":
                    return 11;
                case "diciembre":
                    return 12;
            }
            return 1;
        }

    }

}
