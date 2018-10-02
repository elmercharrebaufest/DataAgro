using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICampaniaMaterialManager
    {
        Resultado TraerCampañasPorGrano(List<CampaniaMaterialSAPDTO> oCampañaMaterialSAP);
    }
}
