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
    public class ObjetivoManager : IObjetivoManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public ObjetivoManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public ObjetivoHome TraerObjetivoHome(int? idComercial, List<int> equipo, int? idZona, int? idComercialLogeado)
        {
            var hoy = DateTime.Now.Date;
            var fechaCambio = new DateTime(hoy.Year, 04, 01);
            var campaña = "";
            if (hoy >= fechaCambio)
            {
                campaña = (hoy.Year - 1).ToString().Substring(2) + "-" + hoy.Year.ToString().Substring(2);
            }
            else
            {
                campaña = hoy.Year.ToString().Substring(2) + "-" + (hoy.Year + 1).ToString().Substring(2);
            }
            var campañaAñoFiscal = repositorio.Obtener<Campaña>(a => a.Descripcion == campaña);
            var listResult = new ObjetivoHome();
            if (campañaAñoFiscal == null)
            { 
                logger.Info("En tabla Campaña no existe la campaña " + campaña);
                return listResult;
            }
            
            var oComercial = repositorio.Obtener<Comercial>(x => x.ComercialId == idComercialLogeado);
            List<MaterialObjetivo> listaObjetivos = new List<MaterialObjetivo>();
            if (idComercialLogeado != null && oComercial.GrupoDeCompras.Corredor)
            {
                listaObjetivos = repositorio.Listar<ObjetivoComercial, MaterialObjetivo>(x => new MaterialObjetivo
                {
                    Id = x.Id,
                    Material = x.Material.Descripcion,
                    MaterialId = x.MaterialId,
                    Campana = x.Campana.Descripcion,
                    Toneladas = x.ToneladasObjetivos,
                    Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                    ComercialId = x.ComercialId,
                    GrupoDeComprasId = x.GrupoDeComprasId
                }, x => x.Comercial.GrupoDeComprasId == oComercial.GrupoDeComprasId && campañaAñoFiscal.CampañaId == x.CampanaId, 0, "ComercialId");
            }
            else
            {
                listaObjetivos = repositorio.Listar<ObjetivoComercial, MaterialObjetivo>(x => new MaterialObjetivo
                {
                    Id = x.Id,
                    Material = x.Material.Descripcion,
                    MaterialId = x.MaterialId,
                    Campana = x.Campana.Descripcion,
                    Toneladas = x.ToneladasObjetivos,
                    Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                    ComercialId = x.ComercialId,
                    GrupoDeComprasId = x.GrupoDeComprasId
                }, x => equipo.Contains(x.ComercialId)
                && (idComercial == null || idComercial == x.ComercialId)
                && campañaAñoFiscal.CampañaId == x.CampanaId);
                //&& (zonaId == null || zonaId == x.Comercial.GrupoDeComprasId);
            }



            var listaPorMaterial = listaObjetivos.GroupBy(x => x.Material).Select(y => new MaterialObjetivo
            {
                Campana = y.First().Campana,
                Comercial = y.First().Comercial,
                ComercialId = y.First().ComercialId,
                GrupoDeComprasId = y.First().GrupoDeComprasId,
                Id = y.First().Id,
                Material = y.First().Material,
                MaterialId = y.First().MaterialId,
                Toneladas = y.Sum(f => f.Toneladas)
            }).ToList();

            listResult.Objetivos = listaPorMaterial;
            listResult.Comerciales = listaObjetivos.GroupBy(x => x.ComercialId).Select(y => new DetalleObjetivo
            {
                ComercialId = y.Key,
                Comercial = y.First().Comercial,
                Objetivos = listaObjetivos.Where(x => x.ComercialId == y.Key).ToList(),
            }).ToList();

            return listResult;
        }
        public Resultado GuardarObjetivo(ObjetivoComercial objetivo)
        {
            var comercial = repositorio.Obtener<Comercial>(x => x.ComercialId == objetivo.ComercialId);
            objetivo.GrupoDeComprasId = comercial.GrupoDeComprasId;
            var resultado = new Resultado();
            if (objetivo.MaterialId == 0)
                resultado.Error("Material", "Debe elegir Material");
            if (objetivo.CampanaId == 0)
                resultado.Error("Campaña", "Debe elegir Campaña");
            if (objetivo.ToneladasObjetivos == 0)
                resultado.Error("Toneladas", "Las tonealadas tienen que ser mayor a 0");
            if (objetivo.ComercialId == 0)
                resultado.Error("Comercial", "Comercial Inválido");
            if (repositorio.Existe<ObjetivoComercial>(x => x.CampanaId == objetivo.CampanaId && x.MaterialId == objetivo.MaterialId && x.ComercialId == objetivo.ComercialId))
                resultado.Error("Existe", "Ya existe Objetivo para el comercial con esos datos");
            if (resultado.HayError)
                return resultado;

            repositorio.Agregar(objetivo);
            repositorio.GuardarCambios();

            return resultado;
        }
        public Resultado EliminarObjetivo(int id)
        {
            var res = new Resultado();
            try
            {
                repositorio.Remover<ObjetivoComercial>(id);
                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                res.Error("", e.Message);
            }
            return res;
        }
    }
}
