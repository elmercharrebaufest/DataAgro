var viewModel;

var datosIniActividad;

var data = null;

$(document).ready(function () {
    InicializarElementos();

    CrearViewModel();

    AsignarBotones();

    InicializarCombos()
});

function InicializarElementos() {
    kendo.culture("es-AR");

    $("#Proveedores").kendoDropDownList({
        dataTextField: "RazonSocial",
        dataValueField: "ProveedorId"
    });

    $("#Proveedores").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#Proveedores").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#TiposActividades").kendoDropDownList({
        dataTextField: "Descripcion",
        dataValueField: "TipoActividadId"
    });

    $("#TiposActividades").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#TiposActividades").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    var hoy = new Date();
    var anio = hoy.getFullYear();
    var mes = hoy.getMonth();
    var dia = hoy.getDate();
    if (mes < 10) {
        mes = "0" + mes.toString();
    }

    var mesPost = hoy.getMonth() + 1;
    if (mesPost < 10) {
        mesPost = "0" + mesPost.toString();
    }

    if (dia < 10) {
        dia = "0" + dia.toString();
    }

    var date = new Date(anio, mes, dia);
    var datehasta = new Date(anio, mesPost, dia);

    $("#fechaDesde").kendoDatePicker({
        value: date,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });
    $("#fechaHasta").kendoDatePicker({
        value: datehasta,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });

    //$("#FechaDesde").data("kendoDatePicker").value(date);
    //$("#FechaHasta").data("kendoDatePicker").value(datehasta);
}

function InicializarCombos() {
    var funcReturn = function (data) {
        if (ExistsErrorMessages(data.Errores)) {
            ShowErrorMessages(data.Errores);
        }
        else {
            datosIniActividad = data;
            AsignarCombos();
        }
    }

    MSExecuteURLOnServerAsync('/Agenda/Inicializar', funcReturn, '', false);
}

function CrearViewModel() {
    //var ResultadosDataSource = CrearResultadosDataSource([]);

    var param = {
        "TipoActividadId": null,
        "ActividadDetalle": null,
        "ProveedorId": null,
        "FechaDesde": null,
        "FechaHasta": null,
    };

    viewModel = kendo.observable({
        //Resultados: ResultadosDataSource,

        Parametros: param,

        TipoActividadCombo: [],
        ProveedorCombo: [],
    });

    kendo.bind($("#Agenda"), viewModel);
}

function CrearResultadosDataSource(datos) {
    var ds = new kendo.data.DataSource({
        data: datos,
        schema: {
            model: {
                fields: {
                    CondicionId: { type: "number", editable: false },
                    Descripcion: { type: "string", editable: false },
                }
            }
        },
    });

    return ds;
}

function AsignarCombos() {
    datosIniActividad.Datos.TiposActividades.push({
        TipoActividadId: null,
        Descripcion: "Todas las Actividades"
    });
    datosIniActividad.Datos.TiposActividades.sort(function (a, b) {
        var a1 = a.TipoActividadId, b1 = b.TipoActividadId;
        if (a1 == b1) return 0;
        return a1 > b1 ? 1 : -1;
    });

    datosIniActividad.Datos.Proveedores.push({
        ProveedorId: null,
        RazonSocial: "Todos los Proveedores"
    });

    datosIniActividad.Datos.Proveedores.sort(function (a, b) {
        var a1 = a.ProveedorId, b1 = b.ProveedorId;
        if (a1 == b1) return 0;
        return a1 > b1 ? 1 : -1;
    });

    viewModel.set("TipoActividadCombo", datosIniActividad.Datos.TiposActividades);
    viewModel.set("ProveedorCombo", datosIniActividad.Datos.Proveedores);
}

function AsignarBotones() {
    kendo.culture("es-AR");
    $("#butExport").click(function () {
        Exportar();
    });

    $("#butPrevia").click(function () {
        VistaPrevia();
    });

    $("#butCancelar").click(function () {
        Cancelar();
    });
}

function VistaPrevia() {
    var detalle = viewModel.get("Parametros.ActividadDetalle");
    var TipoActividad = viewModel.get("Parametros.TipoActividadId.TipoActividadId");
    var Proveedor = viewModel.get("Parametros.ProveedorId.ProveedorId");
    var FechaDesde = viewModel.get("Parametros.FechaDesde");
    var FechaHasta = viewModel.get("Parametros.FechaHasta");

    if (FechaDesde != null && FechaHasta != null) {
        if (FechaHasta < FechaDesde) {
            MensErr("La fecha hasta no puede ser menor que la fecha desde");
            return false;
        }
    }

    var oParam = {
        "ActividadDetalle": detalle,
        "TipoActividad": TipoActividad,
        "ProveedorId": Proveedor,
        "FechaDesde": FechaDesde,
        "FechaHasta": FechaHasta,
        "ActiveDirectoryId": "0",
    }

    var result = MSExecuteOnServer('/Agenda/VistaPreviaAgenda', oParam);

    for (var ii in result) {
        (function (i) {
            result[i].FechaHoraRecordatorio = kendo.toString(kendo.parseDate(result[i].FechaHoraRecordatorio), "dd/MM/yy HH:mm");
        })(ii);
    }

    armarResultado(result);

    $("#volver").click(function () {
        volver();
    });
}

function armarResultado(result) {
    $("#Agenda").hide();
    $(".cuerpo-tabla").empty();
    $(".formulario").hide();
    $(".resultado-reporteagenda").show();
    $(".Titulo").html("Resultados").css({ "margin": 20, 'text-align': 'center' });
    kendo.culture("es-AR");

    $("#grid").kendoGrid({
        dataSource: result,
        height: 472,
        //groupable: true,
        sortable: true,
        pageable: {
            refresh: true
        },
        filterable: {
            extra: false,
            messages: {
                info: "Filtros:",
                filter: "Filtrar",
                clear: "Limpiar",
                isTrue: "SI",
                isFalse: "NO",
                and: "Y",
                or: "O"
            },
            operators: {
                string: {
                    eq: "Igual",
                    neq: "Distinto",
                    startswith: "Comienza con",
                    contains: "Contiene",
                    endswith: "Finaliza con"
                },
                date: {
                    eq: "Igual",
                    neq: "Distinto",
                    gte: "Después o igual a",
                    gt: "Después",
                    lte: "Antes o igual a",
                    lt: "Antes",
                },
                number: {
                    eq: "Igual a",
                    neq: "Distinto a",
                    gte: "Mayor que o igual a",
                    gt: "Mayor que",
                    lte: "Menor que o igual a",
                    lt: "Menor que"
                }
            }
        },
        columns: [{
            field: "RazonSocial",
            title: "Proveedor",
            width: 220,
            filterable: true
        }, {
            field: "Detalle",
            title: "Detalle de Actividad",
            width: 220,
            filterable: true
        }, {
            field: "TipoDeAcividad",
            title: "Tipo de Actividad",
            width: 220,
            filterable: true
        }, {
            field: "FechaHoraRecordatorio",
            title: "Fecha Recordatorio",
            width: 150,
            filterable: true
        }, {
            field: "Apellido",
            title: "Comercial",
            width: 220,
            filterable: true
        }, {
            field: "NombreContacto",
            title: "Contacto Comercial",
            width: 200,
            filterable: true
        }]
    });

    var grid = $("#grid").data("kendoGrid");
    grid.dataSource.pageSize(12);
    grid.refresh();
}

function volver() {
    $(".resultado-reporteagenda").hide();
    $(".Titulo").html("Reporte de Proveedor").css({ "margin": '', 'text-align': '' })
    $(".formulario").show();
    $("#Agenda").show();
}

function Exportar() {
    var detalle = viewModel.get("Parametros.ActividadDetalle");
    var TipoActividad = viewModel.get("Parametros.TipoActividadId.TipoActividadId");
    var Proveedor = viewModel.get("Parametros.ProveedorId.ProveedorId");
    var FechaDesde = viewModel.get("Parametros.FechaDesde");
    var FechaHasta = viewModel.get("Parametros.FechaHasta");

    if (FechaDesde != null && FechaHasta != null) {
        if (FechaHasta < FechaDesde) {
            MensErr("La fecha hasta no puede ser menor que la fecha desde");
            return false;
        }
    }

    var oParam = {
        "ActividadDetalle": detalle,
        "TipoActividad": TipoActividad,
        "ProveedorId": Proveedor,
        "FechaDesde": FechaDesde,
        "FechaHasta": FechaHasta,
        "ActiveDirectoryId": "0",
    }

    var result = MSExecuteOnServer('/Agenda/ExportarAgenda', oParam);

    if (result != null) {
        if (result.DownloadKey.length > 0) {
            var url = MSGetUrl('/DownLoad/Reporte?key=' + result.DownloadKey);
            window.location = url;
        }
        else {
            MensErr("No existen datos con ese filtro para este Comercial, por favor ajuste los filtros");
            return false;
        }
    }
}