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
        CupoResult GrabarCupo(Cupo cupo, List<DiaCupo> dias);
        KendoGrid<CupoDto> TraerCuposTabla(KendoGridMvcRequest request, List<int> equipo);
        Resultado EliminarCupo(int id, string comercial);
        //Task ObtenerToken();
        Resultado Validar(Cupo cupo, int cantidadCupos, DateTime? fechaHasta);
        void TransmitirCupos();
        Resultado TransmitirCupos(List<string> cupos);
        List<RespuestaCupoStop> ConsultarCuposDiarios();
        CupoDto ObtenerCupo(int id);
        Resultado EliminarVarios(List<int> cupos, string comercial);
        string ObtenerCodigoSap(int id);
        void CrearSugerenciaCupo();
        List<SugerenciaCupoDto> ObtenerSugerenciaCupo(int ComercialId);
        List<CupoResult> AceptarSugerenciaCupo(List<SugerenciaCupoDto> ids);
        CupoResult RechazarSugerenciaCupo(List<int> ids,string motivo);
    }
}
