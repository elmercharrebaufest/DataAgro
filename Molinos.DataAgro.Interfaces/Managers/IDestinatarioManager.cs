using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IDestinatarioManager
    {        
        ResultIniDestinatario TraerTodoDestinatario();

        Destinatario TraerDestinatario(int intDestinatarioId);

        Resultado GrabarDestinatario(Destinatario oDestinatario);

        Resultado EliminarDestinatario(int intDestinatarioId);

    }
}


