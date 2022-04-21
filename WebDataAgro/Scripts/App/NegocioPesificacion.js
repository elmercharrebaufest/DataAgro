var fechaString;
var url;
var viewModel;
var datosIniCrearContrato;
var configurarExcedente;

$(document).ready(function () {
    kendo.culture("es-AR");
    inicializarTodosKendoDate($(".filtroFecha"));
    $('#menuproveedor').hide();
    fechaString = ObtenerFechaDesde();
    Inicializar();
    setInterval(Refrescar, 300000);
    configurarExcedente = ConvertirStringABool(configurarExcedente);
    //DeseleccionarExcedente();
});

function Inicializar() {

    $("#fechaInstruccionId").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        change: function () {
        }
    });
    kendo.ui.FilterMultiCheck.prototype.options.messages =
        $.extend(true, kendo.ui.FilterMultiCheck.prototype.options.messages, {
            "selectedItemsFormat": ""
        });

    var ds = {
        transport: {
            parameterMap: function (options, operation) {

                if (operation == "read") {
                    options.take = 0;
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
                url: '/NegocioPesificacion/BuscaDatosTabla',

                data: function () {

                    let filtroCompleto = TraerFiltrosConValores();

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
                }
            }
        },

        /* serverPaging: false,*/
        //serverSorting: false,
        /*  serverFiltering: true,*/
        //sort: [
        //    { field: "Material", dir: "desc" }
        //],
        //pageSize: 20,
        aggregate: [
            { field: "Cantidad", aggregate: "sum" },
            { field: "KgVencimientoPesificable", aggregate: "sum" },
            { field: "KgNoPesificable", aggregate: "sum" },
            { field: "KgTotales", aggregate: "sum" },
            { field: "USDPesificable", aggregate: "sum" },
            { field: "USDNoPesificable", aggregate: "sum" },
            { field: "USDTotal", aggregate: "sum" }
        ]
    };

    $("#grid").kendoGrid({
        toolbar: kendo.template($("#template").html()),
        excel: {
            fileName: "Reporte Dolarizado.xlsx",
            allPages: true
        },
        dataSource: ds,
        change: onChange,
        footerTemplate: "<b>TOTAL</b>",
        dataBound: function () {
            var grid = $("#grid").data("kendoGrid");
            console.log(grid.dataSource._aggregateResult);
            $("#KilosPesificable").html(kendo.toString((grid.dataSource._aggregateResult.KgVencimientoPesificable != null ?
                grid.dataSource._aggregateResult.KgVencimientoPesificable.sum : 0), 'n0'));
            $("#USDPesificable").html(kendo.toString((grid.dataSource._aggregateResult.USDPesificable != null ?
                grid.dataSource._aggregateResult.USDPesificable.sum: 0), 'n2'));

            $("#KilosNoPesificables").html(kendo.toString((grid.dataSource._aggregateResult.KgNoPesificable != null ?
                grid.dataSource._aggregateResult.KgNoPesificable.sum : 0), 'n0'));
            $("#USDNoPesificable").html(kendo.toString((grid.dataSource._aggregateResult.USDNoPesificable != null ?
                grid.dataSource._aggregateResult.USDNoPesificable.sum : 0), 'n2'));

            $("#KilosTotales").html(kendo.toString((grid.dataSource._aggregateResult.KgTotales != null ?
                grid.dataSource._aggregateResult.KgTotales.sum : 0), 'n0'));
            $("#USDTotales").html(kendo.toString((grid.dataSource._aggregateResult.USDTotal != null ?
                grid.dataSource._aggregateResult.USDTotal.sum : 0), 'n2'));

            grid.tbody.find('tr').each(function myfunction() {
                var row = $(this);
                var dataItem = grid.dataItem(row);

                if (dataItem.Excepcion == true) {
                    row.addClass("k-state-disabled");
                }
            });
        },
        //pageable: true,
        columns: [

            {
                selectable: true, width: "50px", footerTemplate: "<b>TOTAL</b>"
            },
            {
                field: "Excepcion", type: "string", title: "Excepcion", width: 100, editable: function (dataItem) {
                    return false;
                }, template: function (dataItem) {
                    var excedente = dataItem.Excepcion == "Si" || dataItem.Excepcion == true ? "checked" : "";
                    return "<label class='content-input' style='cursor:pointer;'><input "+ (configurarExcedente == true ? "" : "disabled") +" id='Excedente" + dataItem.Id + "'onclick='ConfigurarExcedente(" + dataItem.Id + ")'  type='checkbox'" + excedente + "><i style='color:white'></i></label>";

                }, filterable: true,
            },
            {
                field: "FechaInstruccion", title: "Fecha Instrucción", type: "date", width: 150, format: _DefaultDateTemplate, template: function (dataItem) {
                    return dataItem.FechaInstruccion != null ? kendo.toString(kendo.parseDate(dataItem.FechaInstruccion, 'yyyy-MM-dd'), 'dd/MM/yyyy') : "";
                }
            },
            { field: "Contrato", type: "string", width: 150 },
            { field: "Fijacion", type: "string", width: 150 },
            {
                field: "FechaFijacion", title: "Fecha Fijación", type: "date", width: 150, format: _DefaultDateTemplate, template: function (dataItem) {
                    return dataItem.FechaFijacion != null ? kendo.toString(kendo.parseDate(dataItem.FechaFijacion, 'yyyy-MM-dd'), 'dd/MM/yyyy') : "";
                }
            },
            {
                field: "FechaUltimaAplicacion", title: "Fecha Ultima Aplicacion", type: "date", width: 150, format: _DefaultDateTemplate, template: function (dataItem) {
                    return dataItem.FechaUltimaAplicacion != null ? kendo.toString(kendo.parseDate(dataItem.FechaUltimaAplicacion, 'yyyy-MM-dd'), 'dd/MM/yyyy') : "";
                }
            },
            { field: "Dolarizado", title: "Dolarizado", type: "string", width: 150, template: function (dataItem) { return dataItem.Dolarizado == true ? "Si" : "No"; } },
            { field: "DolarizadoNoProductor", title: "Dolarizado No Productor", type: "string", width: 150, template: function (dataItem) { return dataItem.DolarizadoNoProductor == true ? "Si" : "No"; } },
            { field: "DolarizadoExpress", title: "Dolarizado Express", type: "string", width: 150, template: function (dataItem) { return dataItem.DolarizadoExpress == true ? "Si" : "No"; } },
            {
                field: "FechaHastaDolarizado", title: "Fecha Hasta Dolarizado", type: "date", width: 150, format: _DefaultDateTemplate, template: function (dataItem) {
                    return dataItem.FechaHastaDolarizado != null ? kendo.toString(kendo.parseDate(dataItem.FechaHastaDolarizado, 'yyyy-MM-dd'), 'dd/MM/yyyy') : "";
                }
            },
            { field: "Cantidad", title: "Kilos Negocio", format: "{0:n0}", aggregates: ["sum"], footerTemplate:"#: sum #", width: 150 },
            { field: "CantidadRecibida", title: "Kilos Aplicados", format: "{0:n0}", aggregates: ["sum"], width: 150 },
            { field: "KgVencimientoPesificable", title: "Kilos Pesificable", format: "{0:n0}", aggregates: ["sum"], width: 150 },
            { field: "KgNoPesificable", title: "Kilos No Pesificables", format: "{0:n0}", aggregates: ["sum"], width: 150 },
            { field: "KgTotales", title: "Kilos Totales", format: "{0:n0}", aggregates: ["sum"], width: 150 },
            { field: "USDPesificable", title: "USD Pesificable", format: "{0:n0}", aggregates: ["sum"], width: 150 },
            { field: "USDNoPesificable", title: "USD No Pesificables", format: "{0:n0}", aggregates: ["sum"], width: 150 },
            { field: "USDTotal", title: "USD Totales", format: "{0:n0}", aggregates: ["sum"], },
            { field: "Precio", type: "number", format: "{0:n2}", width: 150 },
            { field: "MonedaId", title: "Moneda", width: 150 },
            { field: "CuitVendedor", title: "CUIT Vendedor", type: "string", width: 150 },
            { field: "NombreVendedor", title: "Vendedor", type: "string", width: 300 },
            { field: "CuitCorredor", title: "CUIT Corredor", type: "string", width: 150 },
            { field: "NombreCorredor", title: "Corredor", type: "string", width: 300 },
            { field: "Clasificacion", type: "string", width: 150 },
            { field: "MaterialDesc", title: "Material Descripción", type: "string", width: 150 },
            { field: "ComercialDesc", title: "Comercial", title: "Comercial", type: "string", width: 150 },
            { field: "Unidad", type: "string", width: 150 },
            { field: "StatusDescripcion", title: "Status", type: "string", width: 150 },
            { field: "CesionDescripcion", title: "Cesion", type: "string", width: 150 },           
            
        ],
        //pageable: {
        //    messages: {
        //        display: "{2} elementos"//,
        //        //empty: "No hay elementos para mostrar",
        //        //page: "P&aacute;gina",
        //        //allPages: "Todas",
        //        //of: "de {0}",
        //        //itemsPerPage: "Elementos por p&aacute;gina",
        //        //first: "Ir a la primer p&aacute;gina",
        //        //previous: "Ir a la p&aacute;gina anterior",
        //        //next: "Ir a la p&aacute;gina siguiente",
        //        //last: "Ir a la &uacute;ltima p&aacute;gina",
        //        //refresh: "Recargar"
        //    },
        //    input: false,
        //    numeric: false
        //},
        scrollable: true,
        height: 550,
        sortable: {
            mode: "multiple",
            allowUnsort: true,
            showIndexes: false
        },
        excelExport: function (e) {
            var sheet = e.workbook.sheets[0];

            for (var i = 1; i < sheet.rows.length; i++) {
                var row = sheet.rows[i];

                //la Fecha en Chrome aparece corrida un dia, solucion:
                if (row.cells[0].value == true) {
                    row.cells[0].value = "SI"
                } else {
                    row.cells[0].value = "NO"
                }
                //if (row.cells[5].value == true) {
                //    row.cells[5].value = "SI"
                //} else {
                //    row.cells[5].value = "NO"
                //}

                if (row.cells[6].value == true) {
                    row.cells[6].value = "SI"
                } else {
                    row.cells[6].value = "NO"
                }

                if (row.cells[7].value == true) {
                    row.cells[7].value = "SI"
                } else {
                    row.cells[7].value = "NO"
                }

                if (row.cells[8].value == true) {
                    row.cells[8].value = "SI"
                } else {
                    row.cells[8].value = "NO"
                }
                var fechaInstruccion = row.cells[1].value;
                var fecha = row.cells[3].value;
                var fechaHasta = row.cells[9].value;
                var fechaFijacion = row.cells[4].value;
                var ultimaAplicacion = row.cells[5].value;
                fechaInstruccion = kendo.parseDate(fechaInstruccion, 'yyyy-MM-dd')
                fechaHasta = kendo.parseDate(fechaHasta, 'yyyy-MM-dd')
                fecha = kendo.parseDate(fecha, 'yyyy-MM-dd')
                fechaFijacion = kendo.parseDate(fechaFijacion, 'yyyy-MM-dd')
                ultimaAplicacion = kendo.parseDate(ultimaAplicacion, 'yyyy-MM-dd')
                if (fechaInstruccion != null) {
                    row.cells[1].value = fechaInstruccion;
                    row.cells[1].format = 'dd/MM/yyyy'
                }
                if (fecha != null) {
                    row.cells[3].value = fecha;
                    row.cells[3].format = 'dd/MM/yyyy'
                }
                if (fechaHasta != null) {
                    row.cells[9].value = fechaHasta;
                    row.cells[9].format = 'dd/MM/yyyy'
                }
                if (fechaFijacion != null) {
                    row.cells[4].value = fechaFijacion;
                    row.cells[4].format = 'dd/MM/yyyy'
                }
                if (ultimaAplicacion != null) {
                    row.cells[5].value = ultimaAplicacion;
                    row.cells[5].format = 'dd/MM/yyyy'
                }

            }
        },
    });


    kendo.culture("es-AR");
    CrearMultiSelectFiltro("#buscadorProveedor", "Proveedor", "CUIT", "/ReportePesificados/ListarProveedor");
    CrearMultiSelectFiltro("#buscadorCorredor", "Proveedor", "CUIT", "/ReportePesificados/ListarCorredor");
    inicializarPopUpSap("Contratos");

    $("#fechaInstruccionId").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        change: function () {
        }
    });
    //$("#FechaInstruccionHastaId").kendoDatePicker({
    //    format: "dd-MM-yyyy",
    //    parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
    //    change: function () {
    //    }
    //});

}

function Filtrar() {
    $('#grid').data('kendoGrid').dataSource.read();

}

$("#DolarizadoId").click(function () {

    if ($("#DolarizadoId").is(':checked')) {
        $("#fechalimiteId").data("kendoDatePicker").enable(false);
        $("#fechalimiteId").data("kendoDatePicker").value("");
    } else {
        $("#fechalimiteId").data("kendoDatePicker").enable();
    }
});


function ObtenerFechaDesde() {
    var hoy = new Date();
    var anio = hoy.getFullYear();
    var mes = hoy.getMonth() + 1;
    var dia = hoy.getDate();
    if (mes < 10) {
        mes = "0" + mes.toString();
    }
    if (dia < 10) {
        dia = "0" + dia.toString();
    }
    return dia + '-' + mes + '-' + anio;
}

function Refrescar() {
    $("#buscar-reporte").click();
}

function setearValoresComboDeInicio() {
    url = $('#descargaReporte').attr('href');
    $('#descargaReporte').attr('href', url + '?fechaString=' + fechaString + '&fechaHastaString=' + fechaString + '&centroId=' + ObtenerValorCentroId() + '&materialId=' + ObtenerValorMaterialId().toString());
}

$("#descargar-reporte").click(function () {
    var url = '/ReporteEvolucionFijacion/DescargarReporte';
    var fecha = $("#fecha").val();
    var fechaHasta = $("#fechaHasta").val();
    var ProveedorId = $("#ProveedorId").val();
    var ComercialId = $("#ComercialId").val();
    var CampanaId = $("#CampanaId").val();
    var MaterialId = $("#MaterialId").val();
    var GrupoCompraId = $("#GrupoCompraId").val();
    var ClasificacionId = $("#Clasificacion").val();
    var DestinoId = $("#DestinoId").val();

    $('#descargarReporte').attr('href', url + '?fecha=' + fecha + '&fechaHasta=' + fechaHasta + '&ProveedorId=' + ProveedorId + '&ComercialId=' + ComercialId
        + '&CampanaId=' + CampanaId + '&MaterialId=' + MaterialId + '&GrupoCompraId=' + GrupoCompraId + '&ClasificacionId=' + ClasificacionId + '&DestinoId=' + DestinoId);
    document.getElementById("descargarReporte").click();
});

function DeshabilitarTildeExcluyenteKgVencimientoPesificable() {

    if ($("#KgVencimientoPesificable").is(":checked")) {
        $("#KgNoPesificable").prop("checked", false)
        $("#KgTotales").prop("checked", false)
    }
}

function DeshabilitarTildeExcluyenteKgNoPesificable() {

    if ($("#KgNoPesificable").is(":checked")) {
        $("#KgVencimientoPesificable").prop("checked", false)
        $("#KgTotales").prop("checked", false)
    }

}
function DeshabilitarTildeExcluyenteKgTotales() {

    if ($("#KgTotales").is(":checked")) {
        $("#KgNoPesificable").prop("checked", false)
        $("#KgVencimientoPesificable").prop("checked", false)
    }

}

function ConfigurarExcedente(idConfiguracion) {
    var id = idConfiguracion;

    var grid = $("#grid").data("kendoGrid").dataSource.data();
    var exce = $("#Excedente" + id).is(":checked") ?? null;
    BlockUi("Guardando...");
    result = MSExecuteOnServer('/NegocioPesificacion/ConfigurarExcedente', { id: id, excedente: exce });
    if (result != null && result.Errores != null && result.Errores.length > 0) {
        MensErr(result.Errores[0].Message);
    }
    $.unblockUI();
    recargarGrilla();
}

function recargarGrilla() {
    $('#grid').data('kendoGrid').dataSource.read();
}

function EnviarMail() {

    BlockUi("Enviando Mail...");
    setTimeout(function () {
        var fecha = $("#fechaInstruccionId").val();
        if (fecha == "") {
            MensErr("Debe ingresar una fecha de instrucción de pesificación");
        } else {
            result = MSExecuteOnServer('/NegocioPesificacion/EnviarMail', { ids: SeleccionarElementos(), fecha: fecha });
            if (result == "Error") {
                MensErr("Debe seleccionar un negocio");
            } else {
                MensInfo("Mail enviado")
                recargarGrilla();
            }
        }
        $.unblockUI();

    }, 500);
 

}

function SeleccionarElementos() {
    var grid = $("#grid").data("kendoGrid");
    var selectedRows = grid.select();
    obj = [];

    selectedRows.each(function (index, row) {
        var selectedItem = grid.dataItem(row).Id;
        obj.push(selectedItem);
    });

    return obj;
}
function onChange(arg) {
    $(".k-state-disabled").removeClass("k-state-selected");
    $(".k-state-disabled").find(".k-checkbox").prop("checked", false);
}
//function DeseleccionarExcedente() {
//    $("[aria-label='Select all rows']").click(function (e) {
//        var grid = $('#grid').data("kendoGrid");
//        var data = grid.dataSource.data();
//        for (var i = 0; i < data.length; i++) {
//            var currentDataItem = data[i];
//            var row = grid.tbody.find("tr[data-uid='" + currentDataItem.uid + "']");
//            if (currentDataItem.name == "Excepcion" && currentDataItem.Excepcion == 'Si') {
//                setTimeout(function () {
//                    $(row).removeClass("k-state-selected");
//                    var currRowCheckbox = $(row).find(".k-checkbox")
//                    $(currRowCheckbox).attr("checked", false);
//                });
//            }
//        }
//    });
//}