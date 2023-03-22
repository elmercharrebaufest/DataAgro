using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IHedgeManager
    {
        FinDelDiaDto Dia();
        List<HedgeMaterialDto> TraerTodosHedgeMaterial();
        List<HedgeObjetivoDto> TraerTodosHedgeObjetivo();
        List<HedgeTCDto> TraerTodosHedgeTC();
        List<HedgeMargenMoliendaDto> TraerTodosHedgeMargenMolienda();
        Resultado GrabarHedgeMaterial(List<HedgeMaterial> hedgeMat, int comercialId);
        Resultado GrabarHedgeObjetivo(List<HedgeObjetivo> hedgeMat, int comercialId);
        Resultado GrabarHedgeTC(HedgeTC hedgeTC, int comercialId);
        Resultado GrabarHedgeMargenMolienda(HedgeMargenMolienda hedgeMargen, int comercialId);
        Resultado EliminarHedgeTC(int hedgeTCId);
        Resultado CerrarDia(int comercialId, byte[] archivo, string idActivedirectory, bool mail, string cuerpoMail, int diferencial);
        Resultado ReabrirDia(int comercialId, double? diferencial);
        Resultado Diferencial();
        string GenerarCuerpoMail(string observaciones);
        ReporteCompraNetModel ObtenerDatosReporte();
        void EnviarMail(int comercialId, DateTime hoy, string cuerpoMail, byte[] archivo);
        void JobCerrarDia(int comercialId, string idActiveDirectory, byte[] archivo, int diferencial);
    }
}
