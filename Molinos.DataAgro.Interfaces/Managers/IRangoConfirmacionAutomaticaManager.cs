using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{

    public interface IRangoConfirmacionAutomaticaManager
    {
        DatosIniAbmRangoConfirmacionAutomatica TraerDatosIniciales();
        ResultIniRangoConfirmacionAutomatica TraerTodoRangoDisponible();
        RangoConfirmacionAutomaticaDto TraerRango(int id);
        Resultado GrabarRangoConfirmacionAutomatica(RangoConfirmacionAutomatica oRango, int comercialId);
        Resultado EliminarRangoConfirmacionAutomatica(int id);
        DataSourceResult TraerTodoRango(DataSourceRequest request);
    }
}