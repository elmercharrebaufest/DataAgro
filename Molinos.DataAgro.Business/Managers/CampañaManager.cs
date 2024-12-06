using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;

namespace Molinos.DataAgro.Business
{
    public class CampañaManager : ICampañaManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public CampañaManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public CampañaDto TraerCampania(int campaniaId)
        {
            return repositorio.Obtener<Campaña, CampañaDto>(x => x.CampañaId == campaniaId, x => new CampañaDto { CampañaId = x.CampañaId, Descripcion = x.Descripcion });
        }

        public List<CampañaDto> TraerCampañasActivas()
        {
            return repositorio.Listar<Material, CampañaDto>(x => new CampañaDto { CampañaId = x.Campaña.CampañaId, Descripcion = x.Campaña.Descripcion }, x => x.CampañaId != null);
        }

        public List<MaterialDto> TraerMaterialPorCampaña(int campanaId)
        {
            return repositorio.Listar<Material, MaterialDto>(x => new MaterialDto { CampañaId = x.CampañaId, Codigo = x.Codigo, Descripcion = x.Descripcion, MaterialId = x.MaterialId }, x => x.CampañaId == campanaId);
        }

        public CampañaHome TraerCampañaHome(int idComercial, List<int> equipo)
        {
            var listaCampaña = new CampañaHome();
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

            var listaCompras = repositorio.ListarConsulta(new TraerComprasHome(idComercial, equipo, campañaAñoFiscal));

            if (listaCompras.Count() > 5)
            {
                listaCampaña.Materiales = listaCompras.Take(4).ToList();
                listaCampaña.Materiales.Add(new MaterialCampaña()
                {
                    Nombre = "Otros",
                    Toneladas = listaCompras.Where(x => !listaCampaña.Materiales.Any(y => y.Nombre == x.Nombre)).Sum(x => x.Toneladas),
                    Campaña = string.Empty
                });
            }
            else
            {
                listaCampaña.Materiales = listaCompras.ToList();
            }

            return listaCampaña;
        }

        public List<CampañaDto> TraerCampañasPorGrano(int materialId)
        {
            return repositorio.Listar<CampañaMaterialHistorico, CampañaDto>(x => new CampañaDto { CampañaId = x.CampañaId, Descripcion = x.Campaña.Descripcion }, x => x.MaterialId == materialId, 0, "CampañaId", DirOrden.Desc);
        }

        public List<CampañaDto> TraerCampañaPorMaterial(int materialId)
        {
            var campanaActualId = repositorio.Obtener<Material, int>(x => x.MaterialId == materialId, x => x.Campaña.CampañaId);
            //&& (x.CampañaId == campanaActualId|| x.CampañaId == campanaActualId-1 || x.CampañaId == campanaActualId+1)
            var campañaConfigurada = ConfigurationManager.AppSettings["CampanaDesde"].ToString();
            var campañaEnAdelante = repositorio.Obtener<Campaña, int>(x => x.Descripcion == campañaConfigurada, x => x.CampañaId);
            return repositorio.Listar<CampañaMaterial, CampañaDto>(x => new CampañaDto { CampañaId = x.Campaña.CampañaId, Descripcion = x.Campaña.Descripcion }, x => x.MaterialId == materialId && (x.CampañaId > campañaEnAdelante), 0, "CampañaId", DirOrden.Desc);
        }

        public List<CalidadEspecialDto> TraerCalidadPorMaterial(int materialId)
        {
            var calidades = new List<CalidadEspecialDto>()
            {
                new CalidadEspecialDto { Id = 0, CodigoSap = "3", Descripcion = "Camara" }
            };
            if (materialId == 3)
            {
                calidades.Add(new CalidadEspecialDto { Id = 0, CodigoSap = "1", Descripcion = "Fabrica" });
            }
            calidades.AddRange(repositorio.Listar<CalidadEspecial, CalidadEspecialDto>(x => new CalidadEspecialDto { Id = x.Id, CodigoSap = x.CodigoSap, Descripcion = x.Descripcion, MaterialId = x.MaterialId },
                x => x.MaterialId == materialId));
            return calidades;
        }

        public List<CampañaDto> TraerTodoCampania()
        {
            return repositorio.Listar<Campaña, CampañaDto>(x => new CampañaDto
            {
                CampañaId = x.CampañaId,
                Descripcion = x.Descripcion
            });
        }
    }
}
