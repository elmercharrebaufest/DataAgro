using Molinos.DataAgro.Entities.Dto;

namespace Molinos.DataAgro.Interfaces
{
    public interface IAltaTempranaAgent
    {
        AltaTempranaNRCODto ObtenerAlta(string cuit);
    }
}