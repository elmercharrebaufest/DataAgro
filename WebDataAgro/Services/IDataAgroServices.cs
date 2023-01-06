using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace WebDataAgro.Services
{
    [ServiceContract]
    public interface IDataAgroServices
    {
        #region Servicios del ABM de Destinatarios
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
        decimal TraerTipoDeCambio(DateTime? fecha);

        [OperationContract]
        ResultadoSap ActualizarCesionContratoSAP(string contratoSAP, bool cesion);

        [OperationContract]
        ResultadoAltaCampoSustentable AltaCampoSustentable(CampoDetalleTerceroDto campo);

        [OperationContract]
        List<SISA> BuscarProveedorEnSisa(string cuit);
        #endregion
    }
}
