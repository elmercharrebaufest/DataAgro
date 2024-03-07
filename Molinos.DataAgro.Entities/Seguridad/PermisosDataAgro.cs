using System.ComponentModel.DataAnnotations;
using Molinos.DataAgro.Entities.Resources;

namespace Molinos.DataAgro.Entities.Seguridad
{
    public enum PermisosDataAgro
    {
        [Display(ResourceType = typeof(Text), Name = "Ingreso_DataAgro")]
        IngresoDataAgro = 0,

        [Display(ResourceType = typeof(Text), Name = "Recibir_Mail_SugerenciaFAQ")]
        Recibir_Mail_SugerenciaFAQ = 50,

        //Proveedor
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
        [Display(ResourceType = typeof(Text), Name = "Subida_Archivos_KMZ")]
        SubidaArchivosKMZ = 112,
        [Display(ResourceType = typeof(Text), Name = "ProveedorZonaPropia")]
        ProveedorZonaPropia = 113,
        [Display(ResourceType = typeof(Text), Name = "ModificarRazonSocial")]
        ModificarRazonSocial = 114,
        [Display(ResourceType = typeof(Text), Name = "DeshabilitarProveedor")]
        DeshabilitarProveedor = 115,
        [Display(ResourceType = typeof(Text), Name = "OcultarCamposEditar")]
        OcultarCamposEditar = 116,
        [Display(ResourceType = typeof(Text), Name = "ReasignarProveedorPantallaCupo")]
        ReasignarProveedorPantallaCupo = 117,
        [Display(ResourceType = typeof(Text), Name = "Proveedor_Apoderados")]
        ProveedorApoderados = 118,

        //Comercial
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
        ListaComercial = 212,

        //CompraNet
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
        [Display(ResourceType = typeof(Text), Name = "Mail_Pesificado")]
        MailPesificado = 311,
        [Display(ResourceType = typeof(Text), Name = "Crear_Negocios_Fason")]
        CrearNegociosFason = 312,
        [Display(ResourceType = typeof(Text), Name = "Crear_Negocios_Agente")]
        CrearNegociosAgente = 313,
        [Display(ResourceType = typeof(Text), Name = "Crear_Negocios_Acuerdos")]
        CrearNegociosAcuerdos = 314,
        [Display(ResourceType = typeof(Text), Name = "Crear_Negocios_Confirmados")]
        NegociosConfirmados = 315,
        [Display(ResourceType = typeof(Text), Name = "Modificar_Neg_Finalizados")]
        ModificarNegFinalizados = 316,
        [Display(ResourceType = typeof(Text), Name = "Configuraciones_Internas")]
        ConfiguracionesInternas = 317,
        [Display(ResourceType = typeof(Text), Name = "Ver_Todos_Negocios")]
        VerTodosNegocios = 318,
        [Display(ResourceType = typeof(Text), Name = "Confirmar_NegocioOrigNorte")]
        ConfirmarNegocioOrigNorte = 319,
        [Display(ResourceType = typeof(Text), Name = "Confirmar_NegocioOrigSur")]
        ConfirmarNegocioOrigSur = 320,
        [Display(ResourceType = typeof(Text), Name = "Confirmar_NegocioOrigCentro")]
        ConfirmarNegocioOrigCentro = 321,
        [Display(ResourceType = typeof(Text), Name = "Confirmar_NegocioCorredoresBsAs")]
        ConfirmarNegocioCorredoresBsAs = 322,
        [Display(ResourceType = typeof(Text), Name = "Confirmar_NegocioCorredoresRosario")]
        ConfirmarNegocioCorredoresRosario = 323,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_OcultarEnTablero")]
        OcultarEnTablero = 324,
        [Display(ResourceType = typeof(Text), Name = "Pre_Anular")]
        PreAnular = 325,
        [Display(ResourceType = typeof(Text), Name = "Anular")]
        Anular = 326,
        [Display(ResourceType = typeof(Text), Name = "Crear_Negocios_FechaMayorDiaAnterior")]
        NegociosFechaMayorDiaAnterior = 327,
        [Display(ResourceType = typeof(Text), Name = "Configuraciones_JefeEnvioMailNegociosConDiaAnterior")]
        JefeEnvioMailNegociosConDiaAnterior = 328,
        [Display(ResourceType = typeof(Text), Name = "NegociosFechaMayorDiaAnteriorFijacion")]
        NegociosFechaMayorDiaAnteriorFijacion = 329,
        [Display(ResourceType = typeof(Text), Name = "EnvioMailNegociosConDiaAnteriorFijacion")]
        EnvioMailNegociosConDiaAnteriorFijacion = 330,
        [Display(ResourceType = typeof(Text), Name = "ModificarDolarizadoExpress")]
        ModificarDolarizadoExpress = 331,
        [Display(ResourceType = typeof(Text), Name = "ModificarCanje")]
        ModificarCanje = 332,
        [Display(ResourceType = typeof(Text), Name = "ModificarPrestamoDevolucion")]
        ModificarPrestamoDevolucion = 333,
        [Display(ResourceType = typeof(Text), Name = "ModificarDolarizadoFinalizado")]
        ModificarDolarizadoFinalizado = 334,
        [Display(ResourceType = typeof(Text), Name = "ModificarVenta")]
        ModificarVenta = 335,
        [Display(ResourceType = typeof(Text), Name = "NoRecibirMail")]
        NoRecibirMail = 336,
        [Display(ResourceType = typeof(Text), Name = "ModificarLimiteDolarizado")]
        ModificarLimiteDolarizado = 337,
        [Display(ResourceType = typeof(Text), Name = "ModificarLimitePesificado")]
        ModificarLimitePesificado = 338,
        [Display(ResourceType = typeof(Text), Name = "ModificarLimitePesificadoFinalizado")]
        ModificarLimitePesificadoFinalizado = 339,
        [Display(ResourceType = typeof(Text), Name = "ConfirmarNegociosMaiz")]
        ConfirmarNegociosMaiz = 340,
        [Display(ResourceType = typeof(Text), Name = "ConfirmarNegociosSoja")]
        ConfirmarNegociosSoja = 341,
        [Display(ResourceType = typeof(Text), Name = "ConfirmarNegociosTrigo")]
        ConfirmarNegociosTrigo = 342,
        [Display(ResourceType = typeof(Text), Name = "ConfirmarNegociosGirasol")]
        ConfirmarNegociosGirasol = 343,
        [Display(ResourceType = typeof(Text), Name = "ConfirmarNegociosGirasolAO")]
        ConfirmarNegociosGirasolAO = 344,
        [Display(ResourceType = typeof(Text), Name = "ModificarFijacionVirtual")]
        ModificarFijacionVirtual = 345,
        [Display(ResourceType = typeof(Text), Name = "EnvioMailNegociosAnulaYReemplaza")]
        EnvioMailNegociosAnulaYReemplaza = 346,
        [Display(ResourceType = typeof(Text), Name = "AsociarNegocio")]
        AsociarNegocio = 347,
        [Display(ResourceType = typeof(Text), Name = "ImporteSustentableEspecial")]
        ImporteSustentableEspecial = 348,
        [Display(ResourceType = typeof(Text), Name = "ActualizarCompraNet")]
        ActualizarCompraNet = 349,
        [Display(ResourceType = typeof(Text), Name = "Pre_AnularFijacion")]
        PreAnularFijacion = 350,
        [Display(ResourceType = typeof(Text), Name = "OyT_Norte")]
        OyT_Norte = 351,

        //Reportes
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
        [Display(ResourceType = typeof(Text), Name = "Visualizar_Reporte_Comercial")]
        VisualizarReporteComercial = 410,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_Reporte_De_Proveedores")]
        VisualizarReporteDeProveedores = 411,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_Reporte_Rango")]
        VisualizarReporteRango = 412,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_Reporte_Evolucion_Fijacion")]
        VisualizarReporteEvolucionFijacion = 413,
        [Display(ResourceType = typeof(Text), Name = "VisualizarReporteDolarizado")]
        VisualizarReporteDolarizado = 414,
        [Display(ResourceType = typeof(Text), Name = "VisualizarAnulacionEnReporteCupo")]
        VisualizarAnulacionEnReporteCupo = 415,
        [Display(ResourceType = typeof(Text), Name = "VisualizarReportePrecioMoaPizarra")]
        VisualizarReportePrecioMoaPizarra = 416,
        [Display(ResourceType = typeof(Text), Name = "VisualizarReportePagoDiferido")]
        VisualizarReportePagoDiferido = 417,
        [Display(ResourceType = typeof(Text), Name = "VisualizarReporteContratosAFijarPase")]
        VisualizarReporteContratosAFijarPase = 418,
        [Display(ResourceType = typeof(Text), Name = "NegocioPesificado")]
        NegocioPesificado = 419,
        [Display(ResourceType = typeof(Text), Name = "Mail_MATPrimary")]
        MailMATPrimary = 420,

        //Configuracion
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
        [Display(ResourceType = typeof(Text), Name = "Administracion_Logs")]
        LogDataAgro = 517,
        [Display(ResourceType = typeof(Text), Name = "Configuracion_Feriado")]
        ConfiguracionFeriado = 518,
        [Display(ResourceType = typeof(Text), Name = "Configuracion_Bolsa")]
        ConfiguracionBolsa = 519,
        [Display(ResourceType = typeof(Text), Name = "Administracion_Proveedores")]
        Administracion_Proveedores = 520,
        [Display(ResourceType = typeof(Text), Name = "Administracion_LogServer")]
        LogServer = 521,
        [Display(ResourceType = typeof(Text), Name = "Habilitacion_Boleto")]
        Habilitacion_Boleto = 522,
        [Display(ResourceType = typeof(Text), Name = "ConfigurarExcedente")]
        ConfigurarExcedente = 523,
        [Display(ResourceType = typeof(Text), Name = "Configuracion_Localidad")]
        ConfiguracionLocalidad = 524,

        //Research
        [Display(ResourceType = typeof(Text), Name = "Datos_Research")]
        DatosResearch = 601,
        [Display(ResourceType = typeof(Text), Name = "Reporte_Research")]
        ReporteResearch = 602,
        [Display(ResourceType = typeof(Text), Name = "Notificaciones_Research")]
        NotificacionesResearch = 603,

        //Cupos
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
        [Display(ResourceType = typeof(Text), Name = "Ver_Todos_Cupos")]
        VerTodosCupos = 705,
        [Display(ResourceType = typeof(Text), Name = "Reasignar_Flete")]
        ReasignarFlete = 706,
        [Display(ResourceType = typeof(Text), Name = "Administracion_EspacioDinamico")]
        AdministracionEspacioDinamico = 707,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_SugerenciaDeCupos")]
        SugerenciaDeCupos = 708,
        [Display(ResourceType = typeof(Text), Name = "Administracion_AlgoritimoDeCupos")]
        AlgoritimoDeCupos = 709,
        [Display(ResourceType = typeof(Text), Name = "Alta_OtrasZonas")]
        AltaOtrasZonas = 710,
        [Display(ResourceType = typeof(Text), Name = "Modificar_OtrasZonas")]
        ModificarOtrasZonas = 711,
        [Display(ResourceType = typeof(Text), Name = "Anular_OtrasZonas")]
        AnularOtrasZonas = 712,
        [Display(ResourceType = typeof(Text), Name = "Alta_OrigenSur")]
        AltaOrigenSur = 713,
        [Display(ResourceType = typeof(Text), Name = "Modificar_OrigenSur")]
        ModificarOrigenSur = 714,
        [Display(ResourceType = typeof(Text), Name = "Anular_OrigenSur")]
        AnularOrigenSur = 715,
        [Display(ResourceType = typeof(Text), Name = "Alta_OrigenCentro")]
        AltaOrigenCentro = 716,
        [Display(ResourceType = typeof(Text), Name = "Modificar_OrigenCentro")]
        ModificarOrigenCentro = 717,
        [Display(ResourceType = typeof(Text), Name = "Anular_OrigenCentro")]
        AnularOrigenCentro = 718,
        [Display(ResourceType = typeof(Text), Name = "Alta_CorredoresBsAs")]
        AltaCorredoresBsAs = 719,
        [Display(ResourceType = typeof(Text), Name = "Modificar_CorredoresBsAs")]
        ModificarCorredoresBsAs = 720,
        [Display(ResourceType = typeof(Text), Name = "Anular_CorredoresBsAs")]
        AnularCorredoresBsAs = 721,
        [Display(ResourceType = typeof(Text), Name = "Alta_CorredoresRosario")]
        AltaCorredoresRosario = 722,
        [Display(ResourceType = typeof(Text), Name = "Modificar_CorredoresRosario")]
        ModificarCorredoresRosario = 723,
        [Display(ResourceType = typeof(Text), Name = "Anular_CorredoresRosario")]
        AnularCorredoresRosario = 724,
        [Display(ResourceType = typeof(Text), Name = "Alta_OrigenNorte")]
        AltaOrigenNorte = 725,
        [Display(ResourceType = typeof(Text), Name = "Modificar_OrigenNorte")]
        ModificarOrigenNorte = 726,
        [Display(ResourceType = typeof(Text), Name = "Anular_OrigenNorte")]
        AnularOrigenNorte = 727,
        [Display(ResourceType = typeof(Text), Name = "SolicitudCupo")]
        SolicitudCupo = 728,
        [Display(ResourceType = typeof(Text), Name = "Disponibilidad_De_Cupos")]
        DisponibilidadDeCupos = 729,
        [Display(ResourceType = typeof(Text), Name = "Habilitacion_De_Cupos")]
        HabilitacionDeCupos = 730,
        [Display(ResourceType = typeof(Text), Name = "Ver_Todas_Las_Sugerencias")]
        VerTodasLasSugerencias = 731,
        [Display(ResourceType = typeof(Text), Name = "Mail_SolExt_Pendientes")]
        Mail_SolExt_Pendientes = 740,
        //Cupo NO Propio
        [Display(ResourceType = typeof(Text), Name = "Visualizar_CupoNoPropio")]
        Visualizar_CupoNoPropio = 732,
        [Display(ResourceType = typeof(Text), Name = "Disponibilidad_De_Cupos_Descarga")]
        DisponibilidadDeCuposDescarga = 733,
        [Display(ResourceType = typeof(Text), Name = "Cupos_Acopios_SolicitudExtraordinaria")]
        Cupos_Acopios_SolicitudExtraordinaria = 734,

        //Externos
        [Display(ResourceType = typeof(Text), Name = "Servicio_Auth")]
        ServicioAuth = 800,
        [Display(ResourceType = typeof(Text), Name = "Ingreso_Externo")]
        IngresoExterno = 801,
        [Display(ResourceType = typeof(Text), Name = "Nuevo_Negocio_Externo")]
        NuevoNegocioExterno = 803,
        [Display(ResourceType = typeof(Text), Name = "Modificar_Negocio_Externo")]
        ModificarNegocioExterno = 804,
        [Display(ResourceType = typeof(Text), Name = "Visualizar_Reporte_CompraNet_Externo")]
        VisualizarReporteCompraNetExterno = 805,
        [Display(ResourceType = typeof(Text), Name = "AltaCupo_Externo")]
        AltaCupo_Externo = 806,

        //Boletos
        [Display(ResourceType = typeof(Text), Name = "GenerarBoleto")]
        GenerarBoleto = 900,

    }
}