using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICampañaMaterial
    {
        Resultado TraerCampañasPorGrano(List<CampañaMaterialSAPDTO> oCampañaMaterialSAP);
    }
}
