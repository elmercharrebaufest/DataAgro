using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IFasonManager
    {
        GrabarFasonResult GrabarFason(Fason oFason);
        GrabarFasonResult FinalizarFason(int fijacionDePrecioContratoId);
        GrabarFasonResult BorrarFason(Fason oFason);
        BasicoContrato TraerFason(int id);
        GrabarContratoResult GrabarAmpliacionFason(Fason oFason);
        Resultado ConfirmarFason(int id);
    }
}


