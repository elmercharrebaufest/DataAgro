using Molinos.DataAgro.Entities.Dto;

namespace Molinos.DataAgro.Interfaces
{
    public interface IDatoDelComercialAgent
    {
        DatosComercialAgentDto ObtenerDatosDeComercial(string Usuario);
    }
}