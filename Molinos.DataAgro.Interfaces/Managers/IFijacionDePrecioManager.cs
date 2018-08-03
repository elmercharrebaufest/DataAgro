using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IFijacionDePrecioManager
    {
        DatosIniAbmFijacionDePrecio TraerDatosIniciales();

        ResultIniFijacionDePrecio TraerTodoFijacionDePrecio();

        FijacionDePrecio TraerFijacionDePrecio(int intFijacionId);

        Resultado GrabarFijacionDePrecio(FijacionDePrecio oFijacionDePrecio, string idActiveDirectory);

        Resultado EliminarFijacionDePrecio(int intFijacionId);
        
    }
}


