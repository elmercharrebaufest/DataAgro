using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces {
    public interface IProveedorManager {
        void Inicializar(MSContext oContexto);

        void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork);

        void Inicializar(MSContext oContexto, IComercialManager oComercial, ICampañaMaterial oCampañaMaterial);

        Task<StoredPorProveedorResult> TraerProveedor(int ProveedorId, string UsuarioDirectory);

        Task<DatosIniProveedor> TraerDatosCombo(int ProveedorId);

        Task<List<Localidad>> TraerLocalidad(int Id);

        Task<DatosLocalidadProvincia> TraerLocalidadProveedorPorCuitAsync(string Cuit);

        Task<DatosLocalidadProvincia> TraerLocalidadProveedorPorCuitAsync(DatosLocalidadProvinciaFiltro oDatosLocalidadProvinciaFiltro);

        Task<ProveedorNuevo> TraerRazonSocial(string cuit);

        Task<EntityErrors> GrabarRecordatorioAsync(ActividadInsetarIni oParam);

        Task<EntityErrors> EliminarRecordatorio(int Id);

        Task<List<ContactoComercial>> TraerContacto(int ProveedorId);

        Task<GrabarProveedorResult> GrabarNuevoProveedor(NuevoProveedor oParam, string idActiveDirectory);

        Task<StoredHistorialResult> TraerHistorialActividad(HistorialActiviad oParam, int ProveedorId, string ActividadId);

        Task<GrabarProveedorResult> UpdateProveedor(NuevoProveedor oParam, string idActiveDirectory);

        Task<EntityErrors> UpdateDatosBasicosProveedor(NuevoProveedor oParam, string idActiveDirectory);

        Task<List<ReporteProveedor>> ObtenerReporteProveedor(string Valor, string idActiveDirectory);

        Task<ProveedorQry> TraerProveedorPorCuit(string cuit);

        Task<Proveedor> TraerProveedor(int? proveedorId);

        Task<List<BusquedaHome>> DevolverProveedores(string filtro);
        List<Proveedor> ListarProveedor(string text);
    }
}
