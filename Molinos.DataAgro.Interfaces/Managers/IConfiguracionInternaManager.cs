using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IConfiguracionInternaManager
    {
        //ConfiguracionInterna TraerConfiguracion();
        Resultado GrabarPrecio(PrecioMoa oConfiguracion, string active);
        Resultado GrabarPizarra(HabilitacionPizarra oConfiguracion, string active, DateTime fechaHasta);
        Resultado GrabarFijacion(HabilitacionFijacion oConfiguracion);
        List<PrecioMoaDto> TraerPrecios(); 
        List<HabilitacionFijacionDto> TraerFijaciones(); 
        List<HabilitacionPizarraDto> TraerPizarra();
        Resultado EliminarPrecio(int id);
        Resultado EliminarPizarra(int id);
        Resultado EliminarFijacion(int id);
        IEnumerable<IGrouping<int, PrecioMoaCompraNetDto>> TraerPrecioCompraNet(int? tiponegocio = null);
        List<PrecioMoaCompraNetDto> TraerPrecioCompraNet(int materialId,int? tiponegocio = null);
        bool HabilitarPizarra(int material);
        List<HabilitacionCampañaDto> TraerCampaña();
        Resultado EliminarCampaña(int id);
        Resultado GrabarCampaña(HabilitacionCampaña oConfiguracion);
        HabilitacionPizarraDto HabilitarPizarraExterno(int material,int tiponegocio);
        List<HabilitacionCampañaDto> HabilitarCampañaExterno(int material);

        List<HabilitacionPagoDiferidoDto> TraerPagosDiferido();
        List<HabilitacionPagoDiferidoDto> TraerPagoDiferido();

        Resultado GrabarPagoDiferido(HabilitacionPagoDiferido oConfiguracion, string active);
        Resultado EliminarHabilitacionPagoDiferido(int id);
        void PausarCargaDePrecios(bool pausa);
        bool TraerPausadoGeneral();
        List<EstadoPrecioMOADto> TraerEstadoPrecioMOA();
        void CambiarEstadoPrecioMOA(List<EstadoPrecioMOADto> precios);
    }
}
