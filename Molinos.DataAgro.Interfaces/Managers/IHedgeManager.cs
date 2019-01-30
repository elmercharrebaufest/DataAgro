using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IHedgeManager
    {
        FinDelDiaDto Dia();
        List<HedgeMaterialDto> TraerTodosHedgeMaterial();
        List<HedgeObjetivoDto> TraerTodosHedgeObjetivo();
        List<HedgeTCDto> TraerTodosHedgeTC();
        Resultado GrabarHedgeMaterial(List<HedgeMaterial> hedgeMat, int comercialId);
        Resultado GrabarHedgeObjetivo(List<HedgeObjetivo> hedgeMat, int comercialId);
        Resultado GrabarHedgeTC(HedgeTC hedgeTC, int comercialId);
        Resultado EliminarHedgeTC(int hedgeTCId);
        Resultado CerrarDia(int comercialId, byte[] archivo, string idActivedirectory,bool mail, string cuerpoMail);
        Resultado ReabrirDia(int comercialId, double? diferencial);
        Resultado Diferencial();
    }
}
