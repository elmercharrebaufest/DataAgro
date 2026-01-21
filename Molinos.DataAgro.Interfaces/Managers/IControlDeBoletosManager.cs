using Molinos.DataAgro.Entities.Dto;
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
        Resultado RegistroContratoPendienteDeControl(int negocioId, int? identificadorConfirma = null);
        Resultado AsociarConfirma(int negocioId);

        List<ProveedorCombo> GetProveedorPorComercial(List<int> equipo);
        List<MaterialCombo> GetMaterial();
        List<ComercialCombo> GetComercial();
        List<BolsaCompraNetQry> GetBolsaCompraNet();
    }
}
