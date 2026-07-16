using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletosValidacionIA;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Agent;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.Managers
{
    public class ControlDeBoletosValidacionIAManagerIA: IControlDeBoletosValidacionIAManagerIA
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly IMailManager mailManager;
        public ControlDeBoletosValidacionIAManagerIA(ILogger logger, IRepositorio repositorio, IMailManager mailManager                                       )
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.mailManager = mailManager;
        }

        public ControlDeBoletosValidacionIADatosContratoDto ObtenerDatosDeContrato(string contratoSAP)
        {
            var datosContrato = new ControlDeBoletosValidacionIADatosContratoDto();
            var contrato = repositorio.Obtener<Negocio>(x => x.ContratoSAP == contratoSAP);
            if (contrato != null)
            {

                datosContrato.Material = contrato.Material.Descripcion;
                datosContrato.Precio = contrato.Precio;
                datosContrato.Destino = contrato.Destino.Descripcion;
                datosContrato.Cosecha = contrato.Campana.Descripcion;
                datosContrato.TipoBoleto = contrato.Boleto.Descripcion;
                datosContrato.FechaOperacion = contrato.FechaOperacion;
                datosContrato.PeriodoEntrega = contrato.FechaDesde.ToString("dd/MM/yyyy") + " - " + contrato.FechaHasta.ToString("dd/MM/yyyy");
                datosContrato.CuitVendedor = contrato.Proveedor.CUIT;
                datosContrato.CuitCorredor = contrato.CorredorId > 0 ? contrato.Corredor.CUIT : string.Empty;
                datosContrato.Moneda = contrato.Moneda?.MonedaId;
                datosContrato.ProvinciaOrigen = contrato.Provincia.Nombre;
                datosContrato.LocalidadOrigen = contrato.Localidad.Nombre;
                datosContrato.Kilos = contrato.Cantidad;
                datosContrato.FechaFijacionDesde = contrato.DesdeFijacion;
                datosContrato.FechaFijacionHasta = contrato.HastaFijacion;
                datosContrato.Bolsa = contrato.Bolsa.Descripcion;
                datosContrato.TipoBoleto = contrato.Boleto.Descripcion;
            }
            return datosContrato;
        }
    }
}
