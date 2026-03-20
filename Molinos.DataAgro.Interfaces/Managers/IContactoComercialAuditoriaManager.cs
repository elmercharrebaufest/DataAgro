using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IContactoComercialAuditoriaManager
    {
        /// <summary>
        /// Obtiene todo el historial de auditoría para un ContactoComercial específico
        /// </summary>
        List<ContactoComercialAuditoriaDto> ObtenerHistorial(int contactoComercialId);

        /// <summary>
        /// Obtiene el historial de auditoría dentro de un rango de fechas
        /// </summary>
        List<ContactoComercialAuditoriaDto> ObtenerHistorialPorFecha(int contactoComercialId, DateTime fechaDesde, DateTime fechaHasta);

        /// <summary>
        /// Obtiene todos los cambios realizados en los últimos días
        /// </summary>
        List<ContactoComercialAuditoriaDto> ObtenerCambiosRecientes(int dias = 7);

        /// <summary>
        /// Obtiene los cambios realizados por un usuario específico
        /// </summary>
        List<ContactoComercialAuditoriaDto> ObtenerCambiosPorUsuario(string usuario);

        /// <summary>
        /// Obtiene el último cambio realizado a un ContactoComercial
        /// </summary>
        ContactoComercialAuditoriaDto ObtenerUltimoCambio(int contactoComercialId);

        /// <summary>
        /// Obtiene resumen de cambios por tipo de operación
        /// </summary>
        Dictionary<string, int> ObtenerResumenPorTipoOperacion(int contactoComercialId);
    }
}
