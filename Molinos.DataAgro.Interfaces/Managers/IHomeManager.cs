using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IHomeManager
    {
        ResultIniContacto TraerTodoContacto(int idComercial, int pagina = 0);

        ResultIniContacto TraerBusquedaContacto(oParamBusqueda oParam);

        CampañaHome TraerInfoCampaña(int idComercial, List<int> equipo);

        DatosIniciales TraerInfoIniciales(List<int> equipo);

        int TraerIdComercial(string idActiveDirectory);

        List<BusquedaHome> BusquedaHome(string filtro, int ComercialId);

        List<ActividadRecordatorio> TraerActividadesPorComercialId(int ComercialId);

        List<ContactoIni> ExportarContactos(List<int> Ids, string idActiveDirectory);

        ExportAll ExportarAll(List<int> Ids, string idActiveDirectory);

        PostItDto TraerTexto(int idComercial);

        GrabarPostItResult GuardarPostIt(PostIt post);
    }
}
