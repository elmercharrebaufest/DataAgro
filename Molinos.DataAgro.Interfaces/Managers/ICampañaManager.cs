using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Molinos.DataAgro.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICampañaManager
    {
        void Inicializar(MSContext oContexto);

        void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork);

        Task<CampañaHome> TraerCampañaHomeAsync(int idComercial);

        Task<List<Campaña>> TraerCampañasActivas();

        Task<List<Material>> TraerMaterialPorCampaña(int CampañaId);

        Task<List<Campaña>> TraerCampañaPorMaterial(int MaterialId);

        Task<List<Campaña>> TraerCampañasPorGrano(int MaterialId);

        Task<Campaña> TraerCampaniaAsync(int CampaniaId);

    }
}
