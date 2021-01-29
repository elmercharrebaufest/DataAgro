using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{

    public interface IDiferencialManager
    {
        DiferencialDto TraerDiferencial();
        Resultado GrabarDiferencial(Diferencial diferencial);
        Resultado EliminarDiferencial(int id);
        Resultado ValidarComprasDiferencial(int comercialId);
    }
}