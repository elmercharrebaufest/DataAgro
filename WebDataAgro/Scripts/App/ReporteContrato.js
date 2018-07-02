
$(document).ready(function () {
    kendo.culture("es-AR");
    CreateGridInformeCompraNet();
});


function CreateGridInformeCompraNet() {
    var defaultFilter = { field: "Estado_Contrato", operator: "eq", value: "Finalizado" };
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                url: '/Contrato/BuscaDatosTabla',
            },
            parameterMap: function (options, operation) {
                if (options.filter) {
                    KendoGrid_FixFilter(ds, options.filter);
                }
                return options;
            }
        },
        schema: {
            data: 'Data',
            total: 'Total',
            model: {
                id: 'Id',
                fields: {
                    Cuit: { type: "number" },
                    Negocio: {type:"number"},
                    Fecha: { type: "date" },
                    FechaDesde: { type: "date" },
                    FechaHasta: { type: "date" },
                    FechaEntrega: { type: "date" },
                    Fecha_Dolarizado: { type: "date" },
                    Cantidad: { type: "number" },
                    Precio: { type: "number" },
                    Dias_Pesificado: { type: "number" },
                    Pesificado: { type: "boolean" },
                    Sustentable: { type: "boolean" },
                    Dolarizado: { type: "boolean" },
                    NoInformaSIO: { type: "boolean" },
                    TrigoEspecial: {type: "boolean"}
                }
            },
        },
        requestStart: function () {
            kendo.ui.progress($("#loading"), false);
        },
        requestEnd: function () {
            kendo.ui.progress($("#loading"), false);
        },
        
        serverPaging: true,
        serverSorting: true,
        sort: { field: "Fecha", dir: "desc" },
        serverFiltering: true,
        pageSize: 20,
        filter: defaultFilter
    };

    $("#grid").kendoGrid({
        toolbar: ["excel"],
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
  
        },
        columns: [
            {
                field: "Cuit", title: "CUIT", width:90, template: function(dataItem)
                {
                if (dataItem.Estado == 1) {
                    return '<div class="statuspendiente "></div>' + dataItem.Cuit;
                    } else if (dataItem.Estado == 2) {
                    return '<div class="statusconfirmado "></div>' + dataItem.Cuit;
                    } else if (dataItem.Estado == 3) {
                    return '<div class="statusoferta "></div>' + dataItem.Cuit;
                    } else if (dataItem.Estado == 4) {
                    return '<div class="statuserror "></div>' + dataItem.Cuit;
                    } else if (dataItem.Estado == 5) {
                    return '<div class="statusfinalizado "></div>' + dataItem.Cuit;
                    } else if (dataItem.Estado == 6) {
                    return '<div class="statusborrado "></div>' + dataItem.Cuit;
                    }
                }
            },

            {
                field: "Proveedor", type: "string", width: 300, filterable: { ui: createMultiSelectProveedor, extra: false },
            },
            { field: "Negocio", width: 90 },
            { field: "Fecha", title: "Operacion", filterable: { extra: true }, width: 80, format: _DefaultDateTemplate },
            { field: "TipoNegocio", title: "Tipo", filterable: {
                    multi: true, dataSource: [{
                        TipoNegocio: "A FIJAR",
                    }, {
                        TipoNegocio: "A PRECIO",
                    }, {
                        TipoNegocio: "FIJACION",
                    }]
                }, width: 70
            },
            { field: "Material", filterable: {
                    multi: true, dataSource: [{
                        Material: "Maiz Duro Dentado",
                    }, {
                        Material: "Trigo Pan",
                    }, {
                        Material: "Semilla de Soja",
                    }]
                }, width: 130, template: "#=Material#"
            },
            { field: "Cantidad", format: "{0:n0}" },
            { field: "Precio", format: "{0:n2}" },
            { field: "Moneda", filterable: {
                    multi: true, dataSource: [{
                        Moneda: "ARP",
                    }, {
                        Moneda: "USD",
                    }]
                }, title: "Mon"
            },
            { field: "Provincia", filterable: { ui: createMultiSelectProvincia } },
            { field: "Localidad", filterable: { ui: createMultiSelectLocalidad } },
            { field:"Campania", value: "Campania", title: "Camp", filterable: { multi: true } },
            {
                title: "Fecha", columns: [
                    { field: "FechaDesde", type: "date", title: "Desde", format: _DefaultDateTemplate, width: 80 },
                    { field: "FechaHasta", type: "date", title: "Hasta", format: _DefaultDateTemplate, width: 80 },
                ]
            },
            { field: "Comercial", title: "Comercial", filterable: { multi: true, search: true } },
            { field: "Sustentable", columns: [
                    { field: "Sustentable", title: "Sust.", template: function (dataItem) { return dataItem.Sustentable ? "Si" : "No"; } },
                    { field: "Importe_Sustentable", title: "Importe", filterable: false},
                    { field: "Moneda_Sustentable", title: "Moneda", filterable: false}
                    ] },
            { field: "Dolarizado", columns: [
                    { field: "Dolarizado", title: "Dolar.", template: function (dataItem) { return dataItem.Dolarizado ? "Si" : "No"; } },
                { field: "Fecha_Dolarizado", title: "Fecha", filterable: false, width: 80}
                ] },
            { field: "Pesificado", columns: [
                    { field: "Pesificado", title: "Pesif. ", template: function (dataItem) { return dataItem.Pesificado ? "Si" : "No"; } },
                    { field: "Dias_Pesificado", title: "Dias", filterable: false}
                ] },
            { field: "NoInformaSIO", title: "No InformaSIO", headerAttributes: { style: "white-space: normal" }, template: function (dataItem) { return dataItem.NoInformaSIO ? "Si" : "No"; } },
            { field: "TrigoEspecial", title: "Trigo Especial", headerAttributes: { style: "white-space: normal" }, template: function (dataItem) { return dataItem.TrigoEspecial ? "Si" : "No"; } },
            { field: "Estado_Contrato", title: "Estado", width: 90, filterable: {
                    multi: true,
                    dataSource: [{
                        Estado_Contrato: "Pendiente",
                    }, {
                        Estado_Contrato: "Confirmado",
                    }, {
                        Estado_Contrato: "Con Error",
                    }, {
                        Estado_Contrato: "Oferta",
                    }, {
                        Estado_Contrato: "Finalizado",
                    }, {
                        Estado_Contrato: "Rechazado",
                    },]
                } },
            { field: "Observacion", type: "string", filterable: false, attributes: {"class": "ColumnaObservacion"}}
        ],
        excelExport: function (e) {
            var sheet = e.workbook.sheets[0];
            var templateSustentable = kendo.template(this.columns[14].columns[0].template);
            var templateDolarizado = kendo.template(this.columns[15].columns[0].template);
            var templatePesificado = kendo.template(this.columns[16].columns[0].template);
            var templateSIO = kendo.template(this.columns[17].template);
            var templateTrigoEsp = kendo.template(this.columns[18].template);

            for (var i = 2; i < sheet.rows.length; i++) {
                var row = sheet.rows[i];

                var dataItem = {
                    Sustentable: row.cells[15].value,
                    Dolarizado: row.cells[18].value,
                    Pesificado: row.cells[20].value,
                    NoInformaSIO: row.cells[22].value,
                    TrigoEspecial: row.cells[23].value,
                };

                row.cells[15].value = templateSustentable(dataItem);
                row.cells[18].value = templateDolarizado(dataItem);
                row.cells[20].value = templatePesificado(dataItem);
                row.cells[22].value = templateSIO(dataItem);
                row.cells[23].value = templateTrigoEsp(dataItem);
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
        sortable: true,
        selectable: "row",

        filterable: {
            height: 350,
            extra: false,
            checkAll: false,
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
                },
                date: {
                    eq: "Igual",
                    gte: "Despu&eacute;s o igual a",
                    lte: "Antes o igual a",
                },
                number: {
                    eq: "Igual a",
                    gte: "Mayor que o igual a",
                    lte: "Menor que o igual a",
                }
            }
        },
        filterMenuInit: function (e) {
            $(e.container).css("width", "200px")
        }
    });


    function createMultiSelect(element, textField, valueField, url ) {
        element.removeAttr("data-bind");

        element.kendoMultiSelect({
            dataTextField: textField,
            dataValueField: valueField,
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
                var filter = { logic: "or", filters: [] };
                var values = this.value();
                $.each(values, function (i, v) {
                    if (v != '') {
                        filter.filters.push({ field: valueField, operator: "eq", value: v });
                    }
                });
                if (values.length == 0) {
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
        
    };
    //Con definir un método de estos para cada columna multiselect estamos, 
    //function createMultiSelectComercial(element) {
    //    return createMultiSelect(element, "Comercial", "ComercialId", "/Contrato/ListarComercial");
    //};
    function createMultiSelectProvincia(element) {
        return createMultiSelect(element, "Provincia", "ProvinciaId", "/Contrato/ListarProvincia")
    }

    function createMultiSelectLocalidad(element) {
        return createMultiSelect(element, "Localidad", "LocalidadId", "/Contrato/ListarLocalidad")
    }

    function createMultiSelectProveedor(element) {
        return createMultiSelect(element, "Proveedor", "ProveedorId", "/Contrato/ListarProveedor");
    };
}
