using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces.Managers
{
    public interface IRolManager
    {
        List<RolPermisoDto> TraerRolPermiso(int id);        
        Resultado GuardarRol(RolDto oRol);
        List<PermisosDataAgro> ObtenerPermisos(int id);
        Resultado EliminarRol(int id);       
        RolDto TraerRol(int id);
        IList<RolDto> TraerRolesPermisos();

        List<RolDto> TraerTodoRoles();



    }
}
