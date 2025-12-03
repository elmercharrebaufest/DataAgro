using NLog;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Molinos.DataAgro.Business.Managers
{
    public class RolManager : IRolManager
    {
        private readonly IRepositorio repositorio;
        private ILogger logger;
        public RolManager(IRepositorio repositorio, ILogger logger)
        {
            this.repositorio = repositorio;
            this.logger = logger;
        }
        public Resultado EliminarRol(int id)
        {
            var oEntityErrors = new Resultado();

            repositorio.Remover<Rol>(id);
            logger.Debug("Eliminando el rol:" + id);
            try
            {
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error("Eliminar", ex.Message);
            }
            return oEntityErrors;
        }
        public Resultado GuardarRol(RolDto oRol)
        {
            var oEntityErrors = new Resultado();
            oEntityErrors = Validar(oRol, oEntityErrors);           

            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            var listaPermisos = oRol.PermisosEnum;
            var rol = TransformarAEntidad(oRol);
            rol.PermisosAsociados = listaPermisos.Select(x => new RolPermiso { Permiso = x, Rol = rol }).ToList();
            if (rol.Id != 0) {
                var entidad = TraerRol(rol.Id);
                entidad.Descripcion = rol.Descripcion;
                if (entidad.PermisosAsociados != null)
                {
                    foreach (var permiso in entidad.PermisosAsociados.ToList())
                    {
                        repositorio.Remover(permiso);
                    }
                }
                else
                {
                    entidad.PermisosAsociados = new List<RolPermiso>();
                }
                foreach (var permiso in rol.PermisosAsociados)
                {

                    entidad.PermisosAsociados.Add(new RolPermiso { Permiso = permiso.Permiso, Rol = entidad });
                }
                try
                {
                    repositorio.GuardarCambios();
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    oEntityErrors.Error(ex.Source, ex.Message);
                    throw;
                }

            }
            else
            {
                repositorio.Agregar(rol);
                repositorio.GuardarCambios();
            }
           
            return oEntityErrors;
        }
        private Rol TransformarAEntidad(RolDto entidad)
        {
            return new Rol
            {
                Id = entidad.Id,
                Descripcion = entidad.Descripcion
            };
        }
        //public Resultado ModificarRol(Rol oRol, string permisos)
        //{
        //    var oEntityErrors = new Resultado();

        //    EntityValid.ValidateAll(oRol, oEntityErrors);

        //    if (oEntityErrors.HayErrores)
        //    {
        //        return oEntityErrors;
        //    }


        //    logger.Debug("Guardando el rol:" + oRol.Descripcion);

        //    return oEntityErrors;
        //}
        public Rol TraerRol(int id)
        {
            return repositorio.Obtener<Rol>(id);
        }


        public List<PermisosDataAgro> ObtenerPermisos(int id)
        {
            var rol = TraerRolPermiso(id);
            var permisos = rol != null ? rol.Select(x => x.Permisos).ToList() : new List<PermisosDataAgro>();
            return permisos;
        }

        public List<RolPermisoDto> TraerRolPermiso(int id)
        {
            return repositorio.Listar<RolPermiso, RolPermisoDto>(x => new RolPermisoDto { RolId = x.RolId, Permisos = x.Permiso }, x => x.RolId == id);
        }

        public Resultado Validar(RolDto rol, Resultado oErrorMessages)
        {
            
            if (repositorio.Existe<Rol>(e => e.Descripcion.ToLower() == rol.Descripcion.ToLower() && (e.Id != rol.Id)))
            {
                oErrorMessages.Errores.Add(new ErrorMessage(400, "Este Rol ya existe"));

            }

            if (String.IsNullOrEmpty(rol.Descripcion))
            {
                oErrorMessages.Errores.Add(new ErrorMessage(400, "Debe completar el campo Descripción"));

            }
            if (rol.PermisosEnum == null || rol.PermisosEnum.Count() == 0)
            {
                oErrorMessages.Errores.Add(new ErrorMessage(400, "Debe agregar los permisos"));
            }

                return oErrorMessages;
        }
        public IList<RolDto> TraerRolesPermisos()
        {
            return repositorio.ObtenerConsultaEscalar(new TraerRolesPermisos()).OrderBy(x => x.Descripcion).ToList();
        }

        RolDto IRolManager.TraerRol(int id)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerRol(id));
        }

        public List<RolDto> TraerTodoRoles()
        {
            return repositorio.Listar<Rol, RolDto>(x => new RolDto()
            {
                Id = x.Id,
                Descripcion = x.Descripcion,
                Disabled = false
            });
        }
    }
}
