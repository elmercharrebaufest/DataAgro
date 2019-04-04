using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IFijacionDePrecioContratoManager
    {
        DatosIniAbmFijacionDePrecioContrato TraerDatosIniciales();

        GrabarContratoResult GrabarAmpliacionFijacion(FijacionDePrecioContrato oFijacion);

        GrabarFijacionResult GrabarFijacionDePrecio(FijacionDePrecioContrato oFijacionDePrecio);

        GrabarFijacionResult ConfirmarFijacion(int fijacionDePrecioContratoId);

        GrabarContratoResult BorrarFijacion(FijacionDePrecioContrato oContrato);

        GrabarFijacionResult FinalizarFijacion(int fijacionDePrecioContratoId, string activeDiretoryId);

        BasicoContrato TraerFijacion(int id);
        List<DatosFijacionDeContratoDto> TraerDatosFijacion(string CuitProveedor, string CuitCorredor, int materialId, string Filtro);
        List<AperturaPrecioDto> TraerAperturaDePrecioPorFijacion(int fijacionId);
    }
}


