using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces.Managers
{
    public interface IInformeComercialAperturaManager
    {
        InformeComercialAperturaDto TraerInformeComercialApertura(int ComercialId);
        Resultado GrabarInformeComercialApertura(InformeComercialApertura oInformeComercialApertura);
    }
}
