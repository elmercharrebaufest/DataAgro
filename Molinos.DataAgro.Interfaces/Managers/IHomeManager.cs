using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IHomeManager
    {
        ResultIniContacto TraerBusquedaContacto(oParamBusqueda oParam, int pagina, List<int> corredoresComerciales);
        CampañaHome TraerInfoCampaña(int idComercial, List<int> equipo);
        ObjetivoHome TraerInfoObjetivo(int? idComercial, List<int> equipo, int? idZona, int? idComercialLogeado);
        DatosIniciales TraerInfoIniciales(List<int> equipo);
        int TraerIdComercial(string idActiveDirectory);
        List<BusquedaHome> BusquedaHome(string filtro, int ComercialId, List<int> equipo, List<int> corredoresComercial);
        List<ActividadRecordatorio> TraerActividadesPorComercialId(int ComercialId);
        List<ContactoIni> ExportarContactos(oParamBusqueda oParam, string idActiveDirectory, List<int> equipo);
        ExportAll ExportarAll(oParamBusqueda oParam, string idActiveDirectory, List<int> equipo);
        PostItDto TraerTexto(int idComercial);
        List<int> ListarTodosLosComercialesConMismaZona(int comercialId);
        GrabarPostItResult GuardarPostIt(PostIt post);
        List<CompraDto> TraerTodoCompraDetalle(List<int> equipo, int? comercialId, int? zonaId);
        List<CompraCampanaActualDto> TraerTodoCompraCampanaActual(List<int> equipo);
        List<CapacidadProductivaDesactualizadaDto> ProveedoresConCapProdDesactualizada(int comercialId, bool esAdministrador);
        byte[] ExportarListadoAXls(List<CapacidadProductivaDesactualizadaDto> listado);
    }
}