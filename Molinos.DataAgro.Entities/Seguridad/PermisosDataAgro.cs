using System.ComponentModel.DataAnnotations;
using Molinos.DataAgro.Entities.Resources;

namespace Molinos.DataAgro.Entities.Seguridad
{
    public enum PermisosDataAgro
    {
        [Display(ResourceType = typeof(Text), Name = "Ingreso_DataAgro")]
        IngresoDataAgro = 0,
        [Display(ResourceType = typeof(Text), Name = "Contador_Proveedores")]
        ContadorProveedores = 101,
        [Display(ResourceType = typeof(Text), Name = "Alta_Datos_Proveedor")]
        AltaDatosProveedor = 102,
        [Display(ResourceType = typeof(Text), Name = "Modificar_Datos_Proveedor")]
        ModificarDatosProveedor = 103,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_Datos_Proveedor")]
        VisualizarDatosProveedor = 104,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_Compras_Proveedor")]
        VisualizarComprasProveedor = 105,
        [Display(ResourceType = typeof(Text), Name = "Descarga_Excel")]
        DescargaExcel = 106,
        [Display(ResourceType = typeof(Text), Name = "Descarga_Pdf")]
        DescargaPdf = 107,
        [Display(ResourceType = typeof(Text), Name = "Descarga_Export_All_Comercial")]
        DescargaExportAllComercial = 108,
        [Display(ResourceType = typeof(Text), Name = "Descarga_Export_All_Visualizador")]
        DescargaExportAllVisualizador = 109,
        [Display(ResourceType = typeof(Text), Name = "Asignar_Proveedores")]
        AsignarProveedores = 110,
        [Display(ResourceType = typeof(Text), Name = "Filtrar_Administrativo")]
        FiltrarAdministrativo = 111,
        [Display(ResourceType = typeof(Text), Name = "Agenda_Comercial")]
        AgendaComercial = 201,
        [Display(ResourceType = typeof(Text), Name = "Informe_Comercial")]
        InformeComercial = 202,       
        [Display(ResourceType = typeof(Text), Name = "Modificar_Interes")]
        ModificarInteres = 203,
        [Display(ResourceType = typeof(Text), Name = "Canal_Notificaciones")]
        CanalNotificaciones = 204,
        [Display(ResourceType = typeof(Text), Name = "Ver_Corredor_Comercial")]
        VerCorredorComercial = 205,
        [Display(ResourceType = typeof(Text), Name = "Ver_Todos_Comercial")]
        VerTodos = 206,
        [Display(ResourceType = typeof(Text), Name = "Ver_Jerarquia")]
        VerJerarquia = 207,
        [Display(ResourceType = typeof(Text), Name = "Notificaciones_Mail_Todos")]
        NotificacionesMailTodos = 208,
        [Display(ResourceType = typeof(Text), Name = "Notificaciones_Mail_Jerarquia")]
        NotificacionesMailJerarquia = 209,        
        [Display(ResourceType = typeof(Text), Name = "Mail_SIO")]
        MailSio = 210,
        [Display(ResourceType = typeof(Text), Name = "Ver_Todos_Tipos_Negocios")]
        VerTodosTiposNegocios = 211,
        [Display(ResourceType = typeof(Text), Name = "Lista_Comercial")]
        ListaComercial = 212 ,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_CompraNet")]
        VisualizarCompraNet = 300,
        [Display(ResourceType = typeof(Text), Name = "Nuevo_Negocios")]
        NuevoNegocios = 301,
        [Display(ResourceType = typeof(Text), Name = "Modificar_Negocios")]
        ModificarNegocios = 302,
        [Display(ResourceType = typeof(Text), Name = "Ampliar_Negocios")]
        AmpliarNegocios = 303,
        [Display(ResourceType = typeof(Text), Name = "Finalizar_Negocios")]
        FinalizarNegocios = 304,
        [Display(ResourceType = typeof(Text), Name = "Rechazar_Negocios")]
        RechazarNegocios = 305,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_Hedge")]
        VisualizarHedge = 306,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_Pizarra")]
        VisualizarPizarra = 307,
        [Display(ResourceType = typeof(Text), Name = "Lista_Comercial_CompraNet")]
        ListaComercialCompraNet = 308,
        [Display(ResourceType = typeof(Text), Name = "Ver_Mesa")]
        VerMesa = 309,
        [Display(ResourceType = typeof(Text), Name = "Mail_Hedge")]
        MailHedge = 310,
        [Display(ResourceType = typeof(Text), Name = "Confirmar_Negocio")]
        ConfirmarNegocio = 311,
        [Display(ResourceType = typeof(Text), Name = "Ver_Todos_Negocios")]
        VerTodosNegocios = 312,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_Reporte_Agenda")]
        VisualizarReporteAgenda = 401,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_Reporte_Proveedor")]
        VisualizarReporteProveedor = 402,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_Indicadores")]
        VisualizarIndicadores = 403,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_Informe_Comercial")]
        VisualizarInformeComercial = 404,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_Informe_Administrativo")]
        VisualizarInformeAdministrativo = 405,
        [Display(ResourceType = typeof(Text), Name = "Descargar_Informe_Administrativo")]
        DescargarInformeAdministrativo = 406,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_Reporte_CompraNet")]
        VisualizarReporteCompraNet = 407,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_Compras_Diarias")]
        VisualizarComprasDiarias = 408,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_Reporte_Cupos")]
        VisualizarReporteCupo = 409,
        [Display(ResourceType = typeof(Text), Name = "Configuracion_Canal_Operacion")]
        ConfiguracionCanalOperacion = 501,
        [Display(ResourceType = typeof(Text), Name = "Configuracion_Entrega_A")]
        ConfiguracionEntregaA = 502,
        [Display(ResourceType = typeof(Text), Name = "Configuracion_Condicion")]
        ConfiguracionCondicion = 503,
        [Display(ResourceType = typeof(Text), Name = "Configuracion_Usuarios")]
        ConfiguracionUsuarios = 504,
        [Display(ResourceType = typeof(Text), Name = "Configuracion_Centros")]
        ConfiguracionCentros = 505,
        [Display(ResourceType = typeof(Text), Name = "Configuracion_Rangos_Precios")]
        ConfiguracionRangosPrecios = 506,        
        [Display(ResourceType = typeof(Text), Name = "Configuracion_Rangos_Confirmacion")]
        ConfiguracionRangosConfirmacion = 507,
        [Display(ResourceType = typeof(Text), Name = "Configuracion_Tareas_Programadas")]
        ConfiguracionTareasProgramadas = 508,
        [Display(ResourceType = typeof(Text), Name = "Configuracion_Operador")]
        ConfiguracionOperador = 509,
        [Display(ResourceType = typeof(Text), Name = "Configuracion_Diferencial")]
        ConfiguracionDiferencial = 510,
        [Display(ResourceType = typeof(Text), Name = "Configuracion_Zona_Girasol")]
        ConfiguracionZonaGirasol = 511,
        [Display(ResourceType = typeof(Text), Name = "Configuracion_Material")]
        ConfiguracionMaterial = 512,
        [Display(ResourceType = typeof(Text), Name = "Configuracion_Logs")]
        ConfiguracionLogs = 513,
        [Display(ResourceType = typeof(Text), Name = "Configuracion_Zona_Cupos")]
        ConfiguracionZonaCupos = 514,
        [Display(ResourceType = typeof(Text), Name = "Configuracion_Constantes")]
        ConfiguracionConstantes = 515,
        [Display(ResourceType = typeof(Text), Name = "Configuracion_Roles_Permisos")]
        ConfiguracionRolesPermisos = 516,
        [Display(ResourceType = typeof(Text), Name = "Datos_Research")]
        DatosResearch = 601,
        [Display(ResourceType = typeof(Text), Name = "Reporte_Research")]
        ReporteResearch = 602,
        [Display(ResourceType = typeof(Text), Name = "Notificaciones_Research")]
        NotificacionesResearch = 603,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_Cupos")]
        VisualizarCupos = 700,
        [Display(ResourceType = typeof(Text), Name = "Alta_Cupos")]
        AltaCupos = 701,
        [Display(ResourceType = typeof(Text), Name = "Modificar_Cupos")]
        ModificarCupos = 702,
        [Display(ResourceType = typeof(Text), Name = "Anular_Cupos")]
        AnularCupos = 703,
        [Display(ResourceType = typeof(Text), Name = "Administracion_Cupos")]
        AdministracionCupos = 704,        
        [Display(ResourceType = typeof(Text), Name = "Home_Corredor")]
        HomeCorredor = 801,
        [Display(ResourceType = typeof(Text), Name = "Notificaciones_Corredor")]
        NotificacionesCorredor = 802,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_Proveedor_De_Corredor")]
        VisualizarProveedorDeCorredor = 803,
        [Display(ResourceType = typeof(Text), Name = "Nuevo_Negocio_Corredor")]
        NuevoNegocioCorredor = 804,
        [Display(ResourceType = typeof(Text), Name = "Modificar_Negocio_Corredor")]
        ModificarNegocioCorredor = 805,
        [Display(ResourceType = typeof(Text), Name = "Ampliar_Negocio_Corredor")]
        AmpliarNegocioCorredor = 806,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_Reporte_CompraNet_Corredor")]
        VisualizarReporteCompraNetCorredor = 807,
    }
}