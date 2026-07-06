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

        private static string ValidarFechasSeguimiento(
            ControlDeBoletosDatosSeguimientoServiceDto seguimiento,
            bool operaSinOblea,
            bool esCartaOferta,
            bool esSinBoleto,
            DateTime? feRecepcionBoleto,
            DateTime? feEnvioFirmas,
            DateTime? feEnvioBolsa,
            DateTime? feEnvioAfip,
            DateTime? feRecepcionFirma,
            DateTime? feRecepcionBolsa,
            DateTime? feRecepcionAfip,
            DateTime? feEnvioSellado)
        {
            if (string.IsNullOrWhiteSpace(seguimiento.TipoBoletoSAP) ||
                string.IsNullOrWhiteSpace(seguimiento.BoletoSapCaracter))
            {
                return "Debe seleccionar un boleto y un caracter.";
            }

            if (string.IsNullOrWhiteSpace(seguimiento.Bolsa))
            {
                return "Debe seleccionar una bolsa.";
            }

            if (esSinBoleto)
            {
                return null;
            }

            if (!feRecepcionBoleto.HasValue)
            {
                return "La Fecha de recepcion de boleto es obligatoria.";
            }

            if (feEnvioFirmas.HasValue && feEnvioFirmas.Value < feRecepcionBoleto.Value)
                return "La Fecha de envio Firmas no puede ser anterior a la Fecha de recepcion de boleto.";

            if ((feEnvioBolsa.HasValue && !operaSinOblea) && feEnvioBolsa.Value < feRecepcionBoleto.Value)
                return "La Fecha de envio Obleado Bolsa no puede ser anterior a la Fecha de recepcion de boleto.";

            if (feEnvioAfip.HasValue && feEnvioAfip.Value < feRecepcionBoleto.Value)
                return "La Fecha de envio Certificacion Arca no puede ser anterior a la Fecha de recepcion de boleto.";

            if (feRecepcionFirma.HasValue)
            {
                if (feRecepcionFirma.Value < feRecepcionBoleto.Value)
                    return "La Fecha de recepcion Firmas no puede ser anterior a la Fecha de recepcion de boleto.";

                if (feEnvioFirmas.HasValue && feRecepcionFirma.Value < feEnvioFirmas.Value)
                    return "La Fecha de recepcion Firmas debe ser igual o mayor a la Fecha de envio Firmas.";
            }

            if (feRecepcionBolsa.HasValue && !operaSinOblea)
            {
                if (feRecepcionBolsa.Value < feRecepcionBoleto.Value)
                    return "La Fecha de recepcion Obleado Bolsa no puede ser anterior a la Fecha de recepcion de boleto.";

                if (feEnvioBolsa.HasValue && feRecepcionBolsa.Value < feEnvioBolsa.Value)
                    return "La Fecha de recepcion Obleado Bolsa debe ser igual o mayor a la Fecha de envio Obleado Bolsa.";
            }

            if (feRecepcionAfip.HasValue)
            {
                if (feRecepcionAfip.Value < feRecepcionBoleto.Value)
                    return "La Fecha de recepcion Certificacion Arca no puede ser anterior a la Fecha de recepcion de boleto.";

                if (feEnvioAfip.HasValue && feRecepcionAfip.Value < feEnvioAfip.Value)
                    return "La Fecha de recepcion Certificacion Arca debe ser igual o mayor a la Fecha de envio Certificacion Arca.";
            }

            if (feEnvioSellado.HasValue && !esCartaOferta)
            {
                if (feEnvioSellado.Value < feRecepcionBoleto.Value)
                    return "La Fecha de envio Sellado no puede ser anterior a la Fecha de recepcion de boleto.";

                if (feRecepcionBolsa.HasValue && feEnvioSellado.Value < feRecepcionBolsa.Value)
                    return "La Fecha de envio Sellado debe ser igual o mayor a la Fecha de recepcion Obleado Bolsa.";
            }

            return null;
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
                    oResultado.Errores.Add(new ErrorMessage { Message = "No se encontro el ContratoSAP informado." });
                    return oResultado;
                }

                var controlDeBoletos = repositorio.Obtener<ControlDeBoletos>(x => x.NegocioId == negocio.Id);
                if (controlDeBoletos == null)
                {
                    oResultado.Errores.Add(new ErrorMessage { Message = "No se encontro el Control de Boletos asociado al contrato." });
                    return oResultado;
                }


                var seguimientoBoleto = repositorio.Obtener<ControlDeBoletosSeguimiento>(x => x.ControlDeBoletosId == controlDeBoletos.Id);
                if (seguimientoBoleto == null || seguimientoBoleto.FechaRecepcionBoleto.HasValue)
                {
                    oResultado.Errores.Add(new ErrorMessage { Message = "No se ha registrado una fecha de recepción para el Control de Boletos asociado al contrato." });
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

                    var erroresValidacion = new List<string>();
                    switch (item.TipoOblea)
                    {
                        case "A":
                            if (bolsaCompraNetIdNullable != null)
                                erroresValidacion.Add("No se debe informar codigo de bolsa.");
                            if (fechaVencimiento != null)
                                erroresValidacion.Add("No se debe informar fecha de vencimiento.");

                            var mensajeDuplicidadA = this.controlDeBoletosManager.VerificarDuplicidadObleaCodigoArca(controlDeBoletosId, string.Empty, item.Oblea);
                            if (!string.IsNullOrWhiteSpace(mensajeDuplicidadA))
                                erroresValidacion.Add(mensajeDuplicidadA.Trim().TrimEnd(','));
                            break;
                        case "F":
                            var mensajeDuplicidadF = this.controlDeBoletosManager.VerificarDuplicidadObleaCodigoArca(controlDeBoletosId, item.Oblea, string.Empty);
                            if (!string.IsNullOrWhiteSpace(mensajeDuplicidadF))
                                erroresValidacion.Add(mensajeDuplicidadF.Trim().TrimEnd(','));
                            break;
                        case "P":
                            if (string.IsNullOrEmpty(item.Bolsa?.Trim()))
                                erroresValidacion.Add("No se debe informar codigo de bolsa.");
                            if (!string.IsNullOrWhiteSpace(item.Bolsa) && bolsaCompraNetIdNullable == null)
                                erroresValidacion.Add("El codigo de bolsa informado no existe.");
                            if (fechaCertificacion != null)
                                erroresValidacion.Add("No se debe informar fecha de certificacion.");
                            break;
                        case "O":
                            var mensajeDuplicidadO = this.controlDeBoletosManager.VerificarDuplicidadObleaCodigoArca(controlDeBoletosId, item.Oblea, string.Empty);
                            if (!string.IsNullOrWhiteSpace(mensajeDuplicidadO))
                                erroresValidacion.Add(mensajeDuplicidadO.Trim().TrimEnd(','));
                            break;
                    }

                    if (erroresValidacion.Any())
                    {
                        oResultado.Errores.Add(new ErrorMessage
                        {
                            Message = string.Format(
                                "Validacion de pre-certificacion para '{0}': {1}",
                                tipoOblea.Descripcion,
                                string.Join(" ", erroresValidacion))
                        });
                        return oResultado;
                    }

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

                var esCartaOferta = negocio.BoletoId == (int)EnumBoletoCompraNet.CARTA_OFERTA;
                var esSinBoleto = negocio.BoletoId == (int)EnumBoletoCompraNet.SIN_BOLETO;

                string tipoProveedor;
                string cuitProveedor;
                if (negocio.CorredorId.HasValue && negocio.CorredorId.Value > 0)
                {
                    tipoProveedor = "CORR";
                    var corredor = repositorio.Obtener<Proveedor>(x => x.ProveedorId == negocio.CorredorId.Value);
                    cuitProveedor = corredor == null ? string.Empty : corredor.CUIT;
                }
                else
                {
                    tipoProveedor = "PROV";
                    var proveedor = negocio.ProveedorId.HasValue
                        ? repositorio.Obtener<Proveedor>(x => x.ProveedorId == negocio.ProveedorId.Value)
                        : null;
                    cuitProveedor = proveedor == null ? string.Empty : proveedor.CUIT;
                }

                var operaSinOblea = !string.IsNullOrWhiteSpace(cuitProveedor) &&
                                    this.controlDeBoletosManager.VerificarOperaSinOblea(cuitProveedor, tipoProveedor) == "SI";

                var mensajeValidacionFechas = ValidarFechasSeguimiento(
                    controlDeBoletosDatosSeguimiento,
                    operaSinOblea,
                    esCartaOferta,
                    esSinBoleto,
                    fechaRecepcionBoleto,
                    fechaEnvioFirma,
                    fechaEnvioBolsa,
                    fechaEnvioAfip,
                    fechaRecepcionFirma,
                    fechaRecepcionBolsa,
                    fechaRecepcionAfip,
                    fechaEnvioSellado);

                if (!string.IsNullOrWhiteSpace(mensajeValidacionFechas))
                {
                    oResultado.Errores.Add(new ErrorMessage { Message = mensajeValidacionFechas });
                    return oResultado;
                }

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
