using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IProveedorManager
    {
        StoredPorProveedorResult TraerProveedor(int ProveedorId, string UsuarioDirectory, List<int> equipo);

        DatosIniProveedor TraerDatosCombo(int ProveedorId);

        List<LocalidadDto> TraerLocalidad(int Id);

        DatosLocalidadProvincia TraerLocalidadProveedorPorCuit(string Cuit);

        DatosLocalidadProvincia TraerLocalidadProveedorPorCuit(DatosLocalidadProvinciaFiltro oDatosLocalidadProvinciaFiltro);

        ProveedorNuevo TraerRazonSocial(string cuit);

        Resultado GrabarRecordatorio(ActividadInsetarIni oParam);

        Resultado EliminarRecordatorio(int Id);
        
        GrabarProveedorResult GrabarNuevoProveedor(NuevoProveedor oParam, string idActiveDirectory);

        StoredHistorialResult TraerHistorialActividad(HistorialActiviad oParam, int ProveedorId, string ActividadId);

        GrabarProveedorResult UpdateProveedor(NuevoProveedor oParam, string idActiveDirectory);

        GrabarProveedorResult UpdateDatosBasicosProveedor(NuevoProveedor oParam, string idActiveDirectory);

        List<ReporteProveedor> ObtenerReporteProveedor(string Valor, string idActiveDirectory);

        ProveedorQry TraerProveedorPorCuit(string cuit);

        ProveedorDto TraerProveedor(int? proveedorId);

        List<BusquedaHome> DevolverProveedores(string filtro);
        List<ProveedorDto> ListarProveedor(string text);

        void EnviarEmailFijacion(FijacionDePrecioContrato oFijacionDePrecioContrato, string idActiveDirectory);

        void EnviarEmail(Contrato oContrato, string idActiveDirectory);
    }
}
