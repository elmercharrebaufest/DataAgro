using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
using System.ServiceModel;

namespace WebDataAgro.Services
{
    [ServiceContract]
    public interface IDataAgroCBServices
    {
        [OperationContract]
        Resultado RegistrarDatosPreCertificacion(ControlDeBoletosPreCertificacionServiceDto controlDeBoletosPreCertificacion);

        [OperationContract]
        Resultado RegistrarDatosSeguimiento(ControlDeBoletosDatosSeguimientoServiceDto controlDeBoletosDatosSeguimiento);
    }
}
