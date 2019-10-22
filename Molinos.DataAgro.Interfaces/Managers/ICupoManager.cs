using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICupoManager
    {
        Resultado GrabarCupo(Cupo cupo,int cantidadCupos);
        KendoGrid<CupoDto> TraerCuposTabla(KendoGridMvcRequest request, List<int> equipo);
        Resultado EliminarCupo(int id, string comercial);
        //Task ObtenerToken();
        Resultado Validar(Cupo cupo, int cantidadCupos);
        void TransmitirCupos();
        Resultado TransmitirCupos(List<string> cupos);
        List<RespuestaCupoStop> ConsultarCuposDiarios();
        CupoDto ObtenerCupo(int id);
    }
}
