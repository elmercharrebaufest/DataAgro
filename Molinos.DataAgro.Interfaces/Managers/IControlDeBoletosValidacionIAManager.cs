using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletosValidacionIA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces.Managers
{
    public interface IControlDeBoletosValidacionIAManager
    {
        ValidacionDeBoletosIADatosContratoDto ObtenerDatosDeContrato(string contratoSAP);
        List<ValidacionDeBoletosIAResultadoClausulaDto> ObtenerClausulas(string contratoSAP);
        List<ValidacionBoletosEstadoDto> ListarEstados();
        List<ValidacionDeBoletosIAConsultaDto> GetValidacionBoletosPendientes(ValidacionBoletoFiltroBusquedaDto filtros);
        List<ValidacionDeBoletosIADatosContratoDto> GetValidacionBoletoSap(string contratoSAP);
        Resultado RegistrarValidacion(ValidacionBoletosDto validacionBoletosDto);
        Resultado RegistrarValidacionResultado(ValidacionBoletosResultadoDto validacionBoletosDto);
        List<ValidacionBoletosResultadoDto> GetValidacionResultadosPorContrato(int validacionBoletosId);
        Resultado AprobarValidacionResultado(AccionesValidacionBoletosDto accionesValidacionBoletos);
        Resultado RechazarValidacionResultado(AccionesValidacionBoletosDto accionesValidacionBoletos);
    }
}
