$(document).ready(function () {
    
    kendo.culture("es-AR");
    
    inicializarTodosKendoDate($(".filtroFecha"));
    $("#fechaCargaId").data("kendoDatePicker").value(new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate()));
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
                url: '/Contrato/BuscaDatosTabla',

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
                    Acuerdo: { type: "number" },
                    Fecha: { type: "date" },
                    FechaCierta: { type: "date" },
                    FechaDesde: { type: "date" },
                    FechaHasta: { type: "date" },
                    FechaEntrega: { type: "date" },
                    Fecha_Dolarizado: { type: "date" },
                    FechaConfirmacion: { type: "date" },
                    UsuarioConfirmador: {type: "string"},
                    Cantidad: { type: "number" },
                    Precio: { type: "number", format: "n2" },
                    Pizarra: { type: "boolean" },
                    Dias_Pesificado: { type: "number" },
                    Pesificado: { type: "boolean" },
                    Sustentable: { type: "boolean" },
                    Dolarizado: { type: "boolean" },
                    NoInformaSIO: { type: "boolean" },
                    TrigoEspecial: { type: "boolean" },
                    DesdeFijacion: { type: "date" },
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
        toolbar: kendo.template($("#templateToolbar").html()),
        //toolbar: ["excel"],
        excel: {
            fileName: "Reporte Contratos.xlsx",
            allPages: true,
        },
        dataSource: ds,
        dataBound: function () {
            $("td:has(div.statuspendiente)").css('border-bottom', '5px solid #ffc100');
            $("td:has(div.statusconfirmado)").css('border-bottom', '5px solid #179e2b');
            $("td:has(div.statusoferta)").css('border-bottom', '5px solid #00adf5');
            $("td:has(div.statuserror)").css('border-bottom', '5px solid #d00707');
            $("td:has(div.statusfinalizado)").css('border-bottom', '5px solid #000000');
            $("td:has(div.statusborrado)").css('border-bottom', '5px solid #848484');
            $("td:has(div.statuspreaprobacion)").css('border-bottom', '5px solid #15deca');
            $("td:has(div.statuspreanulado)").css('border-bottom', 'border-grey');
            $("td:has(div.statusreconfirmarfinalizado)").css('border-bottom', '5px solid #ac67ca');
        },
        columns: [
            {
                field: "Cuit", title: "CUIT", width: 90, template: function (dataItem) {
                    if (dataItem.Estado === 1) {
                        return '<div class="statuspendiente "></div>' + dataItem.Cuit;
                    } else if (dataItem.Estado === 2) {
                        return '<div class="statusconfirmado "></div>' + dataItem.Cuit;
                    } else if (dataItem.Estado === 3) {
                        return '<div class="statusoferta "></div>' + dataItem.Cuit;
                    } else if (dataItem.Estado === 4) {
                        return '<div class="statuserror "></div>' + dataItem.Cuit;
                    } else if (dataItem.Estado === 5) {
                        return '<div class="statusfinalizado "></div>' + dataItem.Cuit;
                    } else if (dataItem.Estado === 6) {
                        return '<div class="statusborrado "></div>' + dataItem.Cuit;
                    } else if (dataItem.Estado == 9) {
                        return '<div class="statuspreaprobacion "></div>' + dataItem.Proveedor;
                    } else if (dataItem.Estado == 10) {
                        return '<div class="statuspreanulado "></div>' + dataItem.Proveedor;
                    } else if (dataItem.Estado == 11) {
                        return '<div class="statusreconfirmarfinalizado "></div>' + dataItem.Proveedor;
                    }
                }
            },

            {
                field: "Proveedor", type: "string", width: 300,
            }, {
                field: "Corredor", type: "string", width: 300,
            },
            {
                field: "Negocio", width: 90
            },
            {
                field: "Acuerdo", width: 90
            },
            { field: "Fecha", title: "Carga", width: 80, format: _DefaultDateTemplate },
            {
                field: "Hora", value: "Hora", title: "Hora", template:
                    function (dataItem) {
                        var numeros = dataItem.Hora.split(":");
                        if (numeros[0] < 10) {
                            numeros[0] = "0" + numeros[0];
                        }
                        if (numeros[1] < 10) {
                            numeros[1] = "0" + numeros[1];
                        }
                        return numeros[0] + ":" + numeros[1];
                    }
            },
            {
                field: "TipoNegocio", title: "Tipo", width: 70
            },
            {
                field: "Material", width: 130, template: "#=Material#"
            },
            {
                field: "Cantidad", format: "{0:n0}"
            },
            {
                field: "Precio", type: "number", format: "{0:n2}"
            },
            {
                field: "Moneda", title: "Moneda"
            },
            { field: "PrecioNeto", title: "Precio Neto", type: "number", format: "{0:n2}" },
            { field: "DestinoDescripcion", title: "Centro" },
            { field: "Pizarra", title: "Pizarra", template: function (dataItem) { return dataItem.Pizarra ? "Si" : "No"; } },
            { field: "ImporteFinanciero", title: "Importe Financiero", type: "number", format: "{0:n2}" },
            { field: "ImporteRedespacho", title: "Importe Redespacho", type: "number", format: "{0:n2}" },
            { field: "PorcentajeComision", title: "Porcentaje Comision", type: "number", format: "{0:n2}" },
            { field: "ImporteComision", title: "Importe Comision", type: "number", format: "{0:n2}" },
            { field: "ImporteBonificacion", title: "Importe Bonificacion", type: "number", format: "{0:n2}" },
            { field: "PorcentajeBonificacion", title: "Porcentaje Bonificacion", type: "number", format: "{0:n2}" },

            { field: "Provincia" },
            { field: "Localidad" },
            { field: "Campania", value: "Campania", title: "Campaña" },
            {
                title: "Fecha", columns: [
                    { field: "FechaDesde", type: "date", title: "Desde", format: _DefaultDateTemplate, width: 80 },
                    { field: "FechaHasta", type: "date", title: "Hasta", format: _DefaultDateTemplate, width: 80 }
                ]
            },

            { field: "Comercial", title: "Comercial" },
            { field: "ComercialCreador", title: "Registro Comercial" },
            {
                field: "Sustentable", columns: [
                    { field: "Sustentable", title: "Sust.", template: function (dataItem) { return dataItem.Sustentable ? "Si" : "No"; } },
                    { field: "Importe_Sustentable", title: "Importe", filterable: false },
                    { field: "Moneda_Sustentable", title: "Moneda", filterable: false }
                ]
            },
            {
                field: "Dolarizado", columns: [
                    { field: "Dolarizado", title: "Dolar.", template: function (dataItem) { return dataItem.Dolarizado ? "Si" : "No"; } },
                    {
                        field: "Fecha_Dolarizado", title: "Fecha", filterable: false, width: 80, format: _DefaultDateTemplate, template: function (dataItem) {
                            return dataItem.Dolarizado ? kendo.toString(kendo.parseDate(dataItem.Fecha_Dolarizado, 'yyyy-MM-dd'), 'dd/MM/yyyy')  : "";
                             
                        }
                    }
                ]
            },
            {
                field: "Pago Diferido", columns: [
                    { field: "Pesificado", title: "Pago Dif.", template: function (dataItem) { return dataItem.Pesificado ? "Si" : "No"; } },
                    { field: "Dias_Pesificado", title: "Dias", filterable: false }
                ]
            },
            { field: "NoInformaSIO", title: "No informa SIO", headerAttributes: { style: "white-space: normal" }, template: function (dataItem) { return dataItem.NoInformaSIO ? "Si" : "No"; } },
            { field: "TrigoEspecial", title: "Trigo Especial", headerAttributes: { style: "white-space: normal" }, template: function (dataItem) { return dataItem.TrigoEspecial ? "Si" : "No"; } },
            {
                field: "Estado_Contrato", title: "Estado", width: 90, sortable: false
            },
            { field: "Observacion", type: "string", filterable: false, attributes: { "class": "ColumnaObservacion" } },
            { field: "FechaCierta", type: "date", title: "Fecha Cierta", format: _DefaultDateTemplate, width: 80 },
            { field: "Rechazo", type: "string", title: "Motivo Rechazo" },
            { field: "ClasificacionDescripcion", type: "string", title: "Clasificación" },
            { field: "FechaOperacion", type: "date", title: "Fecha Operacion", format: _DefaultDateTemplate, width: 80 },
            { field: "MotivoOperacionAnterior", type: "string", title: "Motivo Operación Anterior" },
            
            { field: "UsuarioConfirmador", type: "string", title: "Usuario Confirmador" },
            {
                field: "FechaConfirmacion", type: "date", title: "Fecha Confirmación", format: _DefaultDateTemplate,

                template: function (dataItem) {
                    if (dataItem.FechaConfirmacion != null) {
                        return '<div class="statusexterno "></div>' + kendo.toString(kendo.parseDate(dataItem.FechaConfirmacion, 'yyyy-MM-dd'), 'dd/MM/yyyy HH:mm');
                    } else return "";
                }
            },
            {
                field: "Dolarizado Express", columns: [
                    { field: "DolarizadoExpress", title: "Dolar. Express", template: function (dataItem) { return dataItem.DolarizadoExpress ? "Si" : "No"; } },
                    {
                        field: "Fecha_Dolarizado", title: "Fecha", format: _DefaultDateTemplate, filterable: false, width: 80, template: function (dataItem) {
                            return dataItem.DolarizadoExpress ? kendo.toString(kendo.parseDate(dataItem.Fecha_Dolarizado, 'yyyy-MM-dd'), 'dd/MM/yyyy') : "";

                        }
                    }
                ]
            },
            {
                field: "Dolarizado Corredor", columns: [
                    { field: "DolarizadoCorredor", title: "Dolar. Corredor", template: function (dataItem) { return dataItem.DolarizadoCorredor ? "Si" : "No"; } },
                    {
                        field: "Fecha_Dolarizado", title: "Fecha", format: _DefaultDateTemplate, filterable: false, width: 80, template: function (dataItem) {
                            return dataItem.DolarizadoCorredor ? kendo.toString(kendo.parseDate(dataItem.Fecha_Dolarizado, 'yyyy-MM-dd'), 'dd/MM/yyyy') : "";

                        }
                    }
                ]
            },
            { field: "ChequeElectronicoValor", type: "string", title: "Cheque Electrónico" },
            { field: "PagoCBU", type: "string", title: "Pago Cbu" },
        ],
        excelExport: function (e) {
            var sheet = e.workbook.sheets[0];            
            
            var templateHora = kendo.template(this.columns[6].template);
            var templatePizarra = kendo.template(this.columns[14].template);
            var templateSustentable = kendo.template(this.columns[27].columns[0].template);
            var templateDolarizado = kendo.template(this.columns[28].columns[0].template);
            var templatePesificado = kendo.template(this.columns[29].columns[0].template);
            var templateSIO = kendo.template(this.columns[30].template);
            var templateTrigoEsp = kendo.template(this.columns[31].template);
            var templateDolarizadoExpress = kendo.template(this.columns[41].columns[0].template);
            var templateDolarizadoCorredor = kendo.template(this.columns[42].columns[0].template);

            for (var i = 2; i < sheet.rows.length; i++) {
                var row = sheet.rows[i];

                var dataItem = {
                    Hora: row.cells[6].value,
                    Pizarra: row.cells[14].value,
                    Sustentable: row.cells[28].value,
                    Dolarizado: row.cells[31].value,
                    Pesificado: row.cells[33].value,
                    NoInformaSIO: row.cells[35].value,
                    TrigoEspecial: row.cells[36].value,
                    DolarizadoExpress: row.cells[46].value,
                    DolarizadoCorredor: row.cells[48].value
                };

                var operacionFecha = row.cells[5].value;
                operacionFecha.setHours(operacionFecha.getHours() + 1);
                row.cells[5].value = operacionFecha;

                var fechaDesde = row.cells[24].value;
                var fechaHasta = row.cells[25].value;

                if (fechaDesde != null) {
                    fechaDesde.setHours(fechaDesde.getHours() + 1);
                    row.cells[24].value = fechaDesde;
                }

                if (fechaHasta != null) {

                    fechaHasta.setHours(fechaHasta.getHours() + 1);
                    row.cells[25].value = fechaHasta;
                }


                row.cells[6].value = templateHora(dataItem);
                row.cells[14].value = templatePizarra(dataItem);
                row.cells[28].value = templateSustentable(dataItem);
                row.cells[31].value = templateDolarizado(dataItem);
                row.cells[33].value = templatePesificado(dataItem);
                row.cells[35].value = templateSIO(dataItem);
                row.cells[36].value = templateTrigoEsp(dataItem);
                row.cells[46].value = templateDolarizadoExpress(dataItem);
                row.cells[48].value = templateDolarizadoCorredor(dataItem);
                row.cells[45].format = "yy/MM/dd hh:mm:ss";
                row.cells[31].value = templateDolarizado(dataItem);
            }
        },
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


    //Con definir un método de estos para cada columna multiselect estamos,
    //function createMultiSelectComercial(element) {
    //    return createMultiSelect(element, "Comercial", "ComercialId", "/Contrato/ListarComercial");
    //}

    //function createMultiSelectProvincia(element) {
    //    return createMultiSelect(element, "Provincia", "ProvinciaId", "/Contrato/ListarProvincia");
    //}

    //function createMultiSelectLocalidad(element) {
    //    return createMultiSelect(element, "Localidad", "LocalidadId", "/Contrato/ListarLocalidad");
    //}

    //function createMultiSelectProveedor(element) {
    //    return createMultiSelect(element, "Proveedor", "ProveedorId", "/Contrato/ListarProveedor");
    //}
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
   
    CrearMultiSelectFiltro("#buscadorProveedor", "Proveedor", "ProveedorId", "/Contrato/ListarProveedor");
    CrearMultiSelectFiltro("#buscadorCorredor", "Corredor", "CorredorId", "/Contrato/ListarCorredor");
    CrearMultiSelectFiltro("#ClasificacionId", "Clasificacion", "ClasificacionId", "/Contrato/ListarClasificacion");


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
    MSExecuteOnServerAsync('/Contrato/Export', TraerFiltrosConValores(), funcReturn, true);
}

