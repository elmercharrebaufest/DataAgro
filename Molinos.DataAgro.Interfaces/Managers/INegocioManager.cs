using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;

namespace Molinos.DataAgro.Interfaces
{
    public interface INegocioManager
    {
        Resultado OcultarEnTablero(Negocio negocio);
        void EnvioMailNegociosConDiaAnterior();
        void MigrarContratosPrimary(DateTime fecha);
        void EnvioMailNegociosAnulaYReemplaza();
        void EnviarMailErrorFinalizarNegocio(int contratoId);
    }
}