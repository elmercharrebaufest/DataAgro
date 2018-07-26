using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICampañaMaterial
    {
           Task<EntityErrors> TraerCampañasPorGrano(List<CampañaMaterialSAPDTO> oCampañaMaterialSAP);
    }
}
