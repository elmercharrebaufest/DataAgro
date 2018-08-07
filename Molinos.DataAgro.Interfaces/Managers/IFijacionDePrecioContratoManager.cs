using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IFijacionDePrecioContratoManager
    {
        DatosIniAbmFijacionDePrecioContrato TraerDatosIniciales();

        GrabarContratoResult GrabarAmpliacionFijacion(FijacionDePrecioContrato oFijacion);

        GrabarFijacionResult GrabarFijacionDePrecio(FijacionDePrecioContrato oFijacionDePrecio);

        GrabarFijacionResult ConfirmarFijacion(FijacionDePrecioContrato oFijacionDePrecio);

        GrabarContratoResult BorrarFijacion(FijacionDePrecioContrato oContrato);

        GrabarFijacionResult FinalizarFijacion(FijacionDePrecioContrato oParam, string activeDiretoryId);
    }
}


