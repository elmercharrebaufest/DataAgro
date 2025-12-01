using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;
using WebDataAgro.Models;
using static WebDataAgro.Services.DataAgroServices;

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
        InicializarContratoDto InicializarContrato(int? tipoNegocioId = null);

        [OperationContract]
        DatosCompraNetDto ObtenerDatosCompraNet(int id);

        [OperationContract]
        List<DatosFijacionDeContratoDto> ObtenerFijacionesAutomaticas(string cuitProveedor, string cuitCorredor, int materialId, string filtro, int fijacionId, bool esVirtual = false);

        [OperationContract]
        AltaTempranaNRCODto ValidarProveedor(int proveedorId);

        [OperationContract]
        List<List<PrecioMoaCompraNetDto>> TraerPrecioMoa(int? tipoNegocioId);

        [OperationContract]
        [FaultContract(typeof(ResultadoDto))]
        BasicoContrato TraerContratoCompleto(int id, string tipo);

        [OperationContract]
        BasicoContrato TraerFijacionCompleto(int id);

        [OperationContract]
        GrabarContratoResult GrabarContratoAPrecio(Contrato contrato);

        [OperationContract]
        GrabarContratoResult GrabarContratoAFijar(Contrato contrato);

        [OperationContract]
        bool ValidarDirecto(string cuit);

        [OperationContract]
        HabilitacionPizarraDto HabilitarPizarra(int material, int tiponegocio);

        [OperationContract]
        List<HabilitacionPagoDiferidoDto> TraerPagosDiferido();

        [OperationContract]
        List<HabilitacionCampañaDto> HabilitarCampaña(int material);

        [OperationContract]
        List<PrecioMoaCompraNetDto> TraerPrecioMoaV2(int material, int tiponegocio);

        [OperationContract]
        GrabarFijacionResult GrabarFijacion(FijacionDePrecioContrato contrato);

        [OperationContract]
        List<ContratoCopiar> TraerContratosAcuerdoPorCorredor(int corredorId);

        [OperationContract]
        List<GrabarContratoResult> GrabarContratoMasivo(List<BasicoContrato> contratos);

        [OperationContract]
        Resultado AnularContrato(int negocioId, string MotivoRechazo);

        [OperationContract]
        Resultado AnularFijacion(int negocioId, string MotivoRechazo);

        [OperationContract]
        List<HabilitacionSustentableDto> HabilitarSustentable();

        [OperationContract]
        List<BusquedaHome> BuscarProveedoresConCorredor(string filtroProveedor, string filtro, int? agenteCompraId);

        [OperationContract]
        KendoDataSourceResultDto BuscaDatosTablaContrato(KendoDataSourceRequestDto filtro);

        [OperationContract]
        ResultIniMaterialModel BuscarMateriales();

        [OperationContract]
        BuscarCentroDto BuscarCentro();

        [OperationContract]
        List<CampañaDto> BuscarCampana();

        //[OperationContract]
        //KendoGridResponseDto<ConfiguracionBolsaDto> DatosConfiguracion(KendoGridRequestDto request);

        [OperationContract]
        byte[] ExcelModeloAltaMasiva();

        [OperationContract]
        List<LocalidadDto> ListarLocalidades();

        [OperationContract]
        List<PartidoDto> ListarPartidos();
    }
}