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
                url: '/ReporteRangoConfirmacionAutomatica/BuscaDatosTabla'
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
                    PrecioMinimo: { type: "number" },
                    PrecioMaximo: { type: "number" },
                    Material: { type: "string" },
                    Moneda: { type: "string" },
                    FechaDesde: { type: "date" },
                    FechaHasta: { type: "date" },
                    FechaCreacion: { type: "date" },

                }
            }
        },
        serverPaging: true,
        serverSorting: true,
        serverFiltering: true,
        pageSize: 20
    };

    $("#gridRango").kendoGrid({
        toolbar: kendo.template($("#templateToolbar").html()),
        //toolbar: ["excel"],
        excel: {
            fileName: "Reporte Rango Confirmación Automática.xlsx",
            allPages: true,
        },

        excelExport: function (e) {
            var sheet = e.workbook.sheets[0];
            for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
                var row = sheet.rows[rowIndex];
                for (var cellIndex = 0; cellIndex < row.cells.length; cellIndex++) {
                    row.cells[9].format = "yy/MM/dd hh:mm:ss";
                    row.cells[4].format = "yy/MM/dd hh:mm:ss";
                    row.cells[5].format = "yy/MM/dd hh:mm:ss";
                }
            }
        },
        dataSource: ds,
        sortable: true,
        columns: [
            {
                field: "PrecioMinimo", title: "Precio Mínimo", type: "number", template: function (dataItem) {
                    return kendo.toString(dataItem.PrecioMinimo, "##,#.##").replace(/,/g, ".");
                }
            },
            {
                field: "PrecioMaximo", title: "Precio Máximo", type: "number", template: function (dataItem) {
                    return kendo.toString(dataItem.PrecioMaximo, "##,#.##").replace(/,/g, ".");
                }
            },
            { field: "Moneda", type: "string" },
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
                field: "FechaDesde", title: "Fecha Desde", type: "date", format: _DefaultDateTemplate,
                template: "#= kendo.toString(kendo.parseDate(FechaDesde, 'yyyy-MM-dd'), 'dd/MM/yyyy HH:mm') #"
            },

            {
                field: "FechaHasta", title: "Fecha Hasta", type: "date", format: _DefaultDateTemplate,
                template: "#= kendo.toString(kendo.parseDate(FechaHasta, 'yyyy-MM-dd'), 'dd/MM/yyyy HH:mm') #"
            },
            {
                field: "TipoNegocio", type: "string", filterable: {
                    multi: true, dataSource: [{
                        TipoNegocio: "FIJACIÓN"
                    }, {
                        TipoNegocio: "A PRECIO"
                    }]
                }, title: "Tipo Negocio", width: 70, attributes: {
                    "class": "mobile-sm"
                }
            },           
            {
                field: "Cantidad", type: "number", title: "Cantidad (Tn)", template: function (dataItem) {
                    return kendo.toString(dataItem.Cantidad, "##,#.##").replace(/,/g, ".");
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