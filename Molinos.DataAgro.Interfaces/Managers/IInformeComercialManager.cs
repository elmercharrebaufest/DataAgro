using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IInformeComercialManager
    {
        List<InformeComercialMaterialDisponible> TraerInformeComercial(int ProveedorId);

        DataSourceResult TraerInformesFiltrados(DataSourceRequest filtro);

        InformeResult GrabarInformeComercial(ParamInformeComercial informe, int IdActiveDirectory, List<NuevoProduccion> nuevosCampos, List<NuevoAcopio> nuevosAcopios, 
            ContactoComercial contactoComercial, string direccion, string codigoPostal, int? localidadId);

        RptInformeComercialInfo GenerarInformeComercial(ParamInformeComercial informe, int InformeId);

        List<ReportesList> ListarReportes(ParamReportesIC oParam, List<int> equipo);

        List<InformeList> TraerInformesGenerados();

        List<ResultCapacidadProductiva> TraerCapacidadProductiva(string proveedores);

        int GrabarCapacidadProductiva(string informes);

        ParamInformeComercial ReimprimirInformeComercial(int InformeId);

        List<InformeGeneradoList> TraerInformeComercialGenerado(int ProveedorId);

        Resultado EliminarInformes(int InformeId);

        List<MaterialesModificacionInforme> TraerInformeMateriales(int InformeId);

        Resultado RespuestaDeSapCapacidadProductiva(string cuit, string material, string respuesta);
        
        void EnviarMailInformeComercial(string identificador);

        void GuardarFechaDescargaInformeComercial(List<int> ids);

        Resultado EnviarCapacidadProductivaSAP(List<EnviarCapacidadProductivaSAPDto> enviar);
    }
}
