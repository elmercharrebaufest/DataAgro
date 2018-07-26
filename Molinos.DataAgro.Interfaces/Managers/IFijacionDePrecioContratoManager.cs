using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IFijacionDePrecioContratoManager
    {
        Task<DatosIniAbmFijacionDePrecioContrato> TraerDatosInicialesAsync();

        Task<ResultIniFijacionDePrecioContrato> TraerTodoFijacionDePrecioAsync();

        Task<FijacionDePrecioContrato> TraerFijacionDePrecioAsync(int intFijacionId);

        Task<GrabarContratoResult> GrabarAmpliacionFijacion(FijacionDePrecioContrato oFijacion);

        Task<ResultIniFijacionDePrecioContrato> TraerFijacionDePrecioContratoAsync(int ContratoId);

        Task<GrabarFijacionResult> GrabarFijacionDePrecioAsync(FijacionDePrecioContrato oFijacionDePrecio);

        Task<GrabarFijacionResult> ConfirmarFijacion(FijacionDePrecioContrato oFijacionDePrecio);

        Task<EntityErrors> EliminarFijacionDePrecioAsync(int intFijacionId);

        Task<GrabarContratoResult> BorrarFijacion(FijacionDePrecioContrato oContrato);

        Task<GrabarFijacionResult> FinalizarFijacion(FijacionDePrecioContrato oParam, string activeDiretoryId);
    }
}


