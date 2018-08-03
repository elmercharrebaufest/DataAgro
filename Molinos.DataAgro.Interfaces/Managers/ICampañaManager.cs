using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICampañaManager
    {
        CampañaHome TraerCampañaHome(int idComercial, List<int> equipo);

        List<Campaña> TraerCampañasActivas();

        List<Material> TraerMaterialPorCampaña(int CampañaId);

        List<Campaña> TraerCampañaPorMaterial(int MaterialId);

        List<Campaña> TraerCampañasPorGrano(int materialId);

        List<CalidadEspecial> TraerCalidadPorMaterial(int MaterialId);

        Campaña TraerCampania(int CampaniaId);

    }
}
