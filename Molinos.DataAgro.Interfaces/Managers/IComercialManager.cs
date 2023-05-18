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
        List<ComercialCombo> ObtenerComerciales(List<int> equipo, int comercialId);
        Resultado GrabarComercial(Comercial oComercial, List<Rol> roles);
        Resultado EliminarComercial(int intComercialId);
        List<ComercialDto> ListarComercial(string comercial, List<int> comerciales);
        bool ComercialExiste(string ActiveDirectoryId);
        bool ComercialPerteneceProveedor(List<int> equipo, int proveedorId, List<int> corredoresComercial);
        EnumPerfil ObtenerPerfilDeUsuario(string activeDirectoryId);
        bool EsAdministrador(string activeDirectoryId);
        bool EsCupera(string activeDirectoryId);
        EquipoDto ListarEquipo(string idActiveDirectory);
        int ObtenerComercialId(string idActiveDirectory);
        List<int> CadenaComerciales(int comercialId);
        List<int> ListarCorredoresComercial();
        List<Comercial> ListarComercialesCorredor();
        List<GrupoDeCompras> ListarGrupoDeCompras(string filtro);
        int ComercialAsociado(int proveedorId);
        List<ComercialDto> TraerComercialesProveedor(int proveedorId);
        int TraerZonaDelComercialAsociado();
        List<Comercial> ListarComercialesSinRecibirMail();
        List<Comercial> ListarComercialesOyTNorte();
        List<ComercialQry> ListarComercialesAsignanNegocios();
    }
}