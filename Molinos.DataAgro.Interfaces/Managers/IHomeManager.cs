using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IHomeManager
    {
        ResultIniContacto TraerBusquedaContacto(oParamBusqueda oParam, int pagina);

        CampañaHome TraerInfoCampaña(int idComercial, List<int> equipo);

        DatosIniciales TraerInfoIniciales(List<int> equipo);

        int TraerIdComercial(string idActiveDirectory);

        List<BusquedaHome> BusquedaHome(string filtro, int ComercialId, List<int> equipo, List<int> corredoresComercial, int perfilId);

        List<ActividadRecordatorio> TraerActividadesPorComercialId(int ComercialId);

        List<ContactoIni> ExportarContactos(oParamBusqueda oParam, string idActiveDirectory);

        ExportAll ExportarAll(oParamBusqueda oParam, string idActiveDirectory);

        PostItDto TraerTexto(int idComercial);

        GrabarPostItResult GuardarPostIt(PostIt post);
    }
}
