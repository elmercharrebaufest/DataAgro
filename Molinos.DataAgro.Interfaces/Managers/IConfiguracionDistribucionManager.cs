using Molinos.DataAgro.Entities.Dto.Distribucion;

namespace Molinos.DataAgro.Interfaces
{
    public interface IConfiguracionDistribucionManager
    {
        ConfiguracionDistribucionDto ObtenerConfiguracion();
        void GuardarConfiguracion(ConfiguracionDistribucionDto dto);
    }
}
