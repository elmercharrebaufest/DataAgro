using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IFijacionDePrecioContratoManager
    {
        DatosIniAbmFijacionDePrecioContrato TraerDatosIniciales();

        GrabarContratoResult GrabarAmpliacionFijacion(FijacionDePrecioContrato oFijacion);

        GrabarFijacionResult GrabarFijacionDePrecio(FijacionDePrecioContrato oFijacionDePrecio);

        GrabarFijacionResult ConfirmarFijacion(int fijacionDePrecioContratoId, int comercialId);

        GrabarContratoResult BorrarFijacion(FijacionDePrecioContrato oContrato);
        GrabarContratoResult BorrarFijacionPreAprobacion(int id, string motivo);

        GrabarFijacionResult FinalizarFijacion(int fijacionDePrecioContratoId, string activeDiretoryId);

        BasicoContrato TraerFijacion(int id);
        List<DatosFijacionDeContratoDto> TraerDatosFijacion(string cuitProveedor, string cuitCorredor, int materialId, string filtro, int fijacionId);
        List<AperturaPrecioDto> TraerAperturaDePrecioPorFijacion(int fijacionId);
        void FinalizacionAutomatica(string idActiveDirectory);
        GrabarFijacionResult AprobarFijacion(int id);
        GrabarFijacionResult ActualizarFijacion(FijacionDePrecioContrato oContrato);
        List<DateTime> FechaFeriados();
        DateTime UltimoDiaHabil();
        Resultado ActualizarFijacionSap(FijacionDePrecioContrato fijacion);
        Resultado AltaFijacionSap(FijacionDePrecioContrato fijacion, List<FijacionVirtualSAPDto> fijacionVirtuales);
        Resultado AnularFijacionSAP(FijacionSAP fijacion);
        Resultado AnularFijacionCarga(int fijacionId, string motivoRechazo);
        void BuscarComision(BasicoContrato negocio);
        List<DatosFijacionDeContratoDto> TraerDatosFijacionVirtual(string CuitProveedor, string CuitCorredor, int materialId, string filtro, int fijacionId);
     
    }
}


