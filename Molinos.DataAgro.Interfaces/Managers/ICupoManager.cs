using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICupoManager
    {
        CupoResult GrabarCupo(Cupo cupo,int cantidadCupos, DateTime? fechaHasta);
        KendoGrid<CupoDto> TraerCuposTabla(KendoGridMvcRequest request, List<int> equipo);
        Resultado EliminarCupo(int id, string comercial);
        //Task ObtenerToken();
        Resultado Validar(Cupo cupo, int cantidadCupos, DateTime? fechaHasta);
        void TransmitirCupos();
        Resultado TransmitirCupos(List<string> cupos);
        List<RespuestaCupoStop> ConsultarCuposDiarios();
        CupoDto ObtenerCupo(int id);
    }
}
