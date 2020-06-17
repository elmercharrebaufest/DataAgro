using Molinos.DataAgro.Entities.Dto;
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

        #endregion
    }
}
