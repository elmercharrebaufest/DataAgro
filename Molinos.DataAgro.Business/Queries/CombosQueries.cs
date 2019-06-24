using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Business
{
    public class CombosQueries
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;


        public CombosQueries(ILogger logger,IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public List<Provincia> GetProvinciaCombo()
        {
            return repositorio.Listar<Provincia>().OrderBy(x => x.Orden).ToList(); 
        }

        public List<LocalidadCombo> GetLocalidadCombo()
        {
            return repositorio.Listar<Localidad, LocalidadCombo>(x => new LocalidadCombo()
            {
                LocalidadId = x.LocalidadId,
                Nombre = x.Nombre
            }, null, 15, "Nombre");
        }
        
        public List<Estado> GetEstadoCombo()
        {
            return repositorio.Listar<Estado>().OrderBy(x => x.Descripcion).ToList();
        }

        public List<Segmentacion> GetSegmentacionCombo()
        {
            return repositorio.Listar<Segmentacion>().OrderBy(x => x.Descripcion).ToList();
        }

        public List<MaterialCombo> GetMaterialCombo()
        {
            return repositorio.Listar<Material, MaterialCombo>(x => new MaterialCombo()
            {
                MaterialId = x.MaterialId,
                Descripcion = x.Descripcion
            }, null, 15, "Descripcion");
        }

        public List<TipoActividadCombo> GetTipoActividadCombo()
        {
            return repositorio.Listar<TipoActividad, TipoActividadCombo>(x => new TipoActividadCombo()
            {
                TipoActividadId = x.TipoActividadId,
                Descripcion = x.Descripcion
            }, null, 15, "Descripcion");
        }

        public List<ProveedorCombo> GetProveedorPorComercialCombo(List<int> equipo)
        {
            return repositorio.Listar<ProveedorComercial, ProveedorCombo>(
                x => new ProveedorCombo
                {
                    ProveedorId = x.Proveedor.ProveedorId,
                    RazonSocial = x.Proveedor.RazonSocial
                }, x => equipo.Contains(x.Comercial.ComercialId));
        }

        public List<ComercialCombo> GetComercialCombo()
        {
            return repositorio.Listar<Comercial, ComercialCombo>(x => new ComercialCombo()
            {
                ComercialId = x.ComercialId,
                Apellido = x.Apellido
            }, null, 0, "Apellido");
        }


        public List<ComercialCombo> GetAbmComercialCombo()
        {
            try
            {
                return repositorio.Listar<Comercial, ComercialCombo>(x => new ComercialCombo()
                {
                    ComercialId = x.ComercialId,
                    Apellido = x.Apellido + " " + x.Nombres
                }, c => c.Perfil.PerfilId != (int)EnumPerfil.Visualizador && c.Perfil.PerfilId != (int)EnumPerfil.Administrativo, 0, "Apellido");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return null;
        }

        public List<CentroCombo> GetAbmCentroCombo()
        {
            try
            {
                return repositorio.Listar<Centro, CentroCombo>(x => new CentroCombo()
                {
                    CodigoSap = x.CodigoSap,
                    Descripcion = x.Descripcion
                }, null, 0, "Descripcion");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return null;
        }

        public List<MaterialCombo> GetAbmMaterialCombo()
        {
            try
            {
                return repositorio.Listar<Material, MaterialCombo>(x => new MaterialCombo()
                {
                    Codigo = x.Codigo,
                    Descripcion = x.Descripcion
                }, null, 0, "Descripcion");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return null;
        }

        public List<CampaniaCombo> GetAbmCampaniaCombo()
        {
            try
            {
                return repositorio.Listar<Campaña, CampaniaCombo>(x => new CampaniaCombo()
                {
                    CampaniaId = x.CampañaId,
                    Descripcion = x.Descripcion
                }, null, 0, "Descripcion");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return null;
        }

        public List<ZonaCombo> GetAbmZonaCombo()
        {
            try
            {
                return repositorio.Listar<Zona, ZonaCombo>(x => new ZonaCombo()
                {
                    CodigoSap = x.CodigoSap,
                    Descripcion = x.Descripcion
                }, null, 0, "Descripcion");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return null;
        }
        public List<OperadorCombo> GetAbmOperadorCombo()
        {
            try
            {
                return repositorio.Listar<Operador, OperadorCombo>(x => new OperadorCombo()
                {
                    Id = x.Id,
                    Descripcion = x.Descripcion
                }, null, 0, "Descripcion");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return null;
        }

        public List<ContratoAcuerdoCombo> GetAbmContratoAcuerdoCombo()
        {
            try
            {
                return repositorio.Listar<ContratoAcuerdo, ContratoAcuerdoCombo>(x => new ContratoAcuerdoCombo()
                {
                    Id = x.Id,
                }, null, 0, "Id");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return null;
        }
        public List<RangoCombo> GetAbmRangoCombo()
        {
            try
            {
                return repositorio.Listar<RangoPrecio, RangoCombo>(x => new RangoCombo()
                {
                    PrecioMinimo = x.PrecioMinimo,
                    PrecioMaximo = x.PrecioMaximo,
                    Material = x.Material.Descripcion,
                    MonedaId = x.MonedaId
                }, null, 0,"Material");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return null;
        }

        public List<Perfil> GetPerfilCombo()
        {
            return repositorio.Listar<Perfil>().OrderBy(x => x.Descripcion).ToList();
        }


        public List<GrupoDeCompras> GetGrupoDeComprasCombo()
        {
            return repositorio.Listar<GrupoDeCompras>().OrderBy(x => x.Descripcion).ToList();
        }

        public List<Campaña> GetCampañaCombo()
        {
            return repositorio.Listar<Campaña>().OrderBy(x => x.Descripcion).ToList();
        }

        public List<ClasificacionCompraNet> GetClasificacionCombo()
        {
            return repositorio.Listar<ClasificacionCompraNet>().OrderBy(x => x.Descripcion).ToList();
        }

    }
}





