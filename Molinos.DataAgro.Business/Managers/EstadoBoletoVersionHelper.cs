using Molinos.DataAgro.Entities.Dto;
using System;

namespace Molinos.DataAgro.Business.Managers
{
    internal static class EstadoBoletoVersionHelper
    {
        internal static EstadoBoletoVersionResultado Evaluar(DatosEstadoBoletoDto consultaBoleto, int? versionLocal, string estadoActual)
        {
            if (consultaBoleto == null)
            {
                return new EstadoBoletoVersionResultado
                {
                    Estado = estadoActual,
                    VersionSap = null,
                    EsVersionSapMayor = false,
                    TieneInconsistencia = false
                };
            }

            if (!int.TryParse(consultaBoleto.Version, out var versionSap))
            {
                throw new FormatException($"No se pudo parsear la versión SAP: '{consultaBoleto.Version}'.");
            }

            var versionActual = versionLocal ?? 0;
            var estado = estadoActual;
            var esVersionSapMayor = false;
            var tieneInconsistencia = false;

            if (versionSap > versionActual)
            {
                estado = "El boleto en SAP cuenta con una versión más reciente.";
                esVersionSapMayor = true;
            }
            else if (versionSap == versionActual)
            {
                if (consultaBoleto.Anulado == "X")
                {
                    estado = "Anulado";
                }
                else if (consultaBoleto.Generado == "X" && consultaBoleto.Anulado == "")
                {
                    estado = "Vigente";
                }
                else
                {
                    estado = "Pendiente";
                }
            }
            else if (versionSap == 0 && consultaBoleto.Anulado == "" && consultaBoleto.Generado == "")
            {
                estado = "Pendiente";
            }
            else
            {
                tieneInconsistencia = true;
            }

            return new EstadoBoletoVersionResultado
            {
                Estado = estado,
                VersionSap = versionSap,
                EsVersionSapMayor = esVersionSapMayor,
                TieneInconsistencia = tieneInconsistencia
            };
        }
    }

    internal class EstadoBoletoVersionResultado
    {
        public string Estado { get; set; }
        public int? VersionSap { get; set; }
        public bool EsVersionSapMayor { get; set; }
        public bool TieneInconsistencia { get; set; }
    }
}
