using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces.Managers
{
    public interface IResearchManager
    {
        List<ResearchAvanceSiembraDto> TraerTodoResearchAvanceSiembra();
        Resultado GrabarResearchAvanceSiembra(ResearchAvanceSiembra researchAvanceSiembra, int comercialId);
        Resultado EliminarResearchAvanceSiembra(int researchAvanceSiembraId);

        List<ResearchAvanceCosechaDto> TraerTodoResearchAvanceCosecha();
        Resultado GrabarResearchAvanceCosecha(ResearchAvanceCosecha researchAvanceCosecha, int comercialId);
        Resultado EliminarResearchAvanceCosecha(int researchAvanceCosechaId);

        List<ResearchSituacionCultivoDto> TraerTodoResearchSituacionCultivo();
        Resultado GrabarResearchSituacionCultivo(ResearchSituacionCultivo researchSituacionCultivo, int comercialId);
        Resultado EliminarResearchSituacionCultivo(int researchSituacionCultivoId);

        List<ResearchVentaStockDto> TraerTodoResearchVentaStock();
        Resultado GrabarResearchVentaStock(ResearchVentaStock researchVentaStock, int comercialId);
        Resultado EliminarResearchVentaStock(int researchVentaStockId);

        KendoGrid<ResearchAvanceSiembraDto> TraerAvanceSiembra(KendoGridMvcRequest request);
        KendoGrid<ResearchAvanceCosechaDto> TraerAvanceCosecha(KendoGridMvcRequest request);
        KendoGrid<ResearchSituacionCultivoDto> TraerSituacionCultivoParcial(KendoGridMvcRequest request);
        KendoGrid<ResearchVentaStockDto> TraerVentaStock(KendoGridMvcRequest request);

        NotificacionResearchDto TraerNotificaciones(int id);
        List<NotificacionResearchDto> TraerTodasNotificaciones();
        List<TipoResearch> TraerResearch();
        Resultado GrabarNotificacion(NotificacionResearch notificacion);
        Resultado EliminarNotificacion(int id);

    }
}


