using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICriterioCDWarrantAgent
    {
        List<BasicoContrato> ConsultarContratoWarrant(DateTime desde, DateTime hasta);
    }
}