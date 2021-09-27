var fechaString;
var url;
var viewModel;
var datosIniCrearContrato;

$(document).ready(function () {
    kendo.culture("es-AR");
    inicializarTodosKendoDate($(".filtroFecha"));
    $('#menuproveedor').hide();
    fechaString = ObtenerFechaDesde();
    Inicializar();
    setInterval(Refrescar, 300000);


});

function Inicializar() {
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
                url: '/ReportePesificados/BuscaDatosTabla',

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
                    //Cuit: { type: "number" },
                    //Negocio: { type: "number" },
                    //Acuerdo: { type: "number" },
                    //Fecha: { type: "date" },
                    //FechaCierta: { type: "date" },
                    //FechaDesde: { type: "date" },
                    //FechaHasta: { type: "date" },
                    //FechaEntrega: { type: "date" },
                    //Fecha_Dolarizado: { type: "date" },
                    //FechaConfirmacion: { type: "date" },
                    //UsuarioConfirmador: { type: "string" },
                    //Cantidad: { type: "number" },
                    //Precio: { type: "number", format: "n2" },
                    //Pizarra: { type: "boolean" },
                    //Dias_Pesificado: { type: "number" },
                    //Pesificado: { type: "boolean" },
                    //Sustentable: { type: "boolean" },
                    //Dolarizado: { type: "boolean" },
                    //NoInformaSIO: { type: "boolean" },
                    //TrigoEspecial: { type: "boolean" },
                    //DesdeFijacion: { type: "date" },
                    //FechaOperacion: { type: "date" },
                }
            }
        },

        serverPaging: false,
        serverSorting: false,
        serverFiltering: true,
        //sort: [
        //    { field: "Material", dir: "desc" }
        //],
        pageSize: 20,
        aggregate: [
            { field: "KgVencimientoPesificable", aggregate: "sum" },
            { field: "KgNoPesificable", aggregate: "sum" },
            { field: "KgTotales", aggregate: "sum" },
            { field: "USDPesificable", aggregate: "sum" },
            { field: "USDNoPesificable", aggregate: "sum" },
            { field: "USDTotal", aggregate: "sum" }
        ]
    };

    $("#grid").kendoGrid({
        toolbar: ["excel"],
        excel: {
            fileName: "Reporte Dolarizado.xlsx",
            allPages: true
        },
        dataSource: ds,
        dataBound: function () {
            var grid = $("#grid").data("kendoGrid");
            console.log(grid.dataSource._aggregateResult);
            $("#KilosPesificable").html(kendo.toString(grid.dataSource._aggregateResult.KgVencimientoPesificable.sum, 'n0'));
            $("#USDPesificable").html(kendo.toString(grid.dataSource._aggregateResult.USDPesificable.sum, 'n2'));

            $("#KilosNoPesificables").html(kendo.toString(grid.dataSource._aggregateResult.KgNoPesificable.sum, 'n0'));
            $("#USDNoPesificable").html(kendo.toString(grid.dataSource._aggregateResult.USDNoPesificable.sum, 'n2'));

            $("#KilosTotales").html(kendo.toString(grid.dataSource._aggregateResult.KgTotales.sum, 'n0'));
            $("#USDTotales").html(kendo.toString(grid.dataSource._aggregateResult.USDTotal.sum, 'n2'));
        },
        columns: [
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
            { field: "KgVencimientoPesificable", title: "Kilos Pesificable", format: "{0:n0}", aggregates: ["sum"], width: 150},
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
            //{
            //    field: "CantidadPendiente", format: "{0:n0}"
            //},
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
        scrollable: true,
        height:550,
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

                if (row.cells[4].value == true) {
                    row.cells[4].value = "SI"
                } else {
                    row.cells[4].value = "NO"
                }

                if (row.cells[5].value == true) {
                    row.cells[5].value = "SI"
                } else {
                    row.cells[5].value = "NO"
                }

                if (row.cells[6].value == true) {
                    row.cells[6].value = "SI"
                } else {
                    row.cells[6].value = "NO"
                }

                var fecha = row.cells[2].value;
                var fechaHasta = row.cells[7].value;
                var ultimaAplicacion = row.cells[3].value;
                fechaHasta = kendo.parseDate(fechaHasta, 'yyyy-MM-dd')
                fecha = kendo.parseDate(fecha, 'yyyy-MM-dd')
                ultimaAplicacion = kendo.parseDate(ultimaAplicacion, 'yyyy-MM-dd')
                if (fecha != null) {
                    //fecha = fecha.setHours(fecha.getHours() + 1);
                    row.cells[2].value = fecha;
                    row.cells[2].format = 'dd/MM/yyyy'
                }
                if (fechaHasta != null) {
                    //fecha = fecha.setHours(fecha.getHours() + 1);
                    row.cells[7].value = fechaHasta;
                    row.cells[7].format = 'dd/MM/yyyy'
                }
                if (ultimaAplicacion != null) {
                    //fecha = fecha.setHours(fecha.getHours() + 1);
                    row.cells[3].value = ultimaAplicacion;
                    row.cells[3].format = 'dd/MM/yyyy'
                }

            }
            //aggregates
            //sheet.rows[sheet.rows.length -1].cells[4].value = "";
            //sheet.rows[sheet.rows.length -1].cells[5].value = "";
            //sheet.rows[sheet.rows.length -1].cells[6].value = "";
        },
    });


    kendo.culture("es-AR");
    //$("#KgNoPesificable").kendoNumericTextBox({
    //    culture: "es-AR",
    //    format: "n0",
    //    value: " ",
    //    decimals: 0,
    //    restrictDecimals: true,
    //    spinners: false,
    //    min: 0,
    //});


    //$("#KgVencimientoPesificable").kendoNumericTextBox({
    //    culture: "es-AR",
    //    format: "######################",
    //    value: " ",
    //    decimals: 0,
    //    restrictDecimals: true,
    //    spinners: false,
    //    min: 0,
    //});
    //$("#KgTotales").kendoNumericTextBox({
    //    culture: "es-AR",
    //    format: "######################",
    //    value: " ",
    //    decimals: 0,
    //    restrictDecimals: true,
    //    spinners: false,
    //    min: 0,
    //});

    CrearMultiSelectFiltro("#buscadorProveedor", "Proveedor", "CUIT", "/ReportePesificados/ListarProveedor");
    CrearMultiSelectFiltro("#buscadorCorredor", "Proveedor", "CUIT", "/ReportePesificados/ListarCorredor");


    //$("#SustentableId").click(function () {
    //    if ($("#SustentableId").is(':checked')) {
    //        $("#Importe_SustentableId").prop('disabled', true);
    //        $("#Importe_SustentableId").data("kendoNumericTextBox").value("");
    //    } else {
    //        $("#Importe_SustentableId").prop('disabled', false);
    //    }
    //});
    //$("#PesificadoId").click(function () {
    //    if ($("#PesificadoId").is(':checked')) {
    //        $("#Dias_PesificadoId").prop('disabled', true);
    //        $("#Dias_PesificadoId").data("kendoNumericTextBox").value("");
    //    } else {
    //        $("#Dias_PesificadoId").prop('disabled', false);
    //    }
    //});

    inicializarPopUpSap("Contratos");
}

function Filtrar() {
    $('#grid').data('kendoGrid').dataSource.page(1)
    //$('#grid').data('kendoGrid').dataSource.read();

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

//function ObtenerValorCentroId() {
//    return $("#centroId").val() ? $("#centroId").val() : "0";
//}
//function ObtenerValorMaterialId() {
//    //console.log($("#materialId").data("kendoMultiSelect").value());
//    //return $("#materialId").data("kendoMultiSelect").value();
//    return $("#materialId").val();
//}

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
    var ClasificacionId = $("#ClasificacionId").val();
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

