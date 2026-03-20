using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business
{
    public class ContactoComercialAuditoriaManager : IContactoComercialAuditoriaManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public ContactoComercialAuditoriaManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        /// <summary>
        /// Obtiene todo el historial de auditoría para un ContactoComercial específico
        /// </summary>
        public List<ContactoComercialAuditoriaDto> ObtenerHistorial(int contactoComercialId)
        {
            try
            {
                var auditorias = repositorio.Listar<ContactoComercialAuditoria, ContactoComercialAuditoriaDto>(
                    x => new ContactoComercialAuditoriaDto
                    {
                        ContactoComercialAuditoriaId = x.ContactoComercialAuditoriaId,
                        ContactoComercialId = x.ContactoComercialId,
                        ProveedorId = x.ProveedorId,
                        TipoOperacion = x.TipoOperacion,
                        UsuarioModificacion = x.UsuarioModificacion,
                        FechaModificacion = x.FechaModificacion,
                        ValoresAnteriores = x.ValoresAnteriores,
                        ValoresNuevos = x.ValoresNuevos
                    },
                    x => x.ContactoComercialId == contactoComercialId)
                    .ToList();

                return auditorias
                    .AsEnumerable()
                    .Select(dto => {
                        // Create a temporary entity for GenerarResumen
                        dto.Resumen = GenerarResumen(new ContactoComercialAuditoria
                        {
                            TipoOperacion = dto.TipoOperacion,
                            UsuarioModificacion = dto.UsuarioModificacion,
                            FechaModificacion = dto.FechaModificacion
                        });
                        return dto;
                    })
                    .OrderByDescending(a => a.FechaModificacion)
                    .ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error al obtener historial de auditoría para ContactoComercialId {contactoComercialId}: {ex.Message}");
                return new List<ContactoComercialAuditoriaDto>();
            }
        }

        /// <summary>
        /// Obtiene el historial de auditoría dentro de un rango de fechas
        /// </summary>
        public List<ContactoComercialAuditoriaDto> ObtenerHistorialPorFecha(int contactoComercialId, DateTime fechaDesde, DateTime fechaHasta)
        {
            try
            {
                var auditorias = repositorio.Listar<ContactoComercialAuditoria, ContactoComercialAuditoriaDto>(
                    x => new ContactoComercialAuditoriaDto
                    {
                        ContactoComercialAuditoriaId = x.ContactoComercialAuditoriaId,
                        ContactoComercialId = x.ContactoComercialId,
                        ProveedorId = x.ProveedorId,
                        TipoOperacion = x.TipoOperacion,
                        UsuarioModificacion = x.UsuarioModificacion,
                        FechaModificacion = x.FechaModificacion,
                        ValoresAnteriores = x.ValoresAnteriores,
                        ValoresNuevos = x.ValoresNuevos
                    },
                    x => x.ContactoComercialId == contactoComercialId
                        && x.FechaModificacion >= fechaDesde
                        && x.FechaModificacion <= fechaHasta)
                    .ToList();

                return auditorias
                    .AsEnumerable()
                    .Select(dto => {
                        // Create a temporary entity for GenerarResumen
                        dto.Resumen = GenerarResumen(new ContactoComercialAuditoria
                        {
                            TipoOperacion = dto.TipoOperacion,
                            UsuarioModificacion = dto.UsuarioModificacion,
                            FechaModificacion = dto.FechaModificacion
                        });
                        return dto;
                    })
                    .OrderByDescending(a => a.FechaModificacion)
                    .ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error al obtener historial de auditoría por fecha para ContactoComercialId {contactoComercialId}: {ex.Message}");
                return new List<ContactoComercialAuditoriaDto>();
            }
        }

        /// <summary>
        /// Obtiene todos los cambios realizados en los últimos días
        /// </summary>
        public List<ContactoComercialAuditoriaDto> ObtenerCambiosRecientes(int dias = 7)
        {
            try
            {
                var fechaDesde = DateTime.UtcNow.AddDays(-dias);

                // Fetch raw data without the Resumen field
                var auditorias = repositorio.Listar<ContactoComercialAuditoria, ContactoComercialAuditoriaDto>(
                    x => new ContactoComercialAuditoriaDto
                    {
                        ContactoComercialAuditoriaId = x.ContactoComercialAuditoriaId,
                        ContactoComercialId = x.ContactoComercialId,
                        ProveedorId = x.ProveedorId,
                        TipoOperacion = x.TipoOperacion,
                        UsuarioModificacion = x.UsuarioModificacion,
                        FechaModificacion = x.FechaModificacion,
                        ValoresAnteriores = x.ValoresAnteriores,
                        ValoresNuevos = x.ValoresNuevos
                        // Don't include Resumen here
                    },
                    x => x.FechaModificacion >= fechaDesde)
                    .ToList(); // Materialize to memory FIRST

                // Now apply the custom transformation in-memory using AsEnumerable
                return auditorias
                    .AsEnumerable()
                    .Select(dto => {
                        // Create a temporary entity for GenerarResumen
                        dto.Resumen = GenerarResumen(new ContactoComercialAuditoria 
                        { 
                            TipoOperacion = dto.TipoOperacion,
                            UsuarioModificacion = dto.UsuarioModificacion,
                            FechaModificacion = dto.FechaModificacion
                        });
                        return dto;
                    })
                    .OrderByDescending(a => a.FechaModificacion)
                    .ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error al obtener cambios recientes (últimos {dias} días): {ex.Message}");
                return new List<ContactoComercialAuditoriaDto>();
            }
        }

        /// <summary>
        /// Obtiene los cambios realizados por un usuario específico
        /// </summary>
        public List<ContactoComercialAuditoriaDto> ObtenerCambiosPorUsuario(string usuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuario))
                {
                    logger.Warn("ObtenerCambiosPorUsuario: usuario vacío o nulo");
                    return new List<ContactoComercialAuditoriaDto>();
                }

                var auditorias = repositorio.Listar<ContactoComercialAuditoria, ContactoComercialAuditoriaDto>(
                    x => new ContactoComercialAuditoriaDto
                    {
                        ContactoComercialAuditoriaId = x.ContactoComercialAuditoriaId,
                        ContactoComercialId = x.ContactoComercialId,
                        ProveedorId = x.ProveedorId,
                        TipoOperacion = x.TipoOperacion,
                        UsuarioModificacion = x.UsuarioModificacion,
                        FechaModificacion = x.FechaModificacion,
                        ValoresAnteriores = x.ValoresAnteriores,
                        ValoresNuevos = x.ValoresNuevos
                    },
                    x => x.UsuarioModificacion.Contains(usuario))
                    .ToList();

                logger.Debug($"Se obtuvieron {auditorias.Count} cambios realizados por el usuario: {usuario}");

                return auditorias
                    .AsEnumerable()
                    .Select(dto => {
                        dto.Resumen = GenerarResumen(new ContactoComercialAuditoria
                        {
                            TipoOperacion = dto.TipoOperacion,
                            UsuarioModificacion = dto.UsuarioModificacion,
                            FechaModificacion = dto.FechaModificacion
                        });
                        return dto;
                    })
                    .OrderByDescending(a => a.FechaModificacion)
                    .ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error al obtener cambios por usuario {usuario}: {ex.Message}");
                return new List<ContactoComercialAuditoriaDto>();
            }
        }

        /// <summary>
        /// Obtiene el último cambio realizado a un ContactoComercial
        /// </summary>
        public ContactoComercialAuditoriaDto ObtenerUltimoCambio(int contactoComercialId)
        {
            try
            {
                var auditoria = repositorio.Obtener<ContactoComercialAuditoria>(x => x.ContactoComercialId == contactoComercialId);

                if (auditoria == null)
                {
                    return null;
                }

                return new ContactoComercialAuditoriaDto
                {
                    ContactoComercialAuditoriaId = auditoria.ContactoComercialAuditoriaId,
                    ContactoComercialId = auditoria.ContactoComercialId,
                    ProveedorId = auditoria.ProveedorId,
                    TipoOperacion = auditoria.TipoOperacion,
                    UsuarioModificacion = auditoria.UsuarioModificacion,
                    FechaModificacion = auditoria.FechaModificacion,
                    ValoresAnteriores = auditoria.ValoresAnteriores,
                    ValoresNuevos = auditoria.ValoresNuevos,
                    Resumen = GenerarResumen(auditoria)
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error al obtener último cambio para ContactoComercialId {contactoComercialId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtiene resumen de cambios por tipo de operación
        /// </summary>
        public Dictionary<string, int> ObtenerResumenPorTipoOperacion(int contactoComercialId)
        {
            try
            {
                var auditorias = repositorio.Listar<ContactoComercialAuditoria>(
                    x => x.ContactoComercialId == contactoComercialId);

                return auditorias
                    .GroupBy(x => x.TipoOperacion)
                    .ToDictionary(g => g.Key, g => g.Count());
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error al obtener resumen por tipo de operación para ContactoComercialId {contactoComercialId}: {ex.Message}");
                return new Dictionary<string, int>();
            }
        }

        /// <summary>
        /// Genera un resumen legible del cambio realizado
        /// </summary>
        private string GenerarResumen(ContactoComercialAuditoria auditoria)
        {
            try
            {
                switch (auditoria.TipoOperacion)
                {
                    case "INSERT":
                        return $"Nuevo contacto creado por {auditoria.UsuarioModificacion} el {auditoria.FechaModificacion:dd/MM/yyyy HH:mm:ss}";
                    case "UPDATE":
                        return $"Contacto actualizado por {auditoria.UsuarioModificacion} el {auditoria.FechaModificacion:dd/MM/yyyy HH:mm:ss}";
                    case "DELETE":
                        return $"Contacto eliminado por {auditoria.UsuarioModificacion} el {auditoria.FechaModificacion:dd/MM/yyyy HH:mm:ss}";
                    default:
                        return $"Operación desconocida: {auditoria.TipoOperacion} realizada por {auditoria.UsuarioModificacion} el {auditoria.FechaModificacion:dd/MM/yyyy HH:mm:ss}";
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error al generar resumen de auditoría: {ex.Message}");
                return "Error al generar resumen";
            }
        }
    }
}
