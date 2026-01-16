using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
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
    public class ControlDeBoletosManager : IControlDeBoletosManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly IMailManager mailManager;

        public ControlDeBoletosManager(ILogger logger, IRepositorio repositorio, IMailManager mailManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.mailManager = mailManager;
        }

        public Resultado AsociarConfirma(int negocioId)
        {
            throw new NotImplementedException();
        }

        public Resultado RegistroContratoPendienteDeControl(int negocioId, int? identificadorConfirma = null)
        {
            var oResultado = new Resultado();
            var negocio = repositorio.Obtener<Negocio>(negocioId);
            if (negocio != null)
            {

                var existe = repositorio.Obtener<ControlDeBoletos>(x=> x.NegocioId ==negocioId);
                if (existe == null)
                {
                    var controlDeBoletos = new ControlDeBoletos()
                    {
                        NegocioId = negocioId,
                        FechaCreacion = DateTime.Now,
                        EsConfirma = negocio.BoletoVentaId == (int)EnumBoletoCompraNet.CONFIRMA,
                        IdentificadorConfirma = identificadorConfirma,
                        ControlDeBoletosEstadoId = (int)EnumControlDeBoletosEstado.PENDIENTE_CONTROL,
                        EstadoConfirmaId = (negocio.BoletoVentaId == (int)EnumBoletoCompraNet.CONFIRMA) ? (int)EnumEstadoConfirma.PENDIENTE : (int?)null
                    };
                    repositorio.Agregar(controlDeBoletos);
                    repositorio.GuardarCambios();
                }
            }
            return oResultado;
        }
    }
}
