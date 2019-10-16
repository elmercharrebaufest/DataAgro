using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IClienteStopAgent
    {
        TokenStop ObtenerToken(string clave);
        void CrearCupo(List<string> listaCupos);

        void TransmitirJobCupos();
        List<RespuestaCupoStop> ConsultarCuposDiarios();
        Resultado EliminarCupo(int id);
    }
}