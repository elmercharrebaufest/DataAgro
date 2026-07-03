using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.Managers
{
    public class ControlDeBoletosSAPManager: IControlDeBoletosSAPManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly ILogDataAgroManager logDataAgroManager;
        private readonly IControlDeBoletosManager controlDeBoletosManager;
        public ControlDeBoletosSAPManager(ILogger logger,IRepositorio repositorio, ILogDataAgroManager logDataAgroManager, IControlDeBoletosManager controlDeBoletosManager )
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.logDataAgroManager = logDataAgroManager;
            this.controlDeBoletosManager = controlDeBoletosManager;
        }

        private static DateTime? ConvertirFechaNullable(string fecha)
        {
            if (string.IsNullOrWhiteSpace(fecha))
                return null;

            DateTime resultado;
            return DateTime.TryParseExact(
                fecha,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out resultado)
                ? resultado
                : (DateTime?)null;
        }

        #region Metodos para servicio SAP
        public Resultado RegistrarDatosPreCertificacion(ControlDeBoletosPreCertificacionServiceDto controlDeBoletosPreCertificacion)
        {
            var oResultado = new Resultado();
            try
            {
                if (controlDeBoletosPreCertificacion == null)
                {
                    oResultado.Errores.Add(new ErrorMessage { Message = "El request de pre-certificacion no puede ser nulo." });
                    return oResultado;
                }

                var negocio = repositorio.Obtener<Negocio>(x => x.ContratoSAP == controlDeBoletosPreCertificacion.ContratoSAP);
                if (negocio == null)
                {
                    oResultado.Errores.Add(new ErrorMessage { Message = "No se encontro un negocio para el ContratoSAP informado." });
                    return oResultado;
                }

                var controlDeBoletos = repositorio.Obtener<ControlDeBoletos>(x => x.NegocioId == negocio.Id);
                if (controlDeBoletos == null)
                {
                    oResultado.Errores.Add(new ErrorMessage { Message = "No se encontro el Control de Boletos asociado al negocio." });
                    return oResultado;
                }

                var controlDeBoletosId = controlDeBoletos.Id;
                var detalleRequest = controlDeBoletosPreCertificacion.Detalle?.ToList() ?? new List<ControlDeBoletosDatosPreCertificacionServiceDto>();

                // Códigos de tipo oblea presentes en el request
                var codigosEnRequest = detalleRequest
                    .Select(d => d.TipoOblea)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .ToHashSet();

                var codigosBolsaEnRequest = detalleRequest
                    .Select(d => d.Bolsa)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .ToHashSet();

                // Registros actuales en BD para este control (sin navegación para evitar conflictos FK)
                var registrosEnBd = repositorio.Listar<ControlDeBoletosPreCertificacion>(
                    x => x.ControlDeBoletosId == controlDeBoletosId).ToList();

                var registrosPorTipoObleaId = registrosEnBd
                    .GroupBy(r => r.TipoObleaId)
                    .ToDictionary(g => g.Key, g => g.First());

                var tiposObleaPorCodigo = codigosEnRequest.Any()
                    ? repositorio.Listar<TipoOblea>(t => codigosEnRequest.Contains(t.Codigo))
                        .ToDictionary(t => t.Codigo, t => t)
                    : new Dictionary<string, TipoOblea>();

                var bolsasPorCodigoSap = codigosBolsaEnRequest.Any()
                    ? repositorio.Listar<BolsaCompraNet>(b => codigosBolsaEnRequest.Contains(b.CodigoSap))
                        .ToDictionary(b => b.CodigoSap, b => b.Id)
                    : new Dictionary<string, int>();

                var ahora = DateTime.Now;

                bool hayCambiosEnBd = false;

                // ── 1. Upsert: crear o actualizar los registros que vienen en el request ──
                foreach (var item in detalleRequest)
                {
                    if (string.IsNullOrWhiteSpace(item.TipoOblea))
                    {
                        continue;
                    }

                    TipoOblea tipoOblea;
                    if (!tiposObleaPorCodigo.TryGetValue(item.TipoOblea, out tipoOblea))
                    {
                        continue;
                    }

                    var existente = registrosPorTipoObleaId.ContainsKey(tipoOblea.Id)
                        ? registrosPorTipoObleaId[tipoOblea.Id]
                        : null;
                    var fechaCertificacion = ConvertirFechaNullable(item.FechaCertificacion);
                    var fechaVencimiento = ConvertirFechaNullable(item.FechaVencimiento);

                    int bolsaCompraNetId;
                    var bolsaCompraNetIdNullable = !string.IsNullOrWhiteSpace(item.Bolsa) && bolsasPorCodigoSap.TryGetValue(item.Bolsa, out bolsaCompraNetId)
                        ? (int?)bolsaCompraNetId
                        : null;

                    if (existente != null)
                    {
                        // Modificar - solo actualizar propiedades escalares y FK, NO navegaciones
                        existente.Oblea = item.Oblea?.Trim();
                        existente.FechaCertificacion = fechaCertificacion ?? existente.FechaCertificacion;
                        existente.FechaVencimiento = fechaVencimiento ?? existente.FechaVencimiento;
                        existente.BolsaCompraNetId = bolsaCompraNetIdNullable ?? existente.BolsaCompraNetId;
                        existente.TipoObleaId = tipoOblea.Id;
                        existente.Rechazado = item.Rechazado ?? existente.Rechazado;
                        existente.FechaModificacion = ahora;
                        this.logDataAgroManager.LogCambiosControlBoletos(item, TipoAccionLogDataAgro.Modificar, existente.Id, "Modificacion de Certificacion - Control de Boletos");
                        hayCambiosEnBd = true;
                    }
                    else
                    {
                        // Crear
                        var nuevo = new ControlDeBoletosPreCertificacion
                        {
                            ControlDeBoletosId = controlDeBoletosId,
                            Oblea = item.Oblea?.Trim(),
                            TipoObleaId = tipoOblea.Id,
                            BolsaCompraNetId = bolsaCompraNetIdNullable,
                            FechaCertificacion = fechaCertificacion,
                            FechaVencimiento = fechaVencimiento,
                            Rechazado = item.Rechazado ?? string.Empty,
                            FechaCreacion = ahora
                        };
                        repositorio.Agregar(nuevo);
                        this.logDataAgroManager.LogCambiosControlBoletos(item, TipoAccionLogDataAgro.Crear, nuevo.Id, "Registro de Certificacion - Control de Boletos");
                        hayCambiosEnBd = true;
                        registrosPorTipoObleaId[tipoOblea.Id] = nuevo;
                    }
                }

                // ── 2. Eliminar registros en BD que ya no están en el request ──
                var tiposObleaIdsEnRequest = tiposObleaPorCodigo.Values
                    .Select(t => t.Id)
                    .ToHashSet();

                var registrosAEliminar = registrosEnBd
                    .Where(r => !tiposObleaIdsEnRequest.Contains(r.TipoObleaId))
                    .ToList();

                foreach (var eliminado in registrosAEliminar)
                {
                    repositorio.Remover(eliminado);
                }

                if (registrosAEliminar.Any())
                {
                    hayCambiosEnBd = true;
                }

                if (hayCambiosEnBd)
                {
                    repositorio.GuardarCambios();
                }

                // Ejecutar el estado al final, con la persistencia ya aplicada completa.
                this.controlDeBoletosManager.EstablecerEstadoBoleto(controlDeBoletosId);

                return oResultado;
            }
            catch (Exception ex)
            {
                oResultado.Errores.Add(new ErrorMessage { Message = ex.Message });
                logger.Error(ex.Message);
                return oResultado;
            }
        }
        public Resultado RegistrarDatosSeguimiento(ControlDeBoletosDatosSeguimientoServiceDto controlDeBoletosDatosSeguimiento)
        {
            var oResultado = new Resultado();
            try
            {
                if (controlDeBoletosDatosSeguimiento == null)
                {
                    oResultado.Errores.Add(new ErrorMessage { Message = "El request de seguimiento no puede ser nulo." });
                    return oResultado;
                }

                var negocio = repositorio.Obtener<Negocio>(x => x.ContratoSAP == controlDeBoletosDatosSeguimiento.ContratoSAP);
                if (negocio == null)
                {
                    oResultado.Errores.Add(new ErrorMessage { Message = "No se encontro un negocio para el ContratoSAP informado." });
                    return oResultado;
                }

                var controlDeBoletos = repositorio.Obtener<ControlDeBoletos>(x => x.NegocioId == negocio.Id);
                if (controlDeBoletos == null)
                {
                    oResultado.Errores.Add(new ErrorMessage { Message = "No se encontro el Control de Boletos asociado al negocio." });
                    return oResultado;
                }

                var controlDeBoletosId = controlDeBoletos.Id;
                var datosSeguimiento = repositorio.Obtener<ControlDeBoletosSeguimiento>(x => x.ControlDeBoletosId == controlDeBoletosId);
                var tipoBoletoSAP = controlDeBoletosDatosSeguimiento.TipoBoletoSAP;
                var bolsaCodigoSap = controlDeBoletosDatosSeguimiento.Bolsa;
                var boletoSap = string.IsNullOrWhiteSpace(tipoBoletoSAP)
                    ? null
                    : repositorio.Obtener<BoletoSap>(x => x.Tipo.Contains(tipoBoletoSAP));
                var bolsaCompraNet = string.IsNullOrWhiteSpace(bolsaCodigoSap)
                    ? null
                    : repositorio.Obtener<BolsaCompraNet>(b => b.CodigoSap == bolsaCodigoSap);
                var fechaRecepcionBoleto = ConvertirFechaNullable(controlDeBoletosDatosSeguimiento.FechaRecepcionBoleto);
                var fechaEnvioFirma = ConvertirFechaNullable(controlDeBoletosDatosSeguimiento.FechaEnvioFirma);
                var fechaEnvioBolsa = ConvertirFechaNullable(controlDeBoletosDatosSeguimiento.FechaEnvioBolsa);
                var fechaEnvioAfip = ConvertirFechaNullable(controlDeBoletosDatosSeguimiento.FechaEnvioAfip);
                var fechaRecepcionFirma = ConvertirFechaNullable(controlDeBoletosDatosSeguimiento.FechaRecepcionFirma);
                var fechaRecepcionBolsa = ConvertirFechaNullable(controlDeBoletosDatosSeguimiento.FechaRecepcionBolsa);
                var fechaRecepcionAfip = ConvertirFechaNullable(controlDeBoletosDatosSeguimiento.FechaRecepcionAfip);
                var fechaEnvioSellado = ConvertirFechaNullable(controlDeBoletosDatosSeguimiento.FechaEnvioSellado);
                var ahora = DateTime.Now;

                if (datosSeguimiento != null)
                {
                    if (string.IsNullOrWhiteSpace(controlDeBoletosDatosSeguimiento.BoletoSapCaracter) &&
                        string.IsNullOrWhiteSpace(controlDeBoletosDatosSeguimiento.Bolsa) &&
                        fechaRecepcionBoleto == null &&
                        fechaEnvioFirma == null &&
                        fechaEnvioBolsa == null &&
                        fechaEnvioAfip == null &&
                        fechaRecepcionFirma == null &&
                        fechaRecepcionBolsa == null &&
                        fechaRecepcionAfip == null &&
                        fechaEnvioSellado == null
                       )
                    {
                        repositorio.Remover(datosSeguimiento);
                    }
                    else
                    {
                        datosSeguimiento.BoletoSap = boletoSap;
                        datosSeguimiento.BoletoSapCaracter = controlDeBoletosDatosSeguimiento.BoletoSapCaracter;
                        datosSeguimiento.BolsaCompraNet = bolsaCompraNet;
                        datosSeguimiento.BolsaSellado = bolsaCodigoSap;
                        datosSeguimiento.FechaRecepcionBoleto = fechaRecepcionBoleto;
                        datosSeguimiento.FechaEnvioFirma = fechaEnvioFirma;
                        datosSeguimiento.FechaEnvioBolsa = fechaEnvioBolsa;
                        datosSeguimiento.FechaEnvioAfip = fechaEnvioAfip;
                        datosSeguimiento.FechaRecepcionFirma = fechaRecepcionFirma;
                        datosSeguimiento.FechaRecepcionBolsa = fechaRecepcionBolsa;
                        datosSeguimiento.FechaRecepcionAfip = fechaRecepcionAfip;
                        datosSeguimiento.FechaEnvioSellado = fechaEnvioSellado;
                        datosSeguimiento.ObsCtrlBoleto = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto;
                        datosSeguimiento.ObsCtrlBoleto2 = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto2;
                        datosSeguimiento.FechaModificacion = ahora;
                    }
                    repositorio.GuardarCambios();
                }
                else
                {
                    var seguimiento = new ControlDeBoletosSeguimiento
                    {
                        ControlDeBoletosId = controlDeBoletosId,
                        BoletoSap = boletoSap,
                        BoletoSapCaracter = controlDeBoletosDatosSeguimiento.BoletoSapCaracter,
                        BolsaCompraNet = bolsaCompraNet,
                        BolsaSellado = bolsaCodigoSap,
                        FechaRecepcionBoleto = fechaRecepcionBoleto,
                        FechaEnvioFirma = fechaEnvioFirma,
                        FechaEnvioBolsa = fechaEnvioBolsa,
                        FechaEnvioAfip = fechaEnvioAfip,
                        FechaRecepcionFirma = fechaRecepcionFirma,
                        FechaRecepcionBolsa = fechaRecepcionBolsa,
                        FechaRecepcionAfip = fechaRecepcionAfip,
                        FechaEnvioSellado = fechaEnvioSellado,
                        FechaCreacion = ahora,
                        ObsCtrlBoleto = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto,
                        ObsCtrlBoleto2 = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto2,
                    };
                    repositorio.Agregar(seguimiento);
                    repositorio.GuardarCambios();
                }

                this.controlDeBoletosManager.EstablecerEstadoBoleto(controlDeBoletosId);


                return oResultado;
            }
            catch (Exception ex)
            {
                oResultado.Errores.Add(new ErrorMessage { Message = ex.Message });
                logger.Error(ex.Message);
                return oResultado;
            }
        }
        #endregion

    }
}
