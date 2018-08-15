using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IComercialManager
    {
        DatosIniAbmComercial TraerDatosIniciales();

        ResultIniComercial TraerTodoComercial();

        ComercialDto TraerComercial(int intComercialId);

        List<ComercialCombo> ObtenerComerciales(List<int> equipo);

        Resultado GrabarComercial(Comercial oComercial);

        Resultado EliminarComercial(int intComercialId);

        List<ComercialDto> ListarComercial(string comercial, List<int> comerciales);
        
        bool ComercialExiste(string ActiveDirectoryId);

        bool ComercialPerteneceProveedor(List<int> equipo, int proveedorId);

        EnumPerfil ObtenerPerfilDeUsuario(string activeDirectoryId);

        bool EsAdministrador(string activeDirectoryId);

        List<int> ListarEquipo(string idActiveDirectory);

        int ObtenerComercialId(string idActiveDirectory);
    }
}


