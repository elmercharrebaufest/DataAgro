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
        Resultado GrabarPrecio(PrecioMoa oConfiguracion);
        Resultado GrabarPizarra(HabilitacionPizarra oConfiguracion);
        Resultado GrabarFijacion(HabilitacionFijacion oConfiguracion);
        List<PrecioMoaDto> TraerPrecios(); 
        List<HabilitacionFijacionDto> TraerFijaciones(); 
        List<HabilitacionPizarraDto> TraerPizarra();
        Resultado EliminarPrecio(int id);
        Resultado EliminarPizarra(int id);
        Resultado EliminarFijacion(int id);
        IEnumerable<IGrouping<int, PrecioMoaCompraNetDto>> TraerPrecioCompraNet();
        List<PrecioMoaCompraNetDto> TraerPrecioCompraNet(int materialId);
        bool HabilitarPizarra(int material);
    }
}
