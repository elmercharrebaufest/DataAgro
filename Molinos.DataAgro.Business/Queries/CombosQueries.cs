using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
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


        public CombosQueries(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public List<Provincia> GetProvinciaCombo()
        {
            return repositorio.Listar<Provincia>().OrderBy(x => x.Nombre).ToList();
        }

        public List<Partido> GetPartidoCombo(int provinciaId)
        {
            List<Partido> listPartido;

            //if(provinciaId == -1)
            //    listPartido = repositorio.Listar<Partido>().OrderBy(x => x.Descripcion).ToList();
            //else
                listPartido = repositorio.Listar<Partido>(x => x.ProvinciaId == provinciaId).OrderBy(x => x.Descripcion).ToList();

            //return repositorio.Listar<Partido>(x => x.ProvinciaId == provinciaId).OrderBy(x => x.Descripcion).ToList();
            return listPartido;
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
            }, null, 15, "MaterialId");
        }

        public List<NivelTarifaCombo> GetNivelTarifaCombo()
        {
            return repositorio.Listar<NivelTarifa, NivelTarifaCombo>(x => new NivelTarifaCombo()
            {
                Id = x.Id,
                Descripcion = x.Descripcion,
                CodigoSap = x.CodigoSap
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
            var resultado = repositorio.Listar<ProveedorComercial, ProveedorCombo>(
                x => new ProveedorCombo
                {
                    ProveedorId = x.Proveedor.ProveedorId,
                    RazonSocial = !string.IsNullOrEmpty(x.Proveedor.Alias) ? (x.Proveedor.Alias + " - " + x.Proveedor.RazonSocial) : x.Proveedor.RazonSocial,
                    Alias = x.Proveedor.Alias
                }, x => equipo.Contains(x.Comercial.ComercialId));

            var listaOrdenada = resultado.Where(x => string.IsNullOrEmpty(x.Alias)).OrderBy(x => x.RazonSocial);
            return resultado.Where(x => !string.IsNullOrEmpty(x.Alias)).OrderBy(x => x.Alias).ThenBy(x => x.RazonSocial).Concat(listaOrdenada).ToList();
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
                }, c => c.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.ListaComercial)), 0, "Apellido");
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

        public List<ZonaCupoCombo> GetAbmZonaCupo()
        {
            try
            {
                return repositorio.Listar<ZonaCupo, ZonaCupoCombo>(x => new ZonaCupoCombo()
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
                    MaterialId = x.MaterialId,
                    Codigo = x.Codigo,
                    Descripcion = x.Descripcion,
                    IVA = x.IVA
                }, null, 0, "Descripcion");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return null;
        }

        public List<MotivoCombo> GetAbmMotivoCombo()
        {
            try
            {
                return repositorio.Listar<MotivoAnterior, MotivoCombo>(x => new MotivoCombo()
                {
                    MotivoId = x.Id,
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

        public List<CampaniaTableroCombo> GetAbmCampaniaTableroCombo()
        {
            try
            {
                return repositorio.Listar<Campaña, CampaniaTableroCombo>(x => new CampaniaTableroCombo()
                {
                    CampaniaTableroId = x.CampañaId,
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
                }, null, 0, "Material");
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

        public List<RolCombo> GetRolCombo()
        {
            return repositorio.Listar<Rol, RolCombo>(x => new RolCombo { Id = x.Id, Descripcion = x.Descripcion }).OrderBy(x => x.Descripcion).ToList();
        }

    }
}