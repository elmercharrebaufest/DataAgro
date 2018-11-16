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
        Resultado Ping();

        [OperationContract]
        Resultado GrabarRiesgoComercial(RiesgoComercial oRiesgos);

        [OperationContract]
        Resultado GrabarCampaniaActual(CampaniaActual oRiesgos);

        [OperationContract]
        Resultado ActualizarCampaniaMaterial(List<CampaniaMaterialSAPDTO> oCampaniaMaterialSAP);

        [OperationContract]
        Resultado ActualizarEstadoComercial(List<InformeComercialSAPDTO> oInformeComercialSAP);

        #endregion
    }
}
