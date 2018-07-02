using Molinos.DataAgro.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICampañaManager
    {
        Task<CampañaHome> TraerCampañaHomeAsync(int idComercial);

        Task<List<Campaña>> TraerCampañasActivas();

        Task<List<Material>> TraerMaterialPorCampaña(int CampañaId);

        Task<List<Campaña>> TraerCampañaPorMaterial(int MaterialId);

        Task<List<Campaña>> TraerCampañasPorGrano(int MaterialId);

        Task<Campaña> TraerCampaniaAsync(int CampaniaId);

    }
}
