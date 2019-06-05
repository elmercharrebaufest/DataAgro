using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IProveedorManager
    {
        StoredPorProveedorResult TraerProveedor(int ProveedorId, string UsuarioDirectory, List<int> equipo);

        DatosIniProveedor TraerDatosCombo(int ProveedorId, bool noFiltrarAdministrativo);

        List<LocalidadDto> TraerLocalidad(int Id);

        DatosLocalidadProvincia TraerLocalidadProveedorPorCuit(string Cuit);

        DatosLocalidadProvincia TraerLocalidadProveedorPorCuit(DatosLocalidadProvinciaFiltro oDatosLocalidadProvinciaFiltro);

        ProveedorNuevo TraerRazonSocial(string cuit);

        Resultado GrabarRecordatorio(ActividadInsetarIni oParam);

        Resultado EliminarRecordatorio(int Id);
        
        GrabarProveedorResult GrabarNuevoProveedor(NuevoProveedor oParam, string idActiveDirectory);

        StoredHistorialResult TraerHistorialActividad(HistorialActiviad oParam, int ProveedorId, string ActividadId);

        GrabarProveedorResult UpdateProveedor(NuevoProveedor oParam, string idActiveDirectory, List<int> equipo, int comercialId);


        List<ReporteProveedor> ObtenerReporteProveedor(string Valor, string idActiveDirectory);

        ProveedorQry TraerProveedorPorCuit(string cuit, bool corredor);

        ProveedorDto TraerProveedor(int? proveedorId);

        List<BusquedaHome> DevolverProveedores(string filtroProveedor, bool corredor, List<int> equipo);

        List<BusquedaHome> DevolverProveedoresConCorredor(string filtroProveedor, string filtro);

        List<ProveedorDto> ListarProveedor(string text);
        List<ProveedorDto> ListarCorredor(string text);
        List<ProveedorCorredorDto> ListarProveedorCorredor(int corredorId);
        TraerProveedorResult TraerProveedorParaCorredor(string cuit);
        void EnviarEmailFijacion(FijacionDePrecioContrato oFijacionDePrecioContrato, string idActiveDirectory);
        void EnviarEmail(Contrato oContrato,List<DescuentoBonificacion> objDescuento, List<Calidad> objCalidad, string idActiveDirectory, bool? eliminar);
        string GetEmailUserActiveDirectory(string UserName);        
        GrabarProveedorResult GrabarNuevoCorredor(NuevoCorredor oParam, string idActiveDirectory);
        GrabarProveedorResult UpdateCorredor(NuevoCorredor oParam, string idActiveDirectory, List<int> equipo, int comercialId);
        bool ValidarProveedorEsCorredor(int idproveedor);
        string TraerCuit(int id);
    }
}
