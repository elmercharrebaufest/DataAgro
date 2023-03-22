using Kendo.DynamicLinq;
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

        List<BusquedaHome> DevolverProveedores(string filtroProveedor, int corredor, List<int> equipo, int? agenteCompraId);
        List<BusquedaHome> DevolverProveedoresCorredores(string filtroProveedor, bool esComisionista = false, string cuitProveedor = "", bool? validarSisa = true);

        List<BusquedaHome> DevolverProveedoresConCorredor(string filtroProveedor, string filtro);

        List<ProveedorDto> ListarProveedorTodos();
        List<ProveedorDto> ListarProveedor(string text);
        List<ProveedorDto> ListarCorredor(string text);
        List<ProveedorCorredorDto> ListarProveedorCorredor(int corredorId);
        TraerProveedorResult TraerProveedorParaCorredor(string cuit);
        Resultado EnviarEmailFijacion(FijacionDePrecioContrato oFijacionDePrecioContrato, string idActiveDirectory);
        Resultado EnviarEmail(Contrato oContrato,List<DescuentoBonificacion> objDescuento, List<Calidad> objCalidad, string idActiveDirectory, bool? eliminar);
        GrabarProveedorResult GrabarNuevoCorredor(NuevoCorredor oParam, string idActiveDirectory);
        GrabarProveedorResult UpdateCorredor(NuevoCorredor oParam, string idActiveDirectory, List<int> equipo, int comercialId);
        bool ValidarProveedorEsCorredor(int idproveedor);
        string TraerCuit(int id);
        GrabarProveedorResult GrabarRol(int id, List<Rol> roles,List<Comercial> comerciales);
        List<RolBasicoDto> TraerRolesProveedor(int id);
        bool ValidarDirecto(string cuit);
        DataSourceResult BuscarDatosProveedor(DataSourceRequest request, List<int> equipo);
        DataSourceResult BuscarDatosContacto(DataSourceRequest request, List<int> equipo);
        DataSourceResult BuscarDatosProduccion(DataSourceRequest request, List<int> equipo);
        DataSourceResult BuscarDatosAlmacenamiento(DataSourceRequest request, List<int> equipo);
        void ImportarEstablecimientos(List<CampoDetalleDto> campos, Resultado resultado);
        List<ProveedorDto> TraerProveedoresPorCuit(string cuit);
        int ObtenerIdProveedorPorCuit(string cuit);

        Proveedor ObtenerEmailProveedorPorCuit(string cuit);

        List<CampanaMaterialDetallePorMesDto> BuscarDatosTablaCompras();

        List<CompraDto> TraerTodoCompra(int proveedorId, List<int> equipo);

        DatosCompraNetDto TraerBoletoBolsa(int id);
        void EnviarMailCanje(Contrato contrato, List<DescuentoBonificacion> objDescuento, List<Calidad> objCalidad, string comercial);
        void ActualizarRazonSocial();
        bool MostrarProveedorDeshabilitado(int proveedorId);
        void EnviarMailFijacionVirtual(FijacionDePrecioContrato contrato, string comercial, bool eliminar);
        void EnviarMailPrestamoDevolucion(Contrato contrato, List<DescuentoBonificacion> objDescuento, List<Calidad> objCalidad, string comercial);
        List<ProveedorDto> ListarProveedorTodos(string proveedor);
        ResultadoAltaCampoSustentable AltaCampoSustentable(CampoDetalleTercero campo);
        void GrabarMailProveedor();
        void ActualizarProveedoresHome();
        List<ActividadExportar> ExportarActividades(HistorialActiviad oParam, string idActiveDirectory);
        ProveedorCategoriasSISA ValidarCategoriaSISA(CuitSegmentacion cuitSegmentacion);
    }
}
