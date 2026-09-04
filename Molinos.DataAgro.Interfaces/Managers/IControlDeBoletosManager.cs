using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces.Managers
{
    public interface IControlDeBoletosManager
    {
        // Metodos para cargar combos
        List<ProveedorCombo> GetProveedorPorComercial(List<int> equipo);
        List<MaterialCombo> GetMaterial();
        List<ComercialCombo> GetComercial();
        List<BolsaCompraNet> GetBolsaCompraNet();
        List<Provincia> GetProvincias();
        List<CampaniaCombo> GetCosechas();
        List<ClasificacionCompraNet> GetClasificaciones();
        List<Localidad> GetProcedencias(int provinciaId);
        List<TipoOblea> GetTipoOblea();
        List<BoletoSapDto> GetBoletoSap(int BoletoCompraNetId);
        Resultado RegistroContratoPendienteDeControl(int negocioId, int? altaIdLoteConfirma = null, int? altaIdDocumentoConfirma = null, bool? esConfirmaAltaBorrador = false);
        (List<ControlDeBoletosConsultaDto> Data, int Total) GetControlBoletosPendientes(ControlDeBoletoFiltroBusquedaDto filtros);
        Resultado ModificacionContrato(ControlDeBoletosModificacionContratoDto controlDeBoletosModificacion);
        ControlDeBoletosDatosContratoDto ObtenerDatosDeContrato(int negocioId);
        Resultado ActualizarEstadoBoletosConfirma();
        Resultado RegistrarDatosPreCertificacion(ControlDeBoletosPreCertificacionDto controlDeBoletosPreCertificacion);
        ControlDeBoletosPreCertificacionDto ObtenerDatosPreCertificacion(int datosPreCertificacionId);
        Resultado RegistroDatosDeSeguimiento(ControlDeBoletosDatosSeguimientoDto controlDeBoletosDatosSeguimiento);
        ControlDeBoletosDatosSeguimientoDto ObtenerDatosDeSeguimiento(int controlDeBoletosId);
        List<ControlDeBoletoTrackingDto> ObtenerTrackingBoletos(int controlDeBoletosId);
        List<ControlDeBoletosReporteSeguimientoConsultaDto> GetReporteDeSeguimientoBoletos(ControlDeBoletoFiltroSeguimientoDto filtros);
        string VerificarOperaSinOblea(string cuit, string tipoProveedor);
        string VerificarTipoBoletoyFechaRecepcion(int controlDeBoletosId);
        List<ControlDeBoletosParaModificarDto> GetBoletosParaModificar(ControlDeBoletosParaModificarFiltroDto filtros);
        Resultado GuardarBoletosParaModificarFechas(List<ControlDeBoletosParaModificarDto> boletos);
        string VerificarDuplicidadObleaCodigoArca(int controlDeBoletosId, string numeroOblea, string codigoArca);
        Resultado EliminarControlDeBoletos(EliminarControlDeBoletoDto eliminarControlDeBoleto);
        void EstablecerEstadoBoleto(int controlDeBoletoId);
        ConfirmaDocumentoRegistradoDto ObtenerDocumentoConfirma(int controlDeBoletoId);
        Resultado EliminarPreCertificacion(int controlDeBoletosId);
        Resultado EliminarDatosSeguimiento(int controlDeBoletosId);
        List<ControlDeBoletosDatosContratoHijoDto> ListarContratosHijos(string ContratosSAP);
        Resultado AgregarContratosAlControlDeBoletos(string negociosIds);
    }
}
