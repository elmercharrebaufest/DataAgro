$(document).ready(function () {

    kendo.culture("es-AR");

    inicializarTodosKendoDate($(".filtroFecha"));
    //$("#fechaCargaId").data("kendoDatePicker").value(new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate()));
    CreateGridInformeCompraNet();
    InicializarElementos();


});


function CreateGridInformeCompraNet() {
    kendo.ui.FilterMultiCheck.prototype.options.messages =
        $.extend(true, kendo.ui.FilterMultiCheck.prototype.options.messages, {
            "selectedItemsFormat": ""
        });

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
                url: '/ReporteContratosAFijarPase/BuscaDatosTabla',

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
                    Cuit: { type: "number" },
                    Negocio: { type: "number" },
                    Fecha: { type: "date" },
                    FechaDesde: { type: "date" },
                    FechaHasta: { type: "date" },
                    FechaEntrega: { type: "date" },
                    Cantidad: { type: "number" },
                    PrecioPonderado: { type: "number", format: "n2" },
                    PrecioNetoPonderado: { type: "number", format: "n2" },
                    FechaOperacion: { type: "date" },
                }
            }
        },

        serverPaging: true,
        serverSorting: true,
        serverFiltering: false,
        //sort: [
        //    { field: "Material", dir: "desc" }
        //],

        pageSize: 20,
    };

    $("#grid").kendoGrid({
        //toolbar: kendo.template($("#templateToolbar").html()),
        toolbar: ["excel"],
        excel: {
            fileName: "Reporte Contratos a Fijar Pase.xlsx",
            allPages: true,
        },
        dataSource: ds,
        //dataBound: function () {
        //    $("td:has(div.statuspendiente)").css('border-bottom', '5px solid #ffc100');
        //    $("td:has(div.statusconfirmado)").css('border-bottom', '5px solid #179e2b');
        //    $("td:has(div.statusoferta)").css('border-bottom', '5px solid #00adf5');
        //    $("td:has(div.statuserror)").css('border-bottom', '5px solid #d00707');
        //    $("td:has(div.statusfinalizado)").css('border-bottom', '5px solid #000000');
        //    $("td:has(div.statusborrado)").css('border-bottom', '5px solid #848484');
        //    $("td:has(div.statuspreaprobacion)").css('border-bottom', '5px solid #15deca');
        //    $("td:has(div.statuspreanulado)").css('border-bottom', 'border-grey');
        //    $("td:has(div.statusreconfirmarfinalizado)").css('border-bottom', '5px solid #ac67ca');
        //},
        columns: [
            { field: "Cuit", title: "CUIT", width: 90 },
            { field: "Proveedor", type: "string", width: 300, },
            { field: "Corredor", type: "string", width: 300, },
            { field: "Negocio", width: 90 },
            { field: "Fecha", title: "Carga", width: 80, format: _DefaultDateTemplate },
            { field: "Material", width: 90, template: "#=Material#" },
            { field: "Posicion", width: 90 },
            { field: "Cantidad", format: "{0:n0}", width: 90 },
            { field: "KilosPendiente",title: "Pendientes", format: "{0:n0}", width: 90 },
            { field: "PrecioPonderado",title:"Precio", type: "number", format: "{0:n2}", width: 90 },
            //{ field: "PrecioNetoPonderado", title: "Precio Neto", type: "number", format: "{0:n2}", width: 90 },
            //{ field: "Moneda", title: "Moneda", type: "string", width: 90 },

            //{ field: "ImporteFinanciero", title: "Importe Financiero", type: "number", format: "{0:n2}", width: 90 },
            //{ field: "MonedaFinanciero", title: "Moneda", type: "string", width: 90  },
            //{ field: "ImporteRedespacho", title: "Importe Redespacho", type: "number", format: "{0:n2}", width: 90 },
            //{ field: "MonedaRedespacho", title: "Moneda", type: "string", width: 90 },
            //{ field: "PorcentajeComision", title: "Porcentaje Comision", type: "number", format: "{0:n2}", width: 90  },
            //{ field: "ImporteComision", title: "Importe Comision", type: "number", format: "{0:n2}", width: 90  },
            //{ field: "MonedaComision", title: "Moneda", type: "string", width: 90 },
            //{ field: "ImporteBonificacion", title: "Importe Bonificacion", type: "number", format: "{0:n2}", width: 90  },
            //{ field: "PorcentajeBonificacion", title: "Porcentaje Bonificacion", type: "number", format: "{0:n2}", width: 90  },
            //{ field: "MonedaBonificacion", title: "Moneda", type: "string", width: 90  },

            { field: "Plus", title: "Plus", type: "number", format: "{0:n2}", width: 90 },

            //{ field: "ImporteBasis", title: "Importe Basis", type: "number", format: "{0:n2}", width: 90 },
            //{ field: "MonedaBasis", title: "Moneda", type: "string", width: 90  },
            { field: "FechaHasta", type: "date", title: "Hasta", format: _DefaultDateTemplate, width: 80, width: 90  },
            { field: "FechaOperacion", type: "date", title: "Fecha Operacion", format: _DefaultDateTemplate, width: 80, width: 90  },

        ],
        //excelExport: function (e) {
        //    var sheet = e.workbook.sheets[0];

        //    var templateHora = kendo.template(this.columns[6].template);
        //    var templatePizarra = kendo.template(this.columns[15].template);
        //    var templateSustentable = kendo.template(this.columns[34].columns[0].template);
        //    var templateDolarizado = kendo.template(this.columns[35].columns[0].template);
        //    var templateDolarizadoExpress = kendo.template(this.columns[36].columns[0].template);
        //    var templateDolarizadoCorredor = kendo.template(this.columns[37].columns[0].template);
        //    var templatePesificado = kendo.template(this.columns[38].columns[0].template);
        //    var templateSIO = kendo.template(this.columns[39].template);
        //    var templateTrigoEsp = kendo.template(this.columns[40].template);
        //    var templateCesion = kendo.template(this.columns[58].template);//agregar uno cuando se suba  DescripcionOperacionAnterior

        //    for (var i = 2; i < sheet.rows.length; i++) {
        //        var row = sheet.rows[i];

        //        var dataItem = {
        //            Hora: row.cells[6].value,
        //            Pizarra: row.cells[15].value,
        //            Sustentable: row.cells[35].value,
        //            Dolarizado: row.cells[40].value,
        //            DolarizadoExpress: row.cells[42].value,
        //            DolarizadoCorredor: row.cells[44].value,
        //            Pesificado: row.cells[46].value,
        //            NoInformaSIO: row.cells[48].value,
        //            TrigoEspecial: row.cells[49].value,
        //            Cesion: row.cells[67].value,//agregar uno cuando se suba  DescripcionOperacionAnterior
        //        };

        //        var operacionFecha = row.cells[5].value;
        //        operacionFecha.setHours(operacionFecha.getHours() + 1);
        //        row.cells[5].value = operacionFecha;

        //        var fechaDesde = row.cells[31].value;
        //        var fechaHasta = row.cells[32].value;

        //        if (fechaDesde != null) {
        //            fechaDesde.setHours(fechaDesde.getHours() + 1);
        //            row.cells[31].value = fechaDesde;
        //        }

        //        if (fechaHasta != null) {

        //            fechaHasta.setHours(fechaHasta.getHours() + 1);
        //            row.cells[32].value = fechaHasta;
        //        }


        //        row.cells[6].value = templateHora(dataItem);
        //        row.cells[15].value = templatePizarra(dataItem);
        //        row.cells[35].value = templateSustentable(dataItem);
        //        row.cells[40].value = templateDolarizado(dataItem);
        //        row.cells[41].value = row.cells[40].value == "Si" ? row.cells[41].value : "";
        //        row.cells[42].value = templateDolarizadoExpress(dataItem);
        //        row.cells[43].value = row.cells[42].value == "Si" ? row.cells[43].value : "";
        //        row.cells[44].value = templateDolarizadoCorredor(dataItem);
        //        row.cells[45].value = row.cells[44].value == "Si" ? row.cells[45].value : "";
        //        row.cells[46].value = templatePesificado(dataItem);
        //        row.cells[48].value = templateSIO(dataItem);
        //        row.cells[49].value = templateTrigoEsp(dataItem);
        //        row.cells[67].value = templateCesion(dataItem);//agregar uno cuando se suba  DescripcionOperacionAnterior

        //    }
        //},
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
        height: 550,
        sortable: {
            mode: "multiple",
            allowUnsort: true,
            showIndexes: false
        },
        selectable: "row",

        filterable: false,
        filterMenuInit: function (e) {
            if (e.field == "Proveedor" || e.field == "Comercial" || e.field == "Provincia" || e.field == "Localidad") {
                $(e.container).css("width", "300px");
            } else {
                $(e.container).css("width", "150px");
            }
        }
    });
}

function ObtenerFecha() {
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

function ObtenerFechaMas30() {
    var hoy = new Date();
    hoy.setDate(hoy.getDate() + 30);
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

function InicializarElementos() {
    $(".number").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        value: " ",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0
    });
    kendo.culture("es-AR");

    $("#nuevoNumContrato").kendoNumericTextBox({
        culture: "es-AR",
        format: "######################",
        value: " ",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0
    });

    CrearMultiSelectFiltro("#buscadorProveedor", "Proveedor", "ProveedorId", "/ReporteContratosAFijarPase/ListarProveedor");
    CrearMultiSelectFiltro("#buscadorCorredor", "Corredor", "CorredorId", "/ReporteContratosAFijarPase/ListarCorredor");
    CrearMultiSelectFiltro("#ClasificacionId", "Clasificacion", "ClasificacionId", "/ReporteContratosAFijarPase/ListarClasificacion");


    $("#SustentableId").click(function () {
        if ($("#SustentableId").is(':checked')) {
            $("#Importe_SustentableId").prop('disabled', true);
            $("#Importe_SustentableId").data("kendoNumericTextBox").value("");
        } else {
            $("#Importe_SustentableId").prop('disabled', false);
        }
    });
    $("#PesificadoId").click(function () {
        if ($("#PesificadoId").is(':checked')) {
            $("#Dias_PesificadoId").prop('disabled', true);
            $("#Dias_PesificadoId").data("kendoNumericTextBox").value("");
        } else {
            $("#Dias_PesificadoId").prop('disabled', false);
        }
    });

    inicializarPopUpSap("Contratos");
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

//function filtroContratoSap(nombreDelFiltro) {

//    let filtrosContratosSap = [];
//    $("#contratos-table td").each(function (e) {

//        let valorBuscado = $("#contratos-table td")[e].innerText;

//        (valorBuscado != "" &&
//            $(valorBuscado != null)) ? filtrosContratosSap.push(new FiltroHijo(nombreDelFiltro, valorBuscado, "eq")) : null;
//    });
//    return (filtrosContratosSap.length > 0) ? new FiltroPadre("or", filtrosContratosSap) : null;
//}


function customExport() {
    //TraerFiltrosConValores();
    var funcReturn = function (data) {
        if (data != null) {
            if (data.DownloadKey.length > 0) {
                var url = MSGetUrl('/DownLoad/Excel?key=' + data.DownloadKey);
                window.location = url;
            }
        }
    };
    MSExecuteOnServerAsync('/ReporteContratosAFijarPase/Export', TraerFiltrosConValores(), funcReturn, true);
}

