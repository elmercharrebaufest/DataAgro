using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface INegocioManager
    {
        Resultado OcultarEnTablero(Negocio negocio);
        void EnvioMailNegociosConDiaAnterior();
        void MigrarContratosPrimary(DateTime fecha);
        void EnvioMailNegociosAnulaYReemplaza();
        void EnviarMailErrorFinalizarNegocio(int contratoId);
        BasicoContrato TraerAcuerdo(int contratoId);
        Resultado ControlesAccesoConDescarga(Negocio oContrato);
        Cupo TransformarContratoACupo(Negocio contrato);
        Resultado ValidarAltaTemprana(Negocio oContratoAcuerdo, Proveedor proveedor);
        Resultado ValidarSinBoleto(Negocio contrato);
        double DevolverCantidadDisponible(bool? sustentableOEPA, List<Contrato> contratosPendientes, List<CcPpPendienteAplicarDto> ccppPendientes, bool sinBoleto);
    }
}