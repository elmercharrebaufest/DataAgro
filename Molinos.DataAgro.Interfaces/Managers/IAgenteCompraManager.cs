using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IAgenteCompraManager
    {
        GrabarAgenteResult GrabarAgente(AgenteCompra oAgente);
        GrabarAgenteResult FinalizarAgente(int agenteId);
        GrabarAgenteResult BorrarAgente(AgenteCompra oAgente);
        BasicoContrato TraerAgente(int contratoId);
        GrabarAgenteResult GrabarAmpliacionAgente(AgenteCompra oAgente);
        Resultado ConfirmarAgenteCompra(int id, int comercialId);
    }
}


