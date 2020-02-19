using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Business
{
    public interface IProcesadorCriterio
    {
        decimal Calcular(Criterio comando);
    }
    public interface IProcesadorCriterio<in TCriterio> : IProcesadorCriterio
    {
        decimal Calcular(TCriterio comando);
    }
}
