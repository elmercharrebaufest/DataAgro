$(document).ready(function () {
    kendo.culture("es-AR");
    InicializarDate();
    InicializarElementos();
    CreateGridInformeCompraNet();
        
});

function CreateGridInformeCompraNet() {
    kendo.ui.FilterMultiCheck.prototype.options.messages =
        $.extend(true, kendo.ui.FilterMultiCheck.prototype.options.messages, {
            "selectedItemsFormat": ""
        });
   
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                url: '/Contrato/BuscaDatosTabla',
                data: function () {
                    obj = {};
                    obj.ContratoSAP = $("#ContratoSAP").val();
                    obj.TipoNegocioId = $("#tipoNegocioId").val();
                    obj.FechaEntregaDesde = $("#fechaDesdeTopeId").val();
                    obj.FechaEntregaHasta = $("#fechaHastaTopeId").val();
                    obj.FechaCarga = $("#fechaCargaId").val();
                    obj.FechaHastaFijacion = $("#fechaHastaFijacionId").val();
                    obj.CampaniaId = $("#campaniaId").val();
                    obj.MaterialId = $("#materialId").val();
                    obj.EstadoId = $("#estadoId").val();
                    obj.CentroId = $("#centroId").val();                    
                    obj.BoletoCompraNetId = $("#BoletoId").val();
                    obj.GrupoDeCompraId = $("#grupoDeCompraId").val();
                    obj.ImporteSustentable = $("#ImporteSustentable").val();
                    obj.DiasDiferimiento = $("#DiasDiferimiento").val();
                    obj.FechaLimiteDolarizado = $("#fechalimiteId").val();

                    obj.Importe = $("#importeSustentableId").is(':checked');
                   
                    var lista = [];
                        lista = $("#buscadorProveedor").data("kendoMultiSelect").value();
                    obj.ProveedorId = lista.filter(function (value, index, arr) {
                        return value != "";
                    });
                    lista = $("#buscadorCorredor").data("kendoMultiSelect").value();
                    obj.CorredorId = lista.filter(function (value, index, arr) {
                        return value != "";
                    });
                    return obj;
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
                    Fecha_Dolarizado: { type: "date" },
                    Cantidad: { type: "number" },
                    Precio: { type: "number", format: "n2" },
                    Dias_Pesificado: { type: "number" },
                    Pesificado: { type: "boolean" },
                    Sustentable: { type: "boolean" },
                    Dolarizado: { type: "boolean" },
                    NoInformaSIO: { type: "boolean" },
                    TrigoEspecial: { type: "boolean" },
                    DesdeFijacion: { type: "date" },
                }
            }
        },

        serverPaging: true,
        serverSorting: true,
        sort: [{ field: "Fecha", dir: "desc" }],
        serverFiltering: false,
        pageSize: 20,
    };
   
    $("#grid").kendoGrid({
        toolbar: ["excel"],
        excel: {
            fileName: "Reporte Contratos.xlsx",
            allPages: true
        },
        dataSource: ds,
        dataBound: function () {
            $("td:has(div.statuspendiente)").css('border-bottom', '5px solid #ffc100');
            $("td:has(div.statusconfirmado)").css('border-bottom', '5px solid #179e2b');
            $("td:has(div.statusoferta)").css('border-bottom', '5px solid #00adf5');
            $("td:has(div.statuserror)").css('border-bottom', '5px solid #d00707');
            $("td:has(div.statusfinalizado)").css('border-bottom', '5px solid #000000');
            $("td:has(div.statusborrado)").css('border-bottom', '5px solid #848484');
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
            { field: "Fecha", title: "Operacion", width: 80, format: _DefaultDateTemplate },
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
            { field: "ImporteFinanciero", title: "Importe Financiero", type: "number", format: "{0:n2}" },
            { field: "ImporteRedespacho", title: "Importe Redespacho", type: "number", format: "{0:n2}" },
            { field: "PorcentajeComision", title: "Porcentaje Comision", type: "number", format: "{0:n2}" },
            { field: "ImporteComision", title: "Importe Comision", type: "number", format: "{0:n2}" },
            { field: "ImporteBonificacion", title: "Importe Bonificacion", type: "number", format: "{0:n2}" },
            { field: "PorcentajeBonificacion", title: "Porcentaje Bonificacion", type: "number", format: "{0:n2}" },

            { field: "Provincia"},
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
                    { field: "Fecha_Dolarizado", title: "Fecha", format: _DefaultDateTemplate, filterable: false, width: 80 }
                ]
            },
            {
                field: "Pesificado", columns: [
                    { field: "Pesificado", title: "Pesif. ", template: function (dataItem) { return dataItem.Pesificado ? "Si" : "No"; } },
                    { field: "Dias_Pesificado", title: "Dias", filterable: false }
                ]
            },
            { field: "NoInformaSIO", title: "No informa SIO", headerAttributes: { style: "white-space: normal" }, template: function (dataItem) { return dataItem.NoInformaSIO ? "Si" : "No"; } },
            { field: "TrigoEspecial", title: "Trigo Especial", headerAttributes: { style: "white-space: normal" }, template: function (dataItem) { return dataItem.TrigoEspecial ? "Si" : "No"; } },
            {
                field: "Estado_Contrato", title: "Estado", width: 90, sortable: false
            },
            { field: "Observacion", type: "string", filterable: false, attributes: { "class": "ColumnaObservacion" } }
        ],
        excelExport: function (e) {
            var sheet = e.workbook.sheets[0];
            var templateHora = kendo.template(this.columns[5].template);
            var templateSustentable = kendo.template(this.columns[24].columns[0].template);
            var templateDolarizado = kendo.template(this.columns[25].columns[0].template);
            var templatePesificado = kendo.template(this.columns[26].columns[0].template);
            var templateSIO = kendo.template(this.columns[27].template);
            var templateTrigoEsp = kendo.template(this.columns[28].template);

            for (var i = 2; i < sheet.rows.length; i++) {
                var row = sheet.rows[i];

                var dataItem = {
                    Hora: row.cells[5].value,
                    Sustentable: row.cells[25].value,
                    Dolarizado: row.cells[28].value,
                    Pesificado: row.cells[30].value,
                    NoInformaSIO: row.cells[32].value,
                    TrigoEspecial: row.cells[33].value,
                };

                var operacionFecha = row.cells[4].value;
                operacionFecha.setHours(operacionFecha.getHours() + 1);
                row.cells[4].value = operacionFecha;

                var fechaHasta = row.cells[21].value;
                var fechaDesde = row.cells[22].value;

                if (fechaHasta != null) {

                    fechaHasta.setHours(fechaHasta.getHours() + 1);
                    row.cells[21].value = fechaHasta;
                }
                if (fechaDesde != null) {
                    fechaDesde.setHours(fechaDesde.getHours() + 1);
                    row.cells[22].value = fechaDesde;
                }

                row.cells[5].value = templateHora(dataItem);
                row.cells[25].value = templateSustentable(dataItem);
                row.cells[28].value = templateDolarizado(dataItem);
                row.cells[30].value = templatePesificado(dataItem);
                row.cells[32].value = templateSIO(dataItem);
                row.cells[33].value = templateTrigoEsp(dataItem);

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

    
    function createMultiSelect(element, textField, valueField, url) {
        element.removeAttr("data-bind");

        element.kendoMultiSelect({
            itemTemplate: "<input type='checkbox'/> #:data." + textField + "#",
            dataBound: function () {
                var items = this.ul.find("li");
                setTimeout(function () {
                    checkInputs(items);
                });
            },

            dataTextField: textField,
            dataValueField: valueField,
            autoClose: false,
            autoBind: false,
            delay: 300,
            dataSource: {
                serverFiltering: true,
                filter: [],
                transport: {
                    read: {
                        url: url,
                        data: function () {
                            return {
                                text: element.data("kendoMultiSelect").input.val()
                            };
                        },
                        prefix: ""
                    }
                },
            },
            change: function (e) {
                var items = this.ul.find("li");
                checkInputs(items);

                var filter = { logic: "or", filters: [] };
                var values = this.value();
                $.each(values, function (i, v) {
                    if (v !== '') {
                        filter.filters.push({ field: valueField, operator: "eq", value: v });
                    }
                });
                if (values.length === 0) {
                    $("#grid").data("kendoGrid").dataSource.filter(defaultFilter);
                } else {
                    $("#grid").data("kendoGrid").dataSource.filter(filter);
                }
            }
        });
        setTimeout(function () {
            $(".k-multiselect").parent().children(".k-dropdown").remove();
            $(".k-multiselect").parent().children("div").find('button').remove();
        }, 200);
    }

    //Con definir un método de estos para cada columna multiselect estamos,
    function createMultiSelectComercial(element) {
        return createMultiSelect(element, "Comercial", "ComercialId", "/Contrato/ListarComercial");
    }

    function createMultiSelectProvincia(element) {
        return createMultiSelect(element, "Provincia", "ProvinciaId", "/Contrato/ListarProvincia");
    }

    function createMultiSelectLocalidad(element) {
        return createMultiSelect(element, "Localidad", "LocalidadId", "/Contrato/ListarLocalidad");
    }

    function createMultiSelectProveedor(element) {
        return createMultiSelect(element, "Proveedor", "ProveedorId", "/Contrato/ListarProveedor");
    }
}


function InicializarDate() {
    kendo.culture("es-AR");   
    var dateCarga = ObtenerFecha();
    var dateEntregaDesde = ObtenerFecha();
    var dateEntregaHasta = ObtenerFechaMas30();
    $("#fechaDesdeTopeId").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd-MM-yyyy"]
    });
    fechaStringDesde = $("#fechaDesdeTopeId").val();

    $("#fechaHastaTopeId").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd-MM-yyyy"]
        
    });
    fechaStringDesde = $("#fechaHastaTopeId").val();

    $("#fechaCargaId").kendoDatePicker({
        value: dateCarga,
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd-MM-yyyy"]
    });
    fechaStringDesde = $("#fechaCargaId").val();

    $("#fechaHastaFijacionId").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd-MM-yyyy"]
    });
    fechaStringDesde = $("#fechaHastaFijacionId").val();

    $("#fechalimiteId").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd-MM-yyyy"]
    });
    fechaStringDesde = $("#fechalimiteId").val();
    
    $("#fechaDesdeTopeId").click(function () {
        $("#fechaDesdeTopeId").val("");
    });
    $("#fechaHastaTopeId").click(function () {
        $("#fechaHastaTopeId").val("");
    });
    $("#fechaCargaId").click(function () {
        $("#fechaCargaId").val(dateCarga);
    });
    $("#fechaHastaFijacionId").click(function () {
        $("#fechaHastaFijacionId").val("");
    });
    $("#fechalimiteId").click(function () {
        $("#fechalimiteId").val("");
    });

    $("#diaDiferimientoId").click(function () {
        $("#diaDiferimientoId").val("");
    });

    $("#ContratoSAP").click(function () {
        $("#ContratoSAP").val("");
    });

    $("#ImporteSustentable").click(function () {
        $("#ImporteSustentable").val("");
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

function mostrarocultar(element) {
    if ($(element).text() == "Mostrar") {
        $(element).text("Ocultar");
    } else {
        $(element).text("Mostrar");
    }
}
function CrearMultiSelectFiltro(element, text, value, url){

    $(element).kendoMultiSelect({
        itemTemplate: " #:data." + text + " #",
        dataBound: function () {
            var items = this.ul.find("li");
            setTimeout(function () {
                checkInputs(items);
            }, 500);
        },

        dataTextField: text,
        dataValueField: value,
        autoClose: false,
        autoBind: false,
        dataSource: {
            serverFiltering: true,
            filter: [],
            transport: {
                read: {
                    url: url,
                    data: function () {
                        return {
                            text: $( element ).data("kendoMultiSelect").input.val()
                        };
                    },
                    prefix: ""
                }
            }
        }
    });
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
    
    CrearMultiSelectFiltro("#buscadorProveedor", "Proveedor", "ProveedorId", "/Contrato/ListarProveedor");
    CrearMultiSelectFiltro("#buscadorCorredor", "Corredor", "CorredorId", "/Contrato/ListarCorredor");
    $("#importeSustentableId").click(function () {
        if ($("#importeSustentableId").is(':checked')) {
            $("#ImporteSustentable").prop('disabled', true);
            $("#ImporteSustentable").data("kendoNumericTextBox").value("");
        } else {
            $("#ImporteSustentable").prop('disabled', false);
        }
    });
   
}

function Filtrar() {
    $('#grid').data('kendoGrid').dataSource.read();
}