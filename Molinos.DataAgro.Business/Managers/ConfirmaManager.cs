using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Clausulas;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.css;
using Molinos.DataAgro.Entities.Common.Enums;
using System.Globalization;
using System.ServiceModel.Channels;

namespace Molinos.DataAgro.Business.Managers
{
    public class ConfirmaManager : IConfirmaManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly IStatusContratoAgent status;

        public ConfirmaManager(IRepositorio repositorio, ILogger logger, IStatusContratoAgent status)
        {
            this.repositorio = repositorio;
            this.logger = logger;
            this.status = status;
        }

        public DatosIniContrato TraerDatosCombos()
        {
            var datosCombo = new DatosIniContrato();
            datosCombo.tiponegocio.Add(new TipoNegocioQry { TipoNegocioId = 1, Descripcion = "Contrato" });
            datosCombo.tiponegocio.Add(new TipoNegocioQry { TipoNegocioId = 2, Descripcion = "Fijación" });
            return datosCombo;
        }

        public List<string> ListarNegociosPorRangoCodigoSAP(int negocioDesde, int negocioHasta, int tipoNegocio)
        {
            var listaNegocios = new List<Negocio>();
            if (tipoNegocio == 1)
            {
                listaNegocios = repositorio.Listar<Negocio>(x => (x.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO || x.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR) && !string.IsNullOrEmpty(x.ContratoSAP) && x.ConfirmadoSAP == true);
            }
            else
            {
                listaNegocios = repositorio.Listar<Negocio>(x => x.TipoNegocioId == (int)EnumTipoNegocio.FIJACION && !string.IsNullOrEmpty(x.ContratoSAP) && x.ConfirmadoSAP == true && x.Canje == true && x.Cantidad >= 10000);
            }

            List<string> codigos = listaNegocios.Where(x => int.Parse(x.ContratoSAP) >= negocioDesde && int.Parse(x.ContratoSAP) <= negocioHasta).Select(x => x.ContratoSAP.TrimStart('0')).ToList();
            codigos.Sort();
            return codigos.FindAll(x => ValidarNegocio(x, tipoNegocio) == ""); ;
        }

        public List<string> ValidarNegocios(List<string> codigosSAP, int tipoNegocio)
        {
            List<string> rechazados = new List<string>();
            foreach (var itemNegocio in codigosSAP)
            {
                var mensaje = ValidarNegocio(itemNegocio, tipoNegocio);
                if (!string.IsNullOrEmpty(mensaje))
                {
                    rechazados.Add(mensaje);
                }
            }
            return rechazados;
        }

        public List<string> FiltrarNegociosPorFecha(string desde, string hasta, int tipoNegocio)
        {
            var fechaDesde = DateTime.ParseExact(desde, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var fechaHasta = hasta == "" ? DateTime.Now : DateTime.ParseExact(hasta, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1);
            var listaNegocios = new List<Negocio>();
            if (tipoNegocio == 1)
            {
                listaNegocios = repositorio.Listar<Negocio>(x => (x.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO || x.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR) && !string.IsNullOrEmpty(x.ContratoSAP) && x.ConfirmadoSAP == true && x.FechaConfirmacion >= fechaDesde && x.FechaConfirmacion <= fechaHasta);
            }
            else
            {
                listaNegocios = repositorio.Listar<Negocio>(x => x.TipoNegocioId == (int)EnumTipoNegocio.FIJACION && !string.IsNullOrEmpty(x.ContratoSAP) && x.ConfirmadoSAP == true && x.Canje == true && x.FechaConfirmacion >= fechaDesde && x.FechaConfirmacion <= fechaHasta && x.Cantidad >= 10000);
            }
            var result = listaNegocios.Select(x => x.ContratoSAP.TrimStart('0')).ToList();
            return result.FindAll(x => ValidarNegocio(x, tipoNegocio) == "");
        }

        public string ValidarNegocio(string codigoSAP, int tipoNegocio)
        {
            var mensaje = "";
            var kilosDisponibles = 10000;
            var contrato = repositorio.Obtener<Contrato>(x => x.ContratoSAP == codigoSAP);
            if (tipoNegocio == (int)EnumTipoNegocio.FIJACION)
            {
                if (contrato != null)
                {
                    if (contrato.Cantidad < kilosDisponibles)
                    {
                        mensaje = $"No se puede generar el confirma para la fijación {codigoSAP} por su cantidad menor a 10 toneladas.";
                        logger.Debug($"No se puede generar el confirma para la fijacion {codigoSAP} por cantidad menor a 10 toneladas.");
                    }
                    if (contrato.Canje != true)
                    {
                        mensaje = $"No se puede generar el confirma para la fijación {codigoSAP} por no ser de canje.";
                        logger.Debug($"No se puede generar el confirma para la fijacion {codigoSAP} por no ser de canje.");
                    }
                    if (contrato.BoletoId != (int)EnumBoletoCompraNet.FISICO && contrato.BoletoId != (int)EnumBoletoCompraNet.CARTA_OFERTA)
                    {
                        mensaje = $"No se puede generar el confirma para la fijación {codigoSAP} por no tener tilde de boleto físico o carta oferta.";
                        logger.Debug($"No se puede generar el confirma para la fijacion {codigoSAP} por no tener tilde de boleto físico o carta oferta.");
                    }
                }
                else
                {
                    mensaje = $"No se encontró el contrato para la fijación {codigoSAP} seleccionada.";
                }
            }
            else
            {
                var res = status.ValidarEstado(codigoSAP);
                if (!string.IsNullOrEmpty(res.Status) && res.Status != "X")
                {
                    string motivoStatus = StatusNegocioConfirma(res);
                    mensaje = $"No se puede generar el confirma con negocio {codigoSAP} para el contrato por su estado: {motivoStatus}";
                    logger.Debug($"No se puede generar el confirma por el status: {res.Status} ({motivoStatus}) - ContratoSAP: {codigoSAP}");
                }
                else if (string.IsNullOrEmpty(res.Status))
                {
                    mensaje = $"No se puede generar el confirma con negocio {codigoSAP} para el contrato por estar en slip.";
                    logger.Debug($"No se puede generar el confirma por tener status vacío (slip) - ContratoSAP: {codigoSAP}");
                }
            }
            return mensaje;
        }

        private string StatusNegocioConfirma(EstadoSAPDto statusNegocio)
        {
            string msje = "";
            switch (statusNegocio.Status)
            {
                case "A":
                    msje = "Con Anulación Automática";
                    break;

                case "X":
                    msje = "Confirmado";
                    break;

                case "F":
                    msje = "Liquidación Finalizada";
                    break;

                case "C":
                    msje = "Cumplido";
                    break;

                case "M":
                    msje = "Con Anulación Parcial";
                    break;

                case "B":
                    msje = "Contrato Anulado Totalmente";
                    break;

                case "K":
                    msje = "Cumplido en Camiones(no se usa)";
                    break;

                case "T":
                    msje = "Contrato de Canje Cerrado(no se usa)";
                    break;

                case "J":
                    msje = "Prefijación Cerrada(no se usa)";
                    break;

                default:
                    break;
            }
            return msje;
        }
    }
}