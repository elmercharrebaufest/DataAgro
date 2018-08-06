using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IFijacionDePrecioContratoManager
    {
        DatosIniAbmFijacionDePrecioContrato TraerDatosIniciales();

        ResultIniFijacionDePrecioContrato TraerTodoFijacionDePrecio();

        GrabarContratoResult GrabarAmpliacionFijacion(FijacionDePrecioContrato oFijacion);

        ResultIniFijacionDePrecioContrato TraerFijacionDePrecioContrato(int ContratoId);

        GrabarFijacionResult GrabarFijacionDePrecio(FijacionDePrecioContrato oFijacionDePrecio);

        GrabarFijacionResult ConfirmarFijacion(FijacionDePrecioContrato oFijacionDePrecio);

        Resultado EliminarFijacionDePrecio(int intFijacionId);

        GrabarContratoResult BorrarFijacion(FijacionDePrecioContrato oContrato);

        GrabarFijacionResult FinalizarFijacion(FijacionDePrecioContrato oParam, string activeDiretoryId);
    }
}


