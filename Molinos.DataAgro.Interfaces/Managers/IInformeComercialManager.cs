using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IInformeComercialManager
    {
        List<InformeComercialMaterialDisponible> TraerInformeComercial(int ProveedorId);

        InformeResult GrabarInformeComercial(ParamInformeComercial informe, int IdActiveDirectory);

        RptInformeComercialInfo GenerarInformeComercial(ParamInformeComercial informe,int InformeId);

        List<ReportesList> ListarReportes(ParamReportesIC oParam, List<int> equipo);

        List<InformeList> TraerInformesGenerados() ;

        List<ResultCapacidadProductiva> TraerCapacidadProductiva(string proveedores);

        int GrabarCapacidadProductiva(string informes);

        ParamInformeComercial ReimprimirInformeComercial(int InformeId);

        List<InformeGeneradoList> TraerInformeComercialGenerado(int ProveedorId);

        Resultado EliminarInformes(int InformeId);

        List<MaterialesModificacionInforme> TraerInformeMateriales(int InformeId);

        Resultado RespuestaDeSapCapacidadProductiva(string cuit, string material, string respuesta);
    }
}
