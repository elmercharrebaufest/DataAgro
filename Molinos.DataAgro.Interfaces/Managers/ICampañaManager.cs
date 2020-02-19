using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICampañaManager
    {
        CampañaHome TraerCampañaHome(int idComercial, List<int> equipo);

        List<CampañaDto> TraerCampañasActivas();

        List<MaterialDto> TraerMaterialPorCampaña(int CampañaId);

        List<CampañaDto> TraerCampañaPorMaterial(int MaterialId);

        List<CampañaDto> TraerCampañasPorGrano(int materialId);

        List<CalidadEspecialDto> TraerCalidadPorMaterial(int MaterialId);

        CampañaDto TraerCampania(int CampaniaId);

        List<CampañaDto> TraerTodoCampania();

    }
}
