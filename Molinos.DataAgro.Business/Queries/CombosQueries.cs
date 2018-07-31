
using Mastersoft.Framework.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business
{
    public class CombosQueries
    {
        //--------------------------------------------------
        //  Variables Privadas
        //--------------------------------------------------

        private IUnitOfWorkAsync mobjUnitOfWork;

        //--------------------------------------------------
        //  Constructor
        //--------------------------------------------------

        public CombosQueries(IUnitOfWorkAsync oUnitOfWork)
        {
            mobjUnitOfWork = oUnitOfWork;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------
         
        public async Task<List<Provincia>> GetProvinciaComboAsync()
        {
            return await mobjUnitOfWork.Repository<Provincia>()
                                       .Queryable()
                                       .OrderBy(x => x.Nombre)
                                       .ToListAsync(); 
        }


        public async Task<List<LocalidadCombo>> GetLocalidadComboAsync()
        {
            return await mobjUnitOfWork.Repository<Localidad>()
                                       .Queryable()
                                       .OrderBy(x => x.Nombre)
                                       .Select(x => new LocalidadCombo()
                                       {
                                           LocalidadId = x.LocalidadId,
                                           Nombre = x.Nombre
                                       })
                                       .ToListAsync();
        }


        public async Task<List<Estado>> GetEstadoComboAsync()
        {
            return await mobjUnitOfWork.Repository<Estado>()
                                       .Queryable()
                                       .OrderBy(x => x.Descripcion)
                                       .ToListAsync();
        }


        public async Task<List<Segmentacion>> GetSegmentacionComboAsync()
        {
            return await mobjUnitOfWork.Repository<Segmentacion>()
                                       .Queryable()
                                       .OrderBy(x => x.Descripcion)
                                       .ToListAsync();
        }


        public async Task<List<MaterialCombo>> GetMaterialComboAsync()
        {
            return await mobjUnitOfWork.Repository<Material>()
                                       .Queryable()
                                       .OrderBy(x => x.Descripcion)
                                       .Select(x => new MaterialCombo()
                                       {
                                           MaterialId = x.MaterialId,
                                           Descripcion = x.Descripcion
                                       })
                                       .ToListAsync();
        }

        public async Task<List<TipoActividadCombo>> GetTipoActividadComboAsync()
        {
            return await mobjUnitOfWork.Repository<TipoActividad>()
                                       .Queryable()
                                       .OrderBy(x => x.Descripcion)
                                       .Select(x => new TipoActividadCombo()
                                       {
                                           TipoActividadId= x.TipoActividadId,
                                           Descripcion = x.Descripcion
                                       })
                                       .ToListAsync();
        }

        public async Task<List<ProveedorCombo>> GetProveedorPorComercialComboAsync(string activeDirectory)
        {
            var oResult = new DatosIniAgendaActividad();            

            var ComercialId = mobjUnitOfWork.Repository<Comercial>().Queryable().Where(x => x.IdActiveDirectory == activeDirectory).SingleOrDefault().ComercialId;
                       
            var proveedores = await mobjUnitOfWork.SelStoreAsync<ProveedorCombo>("DataAgro_TraerProveedorPorComercial", ComercialId).ToListAsync();

            return proveedores.OrderBy(x => x.RazonSocial).ToList();                                  
                                      
        }

        public async Task<List<ComercialCombo>> GetComercialComboAsync()
        {
            return await mobjUnitOfWork.Repository<Comercial>()
                                       .Queryable()
                                       .OrderBy(x => x.Apellido)
                                       .Select(x => new ComercialCombo()
                                       {
                                           ComercialId = x.ComercialId,
                                           Apellido = x.Apellido
                                       })
                                       .ToListAsync();
        }


        public async Task<List<ComercialCombo>> GetAbmComercialComboAsync()
        {
            try
            {
                return await mobjUnitOfWork.Repository<Comercial>()
                                           .Queryable()
                                            .Where(c => c.PerfilId != (int)EnumPerfil.Visualizador || c.PerfilId != (int)EnumPerfil.Administrativo)
                                           .OrderBy(x => x.Apellido)
                                           .Select(x => new ComercialCombo()
                                           {
                                               ComercialId = x.ComercialId,
                                               Apellido = x.Apellido + " " + x.Nombres
                                           })
                                           .ToListAsync();
            }
            catch (Exception ex)
            {
                var a = 1;
            }
            return null;
        }

        public async Task<List<CentroCombo>> GetAbmCentroComboAsync()
        {
            try
            {
                return await mobjUnitOfWork.Repository<Centro>()
                                           .Queryable()
                                           .OrderBy(x => x.Descripcion)
                                           .Select(x => new CentroCombo()
                                           {
                                               CodigoSap = x.CodigoSap,
                                               Descripcion = x.Descripcion
                                           })
                                           .ToListAsync();
            }
            catch (Exception ex)
            {
                var a = 1;
            }
            return null;
        }

        public async Task<List<Perfil>> GetPerfilComboAsync()
        {
            return await mobjUnitOfWork.Repository<Perfil>()
                                       .Queryable()
                                       .OrderBy(x => x.Descripcion)
                                       .ToListAsync();
        }


        public async Task<List<GrupoDeCompras>> GetGrupoDeComprasComboAsync()
        {
            return await mobjUnitOfWork.Repository<GrupoDeCompras>()
                                       .Queryable()
                                       .OrderBy(x => x.Descripcion)
                                       .ToListAsync();
        }

        public async Task<List<Campaña>> GetCampañaComboAsync()
        {
            return await mobjUnitOfWork.Repository<Campaña>()
                                       .Queryable()
                                       .OrderBy(x => x.Descripcion)
                                       .ToListAsync(); 
        }

        public async Task<List<ClasificacionCompraNet>> GetClasificacionComboAsync()
        {
            return await mobjUnitOfWork.Repository<ClasificacionCompraNet>()
                                       .Queryable()
                                       .OrderBy(x => x.Descripcion)
                                       .ToListAsync();
        }        
    }
}





