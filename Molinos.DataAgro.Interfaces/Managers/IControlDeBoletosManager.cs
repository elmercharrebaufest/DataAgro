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
        List<BoletoCompraNet> GetBoletoCompraNet();
        Resultado RegistroContratoPendienteDeControl(int negocioId, int? altaIdLoteConfirma = null);
        Resultado AsociarConfirma(int negocioId);
        List<ControlDeBoletosConsultaDto> GetControlBoletosPendientes(ControlDeBoletoFiltroBusquedaDto filtros);
        Resultado ModificacionContrato(ControlDeBoletosModificacionContratoDto controlDeBoletosModificacion);
        Resultado RegistrarAcciones(List<int> ControlDeBoletoIds, EnumControlDeBoletosAcciones accion);
        Resultado ProcesarBoletosPendientesControl();
        Resultado RegistrarDatosPreCertificacion(ControlDeBoletosPreCertificacionDto controlDeBoletosPreCertificacion);
        ControlDeBoletosPreCertificacionDto ObtenerDatosPreCertificacion(int datosPreCertificacionId);
        Resultado RegistroDatosDeSeguimiento(ControlDeBoletosDatosSeguimientoDto controlDeBoletosDatosSeguimiento);
        ControlDeBoletosDatosSeguimientoDto ObtenerDatosDeSeguimiento(int datosSeguimientoId);
        List<ControlDeBoletoTrackingDto> ObtenerTrackingBoletos(int controlDeBoletosId);
        List<ControlDeBoletosReporteSeguimientoConsultaDto> GetReporteDeSeguimientoBoletos(ControlDeBoletoFiltroSeguimientoDto filtros);
    }
}
