var externo;
var anulacion;
$(document).ready(function () {
    $('#menuproveedor').hide();
    kendo.culture("es-AR");
    externo = ConvertirStringABool(externo);
    inicializarTodosKendoDate($(".filtroFecha"));
    inicializarElementos();
    InicializarCuposIndex();
    anulacion = ConvertirStringABool(anulacion);
    $("#pageSize").kendoDropDownList();
    $(".ocultarAnular").change(function () {
        $('#anulacionMasivaDiv').hide();
    });
    $(".k-i-loading").hide();
});


function InicializarCuposIndex() {

    var ds = {
        transport: {
            parameterMap: function (options, operation) {

                if (operation == "read") {
                    return JSON.stringify(options)
                }
                if (options.filter) {
                    KendoGrid_FixFilter(ds, options.filter);
                }
                return options;
            },
            read: {
                type: 'post',
                dataType: 'json',
                contentType: "application/json",
                url: '/ReporteCupo/BuscaDatosTabla',
                data: function () {
                    let defaultFiltros = [
                        { field: "FechaGeneracion", operator: "eq", value: new Date() }
                    ];

                    let filtroCompleto = TraerFiltrosConValores();

                    if (filtroCompleto.filter == null) {
                        filtroCompleto.filter = new FiltroPadre("and", defaultFiltros);
                        $("#fechaIngresoDesdeId").val(new Date().toLocaleDateString("es-AR").replace(new RegExp('/', 'g'), '-'));
                        $("#fechaIngresoHastaId").val(new Date().toLocaleDateString("es-AR").replace(new RegExp('/', 'g'), '-'));
                    }

                    //ConvertirFechaRegistroAString(filtroCompleto.filter.filters);

                    return filtroCompleto;
                }
            }
        },
        schema: {
            data: 'Data',
            total: 'Total',
            model: {
                id: 'Id',
                fields: {
                    FechaIngreso: { type: "date" },
                    FechaGeneracion: { type: "date" },
                    FleteProcedencia: { type: "boolean" },
                    Sustentable: { type: "boolean" },
                    EPA: { type: "boolean" },
                    EUDR: { type: "boolean" },
                    ConDescarga: { type: "boolean" },
                    Cumplimiento: { type: "boolean" },
                    CTGFechaDesde: { type: "date" },
                    CTGFechaDesde: { type: "date" },
                    Peso: { type: "number" },
                    Km: { type: "number" }

                }
            }
        },
        pageable: {
            numeric: true,
            refresh: true,
            pageSize: 20,
            previousNext: true,
            input: true,
            info: true
        },
        pageSize: 20,
        serverPaging: true,
        serverSorting: true,
        sort: [
        ],
        serverFiltering: true
    };

    $("#grid").kendoGrid({
        toolbar: ["excel"],
        excel: {
            fileName: "Reporte de Cupos.xlsx",
            allPages: true
        },
        dataSource: ds,
        dataBound: function () {
            var grid = $("#grid").data("kendoGrid");
            if (externo) {
                grid.hideColumn("ZonaCupo");
            }
            var view = grid.dataSource.view();
            for (var i = 0; i < view.length; i++) {
                if (view[i].FleteProcedencia) {
                    grid.tbody.find("tr[data-uid='" + view[i].uid + "']")
                        .addClass("flete-procedencia");
                }
                $('[data-toggle="tooltip"]').tooltip();
            }
            $("td:has(div.statusexterno)").css('border-bottom', '5px solid #15deca');
        },
        columns: [
            { selectable: true },
            {
                field: "FechaIngreso", title: externo ? "Fecha de Cupo" : "Fecha de ingreso", type: "date", width: 150, format: _DefaultDateTemplate, template: function (dataItem) {

                    if (dataItem.UsuarioCreador != null || externo) {
                        return '<div class="statusexterno "></div>' + kendo.toString(dataItem.FechaIngreso, "dd/MM/yyyy");
                    }
                    return '<div class=" "></div>' + kendo.toString(dataItem.FechaIngreso, "dd/MM/yyyy");
                }
            },
            { field: "CupoSap", title: "Cupo", type: "string", width: 150 },
            { field: "Material", type: "string", width: 150 },
            { field: "Proveedor", type: "string", width: 150 },
            { field: "Destinatario", type: "string", width: 150 },
            { field: "Centro", title: "Planta", type: "string", width: 150 },
            { field: "Calidad", type: "string", width: 150 },
            { field: "ZonaCupo", title: "Zona", type: "string", width: 150 },
            { field: "FleteProcedencia", title: "Flete", type: "string", width: 150, template: function (dataItem) { return dataItem.FleteProcedencia ? "Si" : "No"; } },
            { field: "Sustentable", title: "Sustentable", type: "string", width: 150, template: function (dataItem) { return dataItem.Sustentable ? "Si" : "No"; } },
            { field: "EPA", title: "EPA", type: "string", width: 150, template: function (dataItem) { return dataItem.EPA ? "Si" : "No"; } },
            { field: "EUDR", title: "EUDR", type: "string", width: 150, template: function (dataItem) { return dataItem.EUDR ? "Si" : "No"; } },
            { field: "ConDescarga", title: "Con Descarga", type: "string", width: 150, template: function (dataItem) { return dataItem.ConDescarga ? "Si" : "No"; } },
            { field: "CupoStop", title: "Cupo STOP", type: "string", width: 150 },
            {
                field: "EstadoCupo", title: "Estado", sortable: false, width: 200,
                template: function (dataItem) {
                    if (dataItem.EstadoCupoId == 1) {
                        return '<div class="status sinctg"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 2) {
                        return '<div class="status activado"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 3) {
                        return '<div class="status arribado"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 5) {
                        return '<div class="status descargado"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 4) {
                        return '<div class="status anulado"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 6) {
                        return '<div class="status sinstop"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 7) {
                        return '<div class="status error" data-toggle="tooltip" data-placement="top" title="' + dataItem.MensajeError + '"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 8) {
                        return '<div class="status sinstop"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 9) {
                        return '<div class="status anulado"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 10) {
                        return '<div class="status sinstop"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 11) {
                        return '<div class="status sinctg"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    }
                }
            },
            { field: "Comercial", type: "string", width: 150 },
            { field: "Observaciones", type: "string", width: 150, hidden: externo },
            { field: "FechaGeneracion", title: "Fecha de registro", type: "date", width: 150, format: _DefaultDateTemplate },
            { field: "Hora", title: "Hora", type: "date", width: 150 },
            { field: "EstadoPlanta", title: "Estado en Planta", type: "string", width: 150 },
            { field: "CartaPorte", title: "Carta de Porte", type: "string", width: 150 },
            { field: "CTG", title: "CTG", type: "string", width: 150 },
            { field: "CTGFechaDesde", title: "Desde CTG", type: "date", width: 150, format: _DefaultDateTemplate },
            { field: "CTGFechaHasta", title: "Hasta CTG", type: "date", width: 150, format: _DefaultDateTemplate },
            { field: "CuitOrigen", title: "Cuit Origen", type: "string", width: 150 },
            { field: "CuitOrigenAfip", title: "Cuit Origen Afip", type: "string", width: 150 },
            { field: "CodLocalidadOrigen", title: "Cod Localidad", type: "string", width: 150 },
            { field: "NroEstablecimientoOrigen", title: "Nro Establecimiento", type: "string", width: 150 },
            { field: "RemitenteComercial", title: "Remitente Comercial", type: "string", width: 150 },
            { field: "CorredorComprador", title: "Corredor Comprador", type: "string", width: 150 },
            { field: "CorredorVendedor", title: "Corredor Vendedor", type: "string", width: 150 },
            { field: "MercadoATermino", title: "Mercado a Termino", type: "string", width: 150 },
            { field: "Cosecha", type: "string", width: 150 },
            { field: "Peso", type: "number", width: 150, format: "{0:n0}", title: "Kg" },
            { field: "Km", type: "number", width: 150 },
            { field: "IntermediarioFlete", title: "Intermediario Flete", type: "string", width: 150 },
            { field: "Transportista", type: "string", width: 150 },
            { field: "Chofer", type: "string", width: 150 },
            { field: "MotivoRechazo", title: "Motivo de Rechazo", type: "string", width: 150 },
            { field: "Cumplimiento", title: "Cumplimiento", type: "string", width: 150, template: function (dataItem) { return dataItem.Cumplimiento ? "Si" : "No"; } },
        ],
        pageable: {
            messages: {
                display: "{2} elementos",
                empty: "No hay elementos para mostrar",
                page: "P&aacute;gina",
                allPages: "Todas",
                of: "de {0}",
                itemsPerPage: "Elementos por p&aacute;gina",
                first: "Ir a la primer p&aacute;gina",
                previous: "Ir a la p&aacute;gina anterior",
                next: "Ir a la p&aacute;gina siguiente",
                last: "Ir a la &uacute;ltima p&aacute;gina",
                refresh: "Recargar"
            },
            input: true,
            numeric: true
        },
        scrollable: false,
        sortable: {
            mode: "multiple",
            allowUnsort: true,
            showIndexes: false
        },
        excelExport: function (e) {
            var sheet = e.workbook.sheets[0];
            var templateflete = kendo.template(this.columns[9].template);
            var templatesustentable = kendo.template(this.columns[10].template);
            var templateEPA = kendo.template(this.columns[11].template);
            var templateEUDR = kendo.template(this.columns[12].template);
            var templateConDescarga = kendo.template(this.columns[13].template);
            var templateCumplimiento = kendo.template(this.columns[40].template);

            for (var i = 1; i < sheet.rows.length; i++) {
                var row = sheet.rows[i];

                var dataitem = { fleteprocedencia: row.cells[8].value };
                row.cells[8].value = templateflete(dataitem);

                var dataitemSustentable = { Sustentable: row.cells[9].value };
                row.cells[9].value = templatesustentable(dataitemSustentable);

                var dataitemEPA = { EPA: row.cells[10].value };
                row.cells[10].value = templateEPA(dataitemEPA);

                var dataitemEUDR = { EUDR: row.cells[11].value };
                row.cells[11].value = templateEUDR(dataitemEUDR);

                var dataitemConDescarga = { ConDescarga: row.cells[12].value };
                row.cells[12].value = templateConDescarga(dataitemConDescarga);

                var dataitemCumplimiento = { Cumplimiento: row.cells[39].value };
                row.cells[39].value = templateCumplimiento(dataitemCumplimiento);

                //la fecha en chrome aparece corrida un dia, solucion:
                var fecha = row.cells[0].value;

                if (fecha != null) {
                    fecha = fecha.setHours(fecha.getHours() + 1);
                    row.cells[0].value = new Date(fecha);
                }
            }
        },
        persistSelection: true,
    });

    $("#gridNoEliminar").kendoGrid({
        columns: [
            { field: "CupoSap", title: "Cupo", type: "string", width: 65 },
            { field: "Proveedor", type: "string", width: 150 },
            { field: "Destinatario", type: "string", width: 50 },
        ]
    });
}


function Filtrar() {
    clearSelectionGrid();
    var grid = $('#grid').data('kendoGrid').dataSource;
    grid.read();
    setTimeout(function () {
        if (anulacion == true && ($("#EstadoId").val() == "1" || $("#EstadoId").val() == "6") && grid.data().length > 0) {
            $('#anulacionMasivaDiv').show();
        } else {
            $('#anulacionMasivaDiv').hide();
        }
    }, 500)

}


function inicializarElementos() {

    CrearMultiSelectFiltro("#buscadorProveedor", "Proveedor", "ProveedorId", "/ReporteCupo/BuscarProveedor");
    $("#ZonaId").kendoMultiSelect({
        open: function (e) { $("#ZonaId").data("kendoMultiSelect").value(''); },
        dataTextField: 'Text',
        dataValueField: 'Value',
        autoClose: false,
        autoBind: false,
    }).data("kendoMultiSelect").value('');

    $("#MaterialId").kendoMultiSelect({
        open: function (e) { $("#MaterialId").data("kendoMultiSelect").value(''); },
        dataTextField: 'Text',
        dataValueField: 'Value',
        autoClose: false,
        autoBind: false,
    }).data("kendoMultiSelect").value('');

    $("#EstadoId").kendoMultiSelect({
        open: function (e) { $("#EstadoId").data("kendoMultiSelect").value(''); },
        dataTextField: 'Text',
        dataValueField: 'Value',
        autoClose: false,
        autoBind: false,
    }).data("kendoMultiSelect").value('');

    $("#ComercialId").kendoMultiSelect({
        open: function (e) { $("#ComercialId").data("kendoMultiSelect").value(''); },
        dataTextField: 'Text',
        dataValueField: 'Value',
        autoClose: false,
        autoBind: false,
    }).data("kendoMultiSelect").value('');

    $("#CentroId").kendoMultiSelect({
        open: function (e) { $("#CentroId").data("kendoMultiSelect").value(''); },
        dataTextField: 'Text',
        dataValueField: 'Value',
        autoClose: false,
        autoBind: false,
    }).data("kendoMultiSelect").value('');


    $("#Destinatario").bind("paste", function (e) {
        e.preventDefault();
        if (e.originalEvent.clipboardData !== undefined) {
            clipText = e.originalEvent.clipboardData.getData('text/plain');
        } else {
            clipText = window.clipboardData.getData('text');
        }
        $("#Destinatario").val(clipText.replace(/(\r\n|\n|\r)/gm, ";"));
        CambioVariosContratos();
    });

    inicializarPopUpSap("Cupos");



}



function mostrarocultar(element) {
    if ($(element).text() == "Mostrar") {
        $(element).text("Ocultar");
    } else {
        $(element).text("Mostrar");
    }
}
function deseleccionarRadioButton() {
    $('[name=FleteProcedencia]:checked').prop('checked', false);
}
function deseleccionarRadioButtonSustentable() {
    $('[name=Sustentable]:checked').prop('checked', false);
}
function deseleccionarRadioButtonEPA() {
    $('[name=EPA]:checked').prop('checked', false);
}
function deseleccionarRadioButtonEUDR() {
    $('[name=EUDR]:checked').prop('checked', false);
}
function deseleccionarRadioButtonConDescarga() {
    $('[name=ConDescarga]:checked').prop('checked', false);
}
function deseleccionarRadioButtonCumplimiento() {
    $('[name=Cumplimiento]:checked').prop('checked', false);
}

function ConvertirFechaRegistroAString(filtros) {
    arr = [];

    filtros.forEach(function (x) {
        (x.field == "FechaRegistro") ? arr.push(x) : null;
    });

    if (arr != null) {
        //se usa asi porque usando toLocaleDateString no trae valores igual al dia de hoy
        arr.forEach(function (x) { x.value = deFechaAString(x.value) });
    }
}

function AnulacionMasiva() {
    var grid = $("#grid").data("kendoGrid");
    var selectedIds = grid.selectedKeyNames();
    if (selectedIds.length > 0) {
        var ids = new Array();
        for (var i = 0; i < selectedIds.length; i++) {
            ids.push(
                { field: "Id", operator: "eq", value: parseInt(selectedIds[i]) }
            );
        }
        var filter = {
            logic: "and",
            filters: [
                {
                    logic: "or",
                    filters: ids
                }
            ]
        };
        var request = {
            filter: filter,
            page: 1,
            skip: 0,
            take: null,
            sort: null
        };
        datos = MSExecuteOnServer('/ReporteCupo/BuscaDatosTabla', request);
        if (datos.Data.length == 0) {
            MensErr("Ningun cupo seleccionado");
        } else {
            //$("#cuposSeleccionados").html("");
            //for (var i = 0; i < datos.Data.length; i++) {
            //    $("#cuposSeleccionados").append(datos.Data[i].CupoSap + " " + datos.Data[i].Destinatario + " " + datos.Data[i].Proveedor + "<br>");
            //}
            var ds2 = new kendo.data.DataSource({ data: datos.Data });
            $("#gridNoEliminar").data("kendoGrid").setDataSource(ds2);
            $("#modalConfirmarAnulacion").modal("show");
        }
    } else {
        MensErr("Ningun cupo seleccionado");
    }

}

function AnulacionMasivaConfirmacion() {

    //let filtroCompleto = TraerFiltrosConValores();
    //if (filtroCompleto.filter == null) {
    //    filtroCompleto.filter = new FiltroPadre("and", defaultFiltros);
    //    $("#fechaIngresoDesdeId").val(new Date().toLocaleDateString("es-AR").replace(new RegExp('/', 'g'), '-'));
    //    $("#fechaIngresoHastaId").val(new Date().toLocaleDateString("es-AR").replace(new RegExp('/', 'g'), '-'));
    //}

    var grid = $("#grid").data("kendoGrid");
    var selectedIds = grid.selectedKeyNames();
    //if (selectedIds.length > 0) {
    //    var ids = new Array();
    //    for (var i = 0; i < selectedIds.length; i++) {
    //        ids.push(
    //            { field: "Id", operator: "neq", value: parseInt(selectedIds[i]) }
    //        );
    //    }
    //    filtroCompleto.filter.filters.push({ logic: "and", filters: ids });
    //}

    var mensaje = MSExecuteOnServer('/ReporteCupo/AnulacionMasiva', { ids: selectedIds });
    if (mensaje == "Ningún cupo para anular") {
        MensErr(mensaje);

    } else {
        MensInfoReload(mensaje);
    }
}

function clearSelectionGrid() {
    var grid = $("#grid").data("kendoGrid");
    grid._selectedIds = {};
    grid.clearSelection();
}

function setPageSize() {
    var grid = $("#grid").data("kendoGrid");
    grid.dataSource.pageSize($("#pageSize").val());
    grid.refresh();
}

