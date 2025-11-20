using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace WebDataAgro.Services
{
    [ServiceContract]
    public interface IDataAgroServices
    {
        [OperationContract]
        ResultadoSap Ping();

        [OperationContract]
        ResultadoSap GrabarRiesgoComercial(RiesgoComercial oRiesgos);

        [OperationContract]
        ResultadoSap GrabarCampaniaActual(CampaniaActual oRiesgos);

        [OperationContract]
        ResultadoSap ActualizarCampaniaMaterial(List<CampaniaMaterialSAPDTO> oCampaniaMaterialSAP);

        [OperationContract]
        ResultadoSap ActualizarEstadoComercial(List<InformeComercialSAPDTO> oInformeComercialSAP);

        [OperationContract]
        ResultadoSap ActualizarContratoSAP(ContratoSAPDto contratoSAP);

        [OperationContract]
        ResultadoSap ActualizarCupoSAP(CupoSapDto cupoSAP);

        [OperationContract]
        ResultadoSap AltaCupoSAP(CupoSapDto cupoSAP);

        [OperationContract]
        ResultadoSap AnularContratoSAP(ContratoSAP contratoSAP);

        [OperationContract]
        ResultadoValidarProveedorComercial ValidarProveedorComercial(string cuit, bool? corredor);

        [OperationContract]
        ResultadoSap AltaContratoSAP(ContratoSAPDto contratoSAP);
        [OperationContract]
        ResultadoSap ActualizarFijacionSAP(FijacionSAPDto fijacionSAP);
        [OperationContract]
        ResultadoSap AltaFijacionSAP(FijacionSAPDto fijacionSAP);
        [OperationContract]
        ResultadoSap AnularFijacionSAP(FijacionSAP fijacionSAP);
        [OperationContract]
        ResultadoSap AnulaFijacionVirtualSAP(FijacionVirtualSAP fijacionSAP);
        [OperationContract]
        bool ProveedorApocrifo(string cuit);
        [OperationContract]
        decimal TraerTipoDeCambio(DateTime? fecha, string moneda, string typeOfRate = "M");

        [OperationContract]
        ResultadoSap ActualizarCesionContratoSAP(string contratoSAP, bool cesion);

        [OperationContract]
        ResultadoAltaCampoSustentable AltaCampoSustentable(CampoDetalleTerceroDto campo);

        [OperationContract]
        List<SISA> BuscarProveedorEnSisa(string cuit);

        [OperationContract]
        ResultadoSap ConfirmarFijacionSAP(string fijacionSAP);

        [OperationContract]
        List<ApoderadoSapDto> ListarApoderadosPorProveedor(string cuit);

        [OperationContract]
        CupoSapTerceroDto DatosCupoSap(string cupoSap);

        [OperationContract]
        ResultEstadoProveedores ObtenerEstadoProveedores(List<string> listaCuits);

        [OperationContract]
        DatosIniContrato InicializarContrato(int? tipoNegocioId = null);

        [OperationContract]
        DatosCompraNetDto ObtenerDatosCompraNet(int id);

        [OperationContract]
        List<DatosFijacionDeContratoDto> ObtenerFijacionesAutomaticas(string cuitProveedor, string cuitCorredor, int materialId, string filtro, int fijacionId, bool esVirtual = false);

        [OperationContract]
        AltaTempranaNRCODto ValidarProveedor(int proveedorId);

        //[OperationContract]
        //IEnumerable<IGrouping<int, PrecioMoaCompraNetDto>> TraerPrecioMoa(int? tipoNegocioId);
    }
}
