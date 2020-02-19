
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;
using Molinos.DataAgro.DataContracts; 

namespace Molinos.DataAgro.ServiceHost
{
    [ServiceContract]
    public interface IDataAgroServices
    {
        #region Servicios del ABM de Destinatarios
        
        [OperationContract]
        EntityErrors GrabarRiesgoComercial(RiesgoComercial oRiesgos);

        [OperationContract]
        EntityErrors GrabarCampaniaActual(CampaniaActual oRiesgos);

        [OperationContract]
        EntityErrors ActualizarCampaniaMaterial(List<CampaniaMaterialSAPDTO> oCampaniaMaterialSAP);

        [OperationContract]
        EntityErrors ActualizarEstadoComercial(List<InformeComercialSAPDTO> oInformeComercialSAP);

        #endregion
    }
}
