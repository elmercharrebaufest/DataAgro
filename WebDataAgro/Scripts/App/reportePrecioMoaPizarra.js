var _DefaultDateTemplate = "{0:dd/MM/yyyy hh:mm:ss tt}";
$(document).ready(function () {
    kendo.culture("es-AR");
    $('#menuproveedor').hide();
    CargarGrillaConfig();
});

function CargarGrillaConfig() {
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                contentType: "application/json",
                url: '/reportePrecioMoaPizarra/BuscaDatosTabla'
            },
            parameterMap: function (options, operation) {
                if (operation == "read") {
                    return JSON.stringify(options)
                }
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
                    Id: { type: "number" },      
                }
            }
        },
        serverPaging: true,
        serverSorting: true,
        serverFiltering: true,
        pageSize: 20
    };

    $("#gridPrecioMoaPizarra").kendoGrid({
        toolbar: kendo.template($("#templateToolbar").html()),
        //toolbar: ["excel"],
        excel: {
            fileName: "Reporte Precios Moa y Pizarra.xlsx",
            allPages: true,
        },

        excelExport: function (e) {
            var sheet = e.workbook.sheets[0];

            for (var i = 1; i < sheet.rows.length; i++) {
                var row = sheet.rows[i];
                row.cells[5].value = kendo.toString(kendo.parseDate(row.cells[5].value, 'yyyy-MM-dd  HH:mm:ss'), 'dd/MM/yyyy HH:mm:ss');
                row.cells[6].value = kendo.toString(kendo.parseDate(row.cells[6].value, 'yyyy-MM-dd  HH:mm:ss'), 'dd/MM/yyyy  HH:mm:ss');
                row.cells[7].value = kendo.toString(kendo.parseDate(row.cells[7].value, 'yyyy-MM-dd  HH:mm:ss'), 'dd/MM/yyyy  HH:mm:ss');
                row.cells[8].value = kendo.toString(kendo.parseDate(row.cells[8].value, 'yyyy-MM-dd  HH:mm:ss'), 'dd/MM/yyyy  HH:mm:ss');
                row.cells[9].value = kendo.toString(kendo.parseDate(row.cells[9].value, 'yyyy-MM-dd  HH:mm:ss'), 'dd/MM/yyyy  HH:mm:ss');
                row.cells[10].value = kendo.toString(kendo.parseDate(row.cells[10].value, 'yyyy-MM-dd HH:mm:ss'), 'dd/MM/yyyy  HH:mm:ss');
                row.cells[12].value = kendo.toString(kendo.parseDate(row.cells[12].value, 'yyyy-MM-dd  HH:mm:ss'), 'dd/MM/yyyy  HH:mm:ss');
            }
        },
        dataSource: ds,
        sortable: true,
        columns: [
            { field: "TipoConfiguracion", title: "Tipo Configuración", type: "string" },
            {                
                field: "TipoNegocio", type: "string", filterable: {
                    multi: true, dataSource: [{
                        TipoNegocio: "FIJACIÓN"
                    }, {
                        TipoNegocio: "A PRECIO"
                    }, {
                        TipoNegocio: "A FIJAR"
                    }]
                }, title: "Tipo Negocio", width: 70, attributes: {
                    "class": "mobile-sm"
                }
            }, 
            {
                field: "Precio", title: "Precio", type: "number", template: function (dataItem) {
                    return kendo.toString(dataItem.Precio, "##,#.##").replace(/,/g, ".");
                }
            },
            { field: "MonedaId", title: "Moneda", type: "string" },
            {
                field: "Material", title: "Material", filterable: {
                    multi: true, dataSource: [{
                        Material: "Maiz"
                    }, {
                        Material: "Trigo"
                    }, {
                        Material: "Soja"
                    }, {
                        Material: "Girasol"
                    }, {
                        Material: "Girasol AO"
                    }]
                }, width: 130, template: "#=Material#"
            },
            {
                field: "DesdeVigencia", title: "Desde Vigencia", type: "date", format: _DefaultDateTemplate,
                template: function (dataItem) {
                    if (dataItem.DesdeVigencia != null) {
                        return '<div></div>' + kendo.toString(kendo.parseDate(dataItem.DesdeVigencia, 'yyyy-MM-dd'), 'dd/MM/yyyy HH:mm');
                    } else return "";
                }
            },

            {
                field: "HastaVigencia", title: "Hasta Vigencia", type: "date", format: _DefaultDateTemplate,
                template: function (dataItem) {
                    if (dataItem.HastaVigencia != null) {
                        return '<div></div>' + kendo.toString(kendo.parseDate(dataItem.HastaVigencia, 'yyyy-MM-dd'), 'dd/MM/yyyy HH:mm');
                    } else return "";
                }
            },
            {
                field: "DesdeEntrega", title: "Desde Entrega", type: "date", format: _DefaultDateTemplate,
                template: function (dataItem) {
                    if (dataItem.FechaCreacion != null) {
                        return '<div></div>' + kendo.toString(kendo.parseDate(dataItem.DesdeEntrega, 'yyyy-MM-dd'), 'dd/MM/yyyy HH:mm');
                    } else return "";
                }
            },

            {
                field: "HastaEntrega", title: "Hasta Entrega", type: "date", format: _DefaultDateTemplate,
                template: function (dataItem) {
                    if (dataItem.HastaEntrega != null) {
                        return '<div></div>' + kendo.toString(kendo.parseDate(dataItem.HastaEntrega, 'yyyy-MM-dd'), 'dd/MM/yyyy HH:mm');
                    } else return "";
                }
            },
            {
                field: "DesdeFijacion", title: "Desde Fijacion", type: "date", format: _DefaultDateTemplate,
                template: function (dataItem) {
                    if (dataItem.DesdeFijacion != null) {
                        return '<div></div>' + kendo.toString(kendo.parseDate(dataItem.DesdeFijacion, 'yyyy-MM-dd'), 'dd/MM/yyyy HH:mm');
                    } else return "";
                }
            },

            {
                field: "HastaFijacion", title: "Hasta Fijacion", type: "date", format: _DefaultDateTemplate,
                template: function (dataItem) {
                    if (dataItem.HastaFijacion != null) {
                        return '<div></div>' + kendo.toString(kendo.parseDate(dataItem.HastaFijacion, 'yyyy-MM-dd'), 'dd/MM/yyyy HH:mm');
                    } else return "";
                }
            },           
            { field: "UsuarioCreador", type: "string", title: "Usuario Creador" },
            {
                field: "FechaCreacion", type: "date", title: "Fecha Creación", format: _DefaultDateTemplate,

                template: function (dataItem) {
                    if (dataItem.FechaCreacion != null) {
                        return '<div></div>' + kendo.toString(kendo.parseDate(dataItem.FechaCreacion, 'yyyy-MM-dd'), 'dd/MM/yyyy HH:mm');
                    } else return "";
                }
            }
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
                    eq: "Igual"
                },
                date: {
                    eq: "Igual",
                    gte: "Despu&eacute;s o igual a",
                    lte: "Antes o igual a"
                },
                number: {
                    eq: "Igual a",
                    gte: "Mayor que o igual a",
                    lte: "Menor que o igual a"
                }
            }
        }
    });
}