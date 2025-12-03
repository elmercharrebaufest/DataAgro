using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Business
{
    public class CampaniaMaterialManager : ICampaniaMaterialManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public CampaniaMaterialManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public Resultado TraerCampañasPorGrano(List<CampaniaMaterialSAPDTO> oCampañaMaterialSAP)
        {

            var oEntityErrors = new Resultado();
            int? ProveedorId;
            int? MaterialId;
            int? ComercialId;
            
            foreach (var camp in oCampañaMaterialSAP)
            {

                ProveedorId = null;
                if (!string.IsNullOrEmpty(camp.CUIT))
                {
                    var proveedor = repositorio.Obtener<Proveedor>(x => x.CUIT == camp.CUIT);
                    if (proveedor != null)
                    {
                        ProveedorId = proveedor.ProveedorId;
                    }
                    else
                    {
                        oEntityErrors.Errores.Add(new ErrorMessage() { Message = "No existe el CUIT" });
                        return oEntityErrors;
                    }
                }

                int? CampañaId = null;
                if (!string.IsNullOrEmpty(camp.Campania))
                {
                    var campaña = repositorio.Obtener<Campaña>(x => x.Descripcion == camp.Campania);
                    if (campaña != null)
                    {
                        CampañaId = campaña.CampañaId;
                    }
                    else
                    {
                        oEntityErrors.Errores.Add(new ErrorMessage() { Message = "No existe la Campaña." });
                        return oEntityErrors;
                    }
                }

                MaterialId = null;
                if (!string.IsNullOrEmpty(camp.Material))
                {
                    var material = repositorio.Obtener<Material>(x => x.Codigo == camp.Material);
                    if (material != null)
                    {
                        MaterialId = material.MaterialId;
                    }
                    else
                    {
                        oEntityErrors.Errores.Add(new ErrorMessage() { Message = "No existe el Material." });
                        return oEntityErrors;
                    }
                }

                ComercialId = null;

                if (!string.IsNullOrEmpty(camp.Comercial))
                {
                    var comercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == camp.Comercial);

                    if (comercial != null)
                    {
                        ComercialId = comercial.ComercialId;
                    }
                    else
                    {
                        oEntityErrors.Errores.Add(new ErrorMessage() { Message = "No existe el comercial." });
                        return oEntityErrors;
                    }
                }                
                try
                {
                    var oCampañaMaterialSave = ObtenerCampañaIdMaterial(ProveedorId, MaterialId, CampañaId, (double)camp.Toneladas, camp.Mes, (int)camp.Anio);
                    GuardarMes(oCampañaMaterialSave.CampañaMaterialId, camp.Mes, (double)camp.Toneladas, (int)camp.Anio, (int)ComercialId);
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


        private void GuardarMes(int CampañaMaterialId, string Mes, double Toneladas, int Año, int ComercialId)
        {
            int MesId = DevolverIdMes(Mes);
            var oCampañaMaterialPorMesSave = repositorio.Obtener<CampañaMaterialPorMes>(x => x.CampañaMaterialId == CampañaMaterialId
               && x.Mes == MesId && x.Año == Año && x.ComercialId == ComercialId);

            if (oCampañaMaterialPorMesSave != null)
            {
                oCampañaMaterialPorMesSave.Toneladas = Toneladas;
            }
            else
            {
                repositorio.Agregar(new CampañaMaterialPorMes
                {
                    CampañaMaterialId = CampañaMaterialId,
                    NroItem = 1,
                    Mes = MesId,
                    Año = Año,
                    ComercialId = ComercialId,
                    Toneladas = Toneladas
                });
            }
        }

        private CampañaMaterial ObtenerCampañaIdMaterial(int? proveedorId, int? MaterialId, int? CampañaId, double? total, string Mes, int Año)
        {
            int MesId = DevolverIdMes(Mes);
            
            var campañamaterial = repositorio.Obtener<CampañaMaterial>(x => x.CampañaId == CampañaId
               && x.ProveedorId == proveedorId && x.MaterialId == MaterialId);

            if (campañamaterial != null)
            {
                var TotalTonelada = repositorio.Listar<CampañaMaterialPorMes, double>(x => x.Toneladas ?? 0, x => x.CampañaMaterialId == campañamaterial.CampañaMaterialId && (!(x.Mes == MesId && x.Año == Año))).Sum();
                TotalTonelada += (double)total;
                campañamaterial.ToneladasCompradas = (double)TotalTonelada;
                return campañamaterial;
            }
            else
            {
                return repositorio.Agregar(new CampañaMaterial()
                {
                    CampañaId = (int)CampañaId,
                    MaterialId = (int)MaterialId,
                    ProveedorId = (int)proveedorId,
                    NroItem = 1,
                    ToneladasCompradas = (double)total,
                });
            }
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
